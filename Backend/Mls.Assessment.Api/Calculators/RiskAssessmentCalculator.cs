using Mls.Assessment.Api.DTOs;

namespace Mls.Assessment.Api.Calculators
{
    public interface IRiskAssessmentCalculator
    {
        string ComputeRisk(PatientInfo patient, IEnumerable<NoteInfo> notes);
    }

    public class RiskAssessmentCalculator : IRiskAssessmentCalculator
    {
        private static readonly string[] TriggerTerms =
        {
        "Hémoglobine A1C", "Microalbumine", "Taille", "Poids",
        "Fumeur", "Fumeuse", "Anormal", "Cholestérol",
        "Vertiges", "Rechute", "Réaction", "Anticorps"
        };

        public string ComputeRisk(PatientInfo patient, IEnumerable<NoteInfo> notes)
        {
            int triggerCount = 0;

            foreach (var note in notes)
            {
                var text = note.Contenu;
                var textLower = text.ToLowerInvariant();

                foreach (var term in TriggerTerms)
                {
                    var termLower = term.ToLowerInvariant();
                    if (!textLower.Contains(termLower))
                        continue;
                    
                    if (termLower == "poids" && textLower.Contains("poids égal"))
                        continue;

                    triggerCount += CountOccurrences(textLower, termLower);
                }
            }

            var age = CalculateAge(patient.DateNaissance);
            var isMale = patient.Genre == Genre.M;

            if (age >= 30)
            {
                if (triggerCount >= 8) return "EarlyOnset";
                if (triggerCount >= 6) return "InDanger";
                if (triggerCount >= 2) return "Borderline";
                return "None";
            }

            if (isMale)
            {
                if (triggerCount >= 5) return "EarlyOnset";
                if (triggerCount >= 3) return "InDanger";
                return "None";
            }

            if (triggerCount >= 7) return "EarlyOnset";
            if (triggerCount >= 4) return "InDanger";
            return "None";
        }

        private static int CountOccurrences(string text, string term)
        {
            int count = 0, index = 0;
            while ((index = text.IndexOf(term, index, StringComparison.Ordinal)) != -1)
            {
                count++;
                index += term.Length;
            }
            return count;
        }

        private static int CalculateAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age;
        }
    }
}
