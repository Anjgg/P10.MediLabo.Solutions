using Mls.Assessment.Api.DTOs;
using System.Net;

namespace Mls.Assessment.Api.Clients
{
    public interface INotesApiClient
    {
        Task<List<NoteInfo>> GetNotesAsync(int patientId);
    }

    public class NotesApiClient : INotesApiClient
    {
        private readonly HttpClient _httpClient;
        public NotesApiClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<NoteInfo>> GetNotesAsync(int patientId)
        {
            var response = await _httpClient.GetAsync($"api/notes/patient/{patientId}");
            if (response.StatusCode == HttpStatusCode.NotFound) return [];
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<NoteInfo>>() ?? [];
        }
    }
}
