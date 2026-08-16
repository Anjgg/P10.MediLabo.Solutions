# P10.MediLabo.Solutions

## Présentation

MediLabo Solutions est une application médicale destinée aux organisateurs de dépistage du
diabète de type 2, permettant de centraliser les dossiers patients, leurs notes de visite et
d'évaluer automatiquement leur niveau de risque diabétique.

Le projet est découpé en microservices .NET :

- **Mls.Patients.Api** : gestion des dossiers patients (SQL Server).
- **Mls.Notes.Api** : historique des notes de consultation par patient (MongoDB).
- **Mls.Assessment.Api** : calcule le niveau de risque diabétique d'un patient (None,
  Borderline, InDanger, EarlyOnset) à partir de ses données démographiques et du contenu de ses
  notes, en interrogeant directement Patients et Notes.
- **Mls.Gateway** : point d'entrée unique (Ocelot) exposé au Frontend, qui route vers les
  différentes API.
- **Mls.Frontend** : application web (ASP.NET Core MVC) consultée par le personnel soignant pour
  consulter/créer des patients, ajouter des notes et visualiser le niveau de risque calculé.

Les services communiquent en HTTP avec authentification Basic, et une bibliothèque partagée
(`Mls.Library`) mutualise l'authentification et l'accès aux données entre les microservices.

## Green Code

### Constat
Dans `Mls.Assessment.Api`, le service `AssessmentService.GetAssessmentAsync` interroge
`Mls.Patients.Api` puis `Mls.Notes.Api` de manière séquentielle, alors que la récupération
des notes ne dépend pas de la réponse du service Patients (seule l'existence du patient est
requise, pas son contenu). Ce chaînage inutile double le temps de latence réseau perçu par
l'appelant et prolonge la durée de vie de la requête HTTP entrante, donc la consommation de
ressources (threads, mémoire, connexions) côté serveur pour chaque appel.

### Piste d'amélioration
Paralléliser les deux appels indépendants avec `Task.WhenAll` une fois l'existence du patient
confirmée, plutôt que de les enchaîner :

```csharp
var patient = await _patientsClient.GetPatientAsync(patientId);
if (patient is null) return null;

var notes = await _notesClient.GetNotesAsync(patientId);
return _riskCalculator.ComputeRisk(patient, notes);
```

devient, si on accepte de récupérer patient et notes en parallèle puis de vérifier l'existence
après coup (nécessite d'adapter légèrement la logique de garde) :

```csharp
var patientTask = _patientsClient.GetPatientAsync(patientId);
var notesTask = _notesClient.GetNotesAsync(patientId);
await Task.WhenAll(patientTask, notesTask);

var patient = patientTask.Result;
if (patient is null) return null;

return _riskCalculator.ComputeRisk(patient, notesTask.Result);
```

Sur un grand volume de requêtes (ex. affichage en masse des risques dans une liste de patients),
ce changement réduit le temps CPU/réseau cumulé et donc l'empreinte énergétique du service.

### Constat
Dans `Mls.Notes.Api/Program.cs`, `NoteDbContext` est enregistré via
`builder.Services.AddDbContext<NoteDbContext>(options => options.UseMongoDB(connectionString, "NotesDb"))`.
`AddDbContext` enregistre le `DbContext` avec une durée de vie *Scoped* : une nouvelle instance
est donc créée à chaque requête HTTP. La surcharge `UseMongoDB(string connectionString, string databaseName)`
construit son client MongoDB à partir de la chaîne de connexion à chaque configuration du contexte,
au lieu de réutiliser un client unique. Or `IMongoClient` encapsule un pool de connexions TCP et est
conçu pour être partagé comme une instance unique pour toute la durée de vie de l'application. Recréer
implicitement ce client à chaque requête, sous forte charge, multiplie les pools de connexions ouverts
en parallèle au lieu d'en réutiliser un seul, ce qui consomme inutilement sockets, mémoire et CPU
côté serveur comme côté MongoDB.

### Piste d'amélioration
Enregistrer un `IMongoClient` unique en singleton et le fournir explicitement à `UseMongoDB`, afin que
tous les `NoteDbContext` (un par requête) réutilisent le même pool de connexions au lieu d'en ouvrir un
nouveau à chaque fois :

```csharp
builder.Services.AddSingleton<IMongoClient>(
    new MongoClient(builder.Configuration.GetConnectionString("NotesDb")));

builder.Services.AddDbContext<NoteDbContext>((sp, options) =>
    options.UseMongoDB(sp.GetRequiredService<IMongoClient>(), "NotesDb"));
```

Cette approche garantit qu'un seul pool de connexions est ouvert et réutilisé quel que soit le nombre
de requêtes concurrentes, ce qui réduit la consommation de ressources sous forte charge.

### Constat
La liste des patients n'est paginée à aucun niveau de la chaîne. `PatientController.GetPatients()`
appelle `PatientService.GetAllPatientsAsync()`, qui appelle `IRepository<Patient, int>.GetAllAsync()`,
qui exécute `_dbSet.ToListAsync()` dans `Repository.cs` : toute la table `Patients` est chargée en
mémoire et renvoyée en une seule réponse, sans `Skip`/`Take` ni paramètre de pagination. Côté
`Mls.Frontend`, `Views/Patients/Index.cshtml` affiche ensuite l'intégralité de cette liste dans un
unique tableau HTML. Cette page étant le point d'entrée principal du parcours patient, chaque visite
déclenche un scan complet de la table et un transfert réseau de tout le dataset, même quand
l'utilisateur ne consulte qu'un petit nombre de patients — un coût qui croît sans borne avec le volume
de données.

### Piste d'amélioration
Introduire une pagination côté API (paramètres `page`/`pageSize`, requête `Skip().Take()` sur le
`DbSet`) et l'exposer côté Frontend avec une navigation par page, afin de ne charger et transférer que
les patients réellement affichés à l'écran :

```csharp
public async Task<IEnumerable<TEntity>> GetPagedAsync(int page, int pageSize)
{
    return await _dbSet
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
```

Cela réduit la charge sur la base de données, le volume de données transférées sur le réseau et le
travail de rendu côté client à chaque consultation de la liste.