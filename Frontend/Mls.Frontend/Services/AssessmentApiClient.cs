using Frontend.Models;
using Mls.Frontend.Models;
using System.Text.Json;

namespace Frontend.Services
{
    public interface IAssessmentApiClient
    {
        Task<AssessmentViewModel?> GetAssessmentAsync(int patientId);
    }

    public class AssessmentApiClient : IAssessmentApiClient
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public AssessmentApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AssessmentViewModel?> GetAssessmentAsync(int patientId)
        {
            var response = await _httpClient.GetAsync($"api/assessment/{patientId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AssessmentViewModel>(JsonOptions);
        }
    }
}