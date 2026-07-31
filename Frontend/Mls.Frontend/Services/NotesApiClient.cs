using Frontend.Models;
using System.Text.Json;

namespace Frontend.Services
{
    public interface INotesApiClient
    {
        Task<List<NoteViewModel>> GetNotesByPatientAsync(int patientId);
        Task CreateNoteAsync(NoteViewModel note);
    }

    public class NotesApiClient : INotesApiClient
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public NotesApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NoteViewModel>> GetNotesByPatientAsync(int patientId)
        {
            var notes = await _httpClient.GetFromJsonAsync<List<NoteViewModel>>($"api/notes/patient/{patientId}", JsonOptions);
            return notes ?? new List<NoteViewModel>();
        }

        public async Task CreateNoteAsync(NoteViewModel note)
        {
            var response = await _httpClient.PostAsJsonAsync("api/notes", note, JsonOptions);
            response.EnsureSuccessStatusCode();
        }
    }
}
