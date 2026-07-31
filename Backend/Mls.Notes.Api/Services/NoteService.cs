using Mls.Library.Repositories;
using Mls.Notes.Api.DTOs;
using Mls.Notes.Api.Models;
using MongoDB.Bson;

namespace Mls.Notes.Api.Services
{
    public interface INoteService
    {
        Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId);
        Task<NoteDto?> GetNoteByIdAsync(string id);
        Task<NoteDto> CreateNoteAsync(NoteDto dto);
    }

    public class NoteService : INoteService
    {
        private readonly IRepository<Note, ObjectId> _repository;

        public NoteService(IRepository<Note, ObjectId> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId)
        {
            var notes = await _repository.FindAsync(n => n.PatientId == patientId);

            return notes
                .OrderByDescending(n => n.DateCreation)
                .Select(ToDto);
        }

        public async Task<NoteDto?> GetNoteByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return null;
            }

            var note = await _repository.GetByIdAsync(objectId);

            return note is null ? null : ToDto(note);
        }

        public async Task<NoteDto> CreateNoteAsync(NoteDto dto)
        {
            var note = new Note
            {
                PatientId = dto.PatientId,
                Contenu = dto.Contenu,
                DateCreation = DateTime.UtcNow
            };

            await _repository.AddAsync(note);
            await _repository.SaveChangesAsync();

            return ToDto(note);
        }

        private static NoteDto ToDto(Note note)
        {
            return new NoteDto
            {
                Id = note.Id.ToString(),
                PatientId = note.PatientId,
                Contenu = note.Contenu,
                DateCreation = note.DateCreation
            };
        }
    }
}
