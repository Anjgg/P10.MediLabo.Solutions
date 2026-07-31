using Mls.Assessment.Api.DTOs;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mls.Assessment.Api.Clients
{
    public interface IPatientsApiClient
    {
        Task<PatientInfo?> GetPatientAsync(int patientId);
    }

    public class PatientsApiClient : IPatientsApiClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };
        public PatientsApiClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<PatientInfo?> GetPatientAsync(int patientId)
        {
            var response = await _httpClient.GetAsync($"api/patients/{patientId}");
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PatientInfo>(_jsonOptions);
        }
    }
}
