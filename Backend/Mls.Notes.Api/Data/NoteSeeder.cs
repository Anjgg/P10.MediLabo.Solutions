using Mls.Notes.Api.Models;

namespace Mls.Notes.Api.Data
{
    /// <summary>
    /// Seed des notes de test pour les 4 patients du sprint 1 (TestNone=1, TestBorderline=2,
    /// TestInDanger=3, TestEarlyOnset=4).
    /// TODO: remplacer ce contenu par le texte réel fourni dans le fichier
    /// "NET P10 Sprint 2 - notes pour les 4 cas de test.xlsx".
    /// </summary>
    public static class NoteSeeder
    {
        public static void SeedIfEmpty(NoteDbContext dbContext)
        {
            if (dbContext.Notes.Any())
            {
                return;
            }

            dbContext.Notes.AddRange(
                new Note { PatientId = 1, Contenu = "Le patient déclare qu'il 'se sent très bien' Poids égal ou inférieur au poids recommandé", DateCreation = DateTime.UtcNow },
                new Note { PatientId = 2, Contenu = "Le patient déclare qu'il ressent beaucoup de stress au travail Il se plaint également que son audition est anormale dernièrement", DateCreation = DateTime.UtcNow },
                new Note { PatientId = 2, Contenu = "Le patient déclare avoir fait une réaction aux médicaments au cours des 3 derniers mois Il remarque également que son audition continue d'être anormale", DateCreation = DateTime.UtcNow },
                new Note { PatientId = 3, Contenu = "Le patient déclare qu'il fume depuis peu", DateCreation = DateTime.UtcNow },
                new Note { PatientId = 3, Contenu = "Le patient déclare qu'il est fumeur et qu'il a cessé de fumer l'année dernière Il se plaint également de crises d’apnée respiratoire anormales Tests de laboratoire indiquant un taux de cholestérol LDL élevé", DateCreation = DateTime.UtcNow },
                new Note { PatientId = 4, Contenu = "Le patient déclare qu'il lui est devenu difficile de monter les escaliers Il se plaint également d’être essoufflé Tests de laboratoire indiquant que les anticorps sont élevés Réaction aux médicaments", DateCreation = DateTime.UtcNow },
                new Note { PatientId = 4, Contenu = "Le patient déclare qu'il a mal au dos lorsqu'il reste assis pendant longtemps", DateCreation = DateTime.UtcNow },
                new Note { PatientId = 4, Contenu = " Le patient déclare avoir commencé à fumer depuis peu Hémoglobine A1C supérieure au niveau recommandé", DateCreation = DateTime.UtcNow },
                new Note { PatientId = 4, Contenu = "Taille, Poids, Cholestérol, Vertige et Réaction", DateCreation = DateTime.UtcNow },
            );

            dbContext.SaveChanges();
        }
    }
}
