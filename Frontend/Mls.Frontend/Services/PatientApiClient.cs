using Frontend.Models;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Frontend.Services
{
    public interface IPatientApiClient
    {
        Task<List<PatientViewModel>> GetPatientsAsync();
        Task<PatientViewModel?> GetPatientAsync(int id);
        Task CreatePatientAsync(PatientViewModel patient);
        Task UpdatePatientAsync(int id, PatientViewModel patient);
    }

    public class PatientApiClient : IPatientApiClient
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public PatientApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PatientViewModel>> GetPatientsAsync()
        {
            var patients = await _httpClient.GetFromJsonAsync<List<PatientViewModel>>("api/patients", JsonOptions);
            return patients ?? new List<PatientViewModel>();
        }

        public async Task<PatientViewModel?> GetPatientAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/patients/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PatientViewModel>(JsonOptions);
        }

        public async Task CreatePatientAsync(PatientViewModel patient)
        {
            var response = await _httpClient.PostAsJsonAsync("api/patients", patient, JsonOptions);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdatePatientAsync(int id, PatientViewModel patient)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/patients/{id}", patient, JsonOptions);
            response.EnsureSuccessStatusCode();
        }
    }
}
