using Mls.Assessment.Api.Calculators;
using Mls.Assessment.Api.Clients;

namespace Mls.Assessment.Api.Services
{
    public interface IAssessmentService
    {
        Task<string?> GetAssessmentAsync(int patientId);
    }
    public class AssessmentService : IAssessmentService
    {
        private readonly IPatientsApiClient _patientsClient;
        private readonly INotesApiClient _notesClient;
        private readonly IRiskAssessmentCalculator _riskCalculator;


        public AssessmentService(IPatientsApiClient patientsClient, INotesApiClient notesClient, IRiskAssessmentCalculator riskCalculator)
        {
            _patientsClient = patientsClient;
            _notesClient = notesClient;
            _riskCalculator = riskCalculator;
        }

        public async Task<string?> GetAssessmentAsync(int patientId)
        {
            var patient = await _patientsClient.GetPatientAsync(patientId);

            if (patient is null)
            {
                return null;
            }

            var notes = await _notesClient.GetNotesAsync(patientId);

            var assessment = _riskCalculator.ComputeRisk(patient, notes);

            return assessment;
        }
    }
}
