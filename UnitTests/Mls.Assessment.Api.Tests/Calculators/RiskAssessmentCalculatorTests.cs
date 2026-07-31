using Mls.Assessment.Api.Calculators;
using Mls.Assessment.Api.DTOs;

namespace Mls.Assessment.Api.Tests.Calculators
{
    [TestClass]
    public class RiskAssessmentCalculatorTests
    {
        private RiskAssessmentCalculator _calculator = null!;

        [TestInitialize]
        public void Setup()
        {
            _calculator = new RiskAssessmentCalculator();
        }

        private static DateOnly BirthDateForAge(int age)
        {
            // Anniversaire "aujourd'hui" : garantit un âge exact, sans dépendre
            // de la date d'exécution des tests.
            return DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-age);
        }

        [TestMethod]
        public void ComputeRisk_TestNonePatient_ReturnsNone()
        {
            var patient = new PatientInfo { Id = 1, DateNaissance = BirthDateForAge(59), Genre = Genre.F };
            var notes = new List<NoteInfo>
            {
                new() { Contenu = "Le patient déclare qu'il 'se sent très bien' Poids égal ou inférieur au poids recommandé" }
            };

            var result = _calculator.ComputeRisk(patient, notes);

            Assert.AreEqual("None", result);
        }

        [TestMethod]
        public void ComputeRisk_TestBorderlinePatient_ReturnsBorderline()
        {
            var patient = new PatientInfo { Id = 2, DateNaissance = BirthDateForAge(81), Genre = Genre.M };
            var notes = new List<NoteInfo>
            {
                new() { Contenu = "Le patient déclare qu'il ressent beaucoup de stress au travail Il se plaint également que son audition est anormale dernièrement" },
                new() { Contenu = "Le patient déclare avoir fait une réaction aux médicaments au cours des 3 derniers mois Il remarque également que son audition continue d'être anormale" }
            };

            var result = _calculator.ComputeRisk(patient, notes);

            Assert.AreEqual("Borderline", result);
        }

        [TestMethod]
        public void ComputeRisk_TestInDangerPatient_ReturnsInDanger()
        {
            var patient = new PatientInfo { Id = 3, DateNaissance = BirthDateForAge(22), Genre = Genre.M };
            var notes = new List<NoteInfo>
            {
                new() { Contenu = "Le patient déclare qu'il fume depuis peu" },
                new() { Contenu = "Le patient déclare qu'il est fumeur et qu'il a cessé de fumer l'année dernière Il se plaint également de crises d’apnée respiratoire anormales Tests de laboratoire indiquant un taux de cholestérol LDL élevé" }
            };

            var result = _calculator.ComputeRisk(patient, notes);

            Assert.AreEqual("InDanger", result);
        }

        [TestMethod]
        public void ComputeRisk_TestEarlyOnsetPatient_ReturnsEarlyOnset()
        {
            var patient = new PatientInfo { Id = 4, DateNaissance = BirthDateForAge(24), Genre = Genre.F };
            var notes = new List<NoteInfo>
            {
                new() { Contenu = "Le patient déclare qu'il lui est devenu difficile de monter les escaliers Il se plaint également d’être essoufflé Tests de laboratoire indiquant que les anticorps sont élevés Réaction aux médicaments" },
                new() { Contenu = "Le patient déclare qu'il a mal au dos lorsqu'il reste assis pendant longtemps" },
                new() { Contenu = " Le patient déclare avoir commencé à fumer depuis peu Hémoglobine A1C supérieure au niveau recommandé" },
                new() { Contenu = "Taille, Poids, Cholestérol, Vertige et Réaction" }
            };

            var result = _calculator.ComputeRisk(patient, notes);

            Assert.AreEqual("EarlyOnset", result);
        }

        [TestMethod]
        public void ComputeRisk_NoNotes_ReturnsNone()
        {
            var patient = new PatientInfo { Id = 5, DateNaissance = BirthDateForAge(45), Genre = Genre.M };

            var result = _calculator.ComputeRisk(patient, new List<NoteInfo>());

            Assert.AreEqual("None", result);
        }

        [TestMethod]
        public void ComputeRisk_PoidsEgal_DoesNotCountAsTrigger()
        {
            var patient = new PatientInfo { Id = 6, DateNaissance = BirthDateForAge(40), Genre = Genre.F };
            var notes = new List<NoteInfo>
            {
                new() { Contenu = "Poids égal au poids recommandé" }
            };

            var result = _calculator.ComputeRisk(patient, notes);

            Assert.AreEqual("None", result);
        }

        [TestMethod]
        public void ComputeRisk_PoidsWithoutEgal_CountsAsTrigger()
        {
            // Contrôle : "Poids" seul (sans "égal") doit bien compter comme déclencheur.
            var patient = new PatientInfo { Id = 7, DateNaissance = BirthDateForAge(40), Genre = Genre.F };
            var notes = new List<NoteInfo>
            {
                new() { Contenu = "Poids anormal, Cholestérol élevé" }
            };

            var result = _calculator.ComputeRisk(patient, notes);

            // Poids + Anormal + Cholestérol = 3 occurrences, âge>=30 -> Borderline (2-5)
            Assert.AreEqual("Borderline", result);
        }

        [TestMethod]
        public void ComputeRisk_IsCaseInsensitive()
        {
            var patient = new PatientInfo { Id = 8, DateNaissance = BirthDateForAge(81), Genre = Genre.M };
            var notes = new List<NoteInfo>
            {
                new() { Contenu = "AUDITION EST ANORMALE" },
                new() { Contenu = "audition continue d'être ANORMALE" }
            };

            var result = _calculator.ComputeRisk(patient, notes);

            Assert.AreEqual("Borderline", result);
        }

        [TestMethod]
        public void ComputeRisk_FumeAlone_DoesNotMatchFumeurTrigger()
        {
            var patient = new PatientInfo { Id = 9, DateNaissance = BirthDateForAge(22), Genre = Genre.M };
            var notes = new List<NoteInfo>
            {
                new() { Contenu = "Le patient déclare qu'il fume depuis peu" }
            };

            var result = _calculator.ComputeRisk(patient, notes);

            Assert.AreEqual("None", result);
        }
    }
}
