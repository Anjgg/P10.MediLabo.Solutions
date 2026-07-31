using Microsoft.EntityFrameworkCore;
using Mls.Notes.Api.Data;
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
        private readonly NoteDbContext _dbContext;

        public NoteService(NoteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<NoteDto>> GetNotesByPatientIdAsync(int patientId)
        {
            var notes = await _dbContext.Notes
                .Where(n => n.PatientId == patientId)
                .OrderByDescending(n => n.DateCreation)
                .ToListAsync();

            return notes.Select(ToDto);
        }

        public async Task<NoteDto?> GetNoteByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return null;
            }

            var note = await _dbContext.Notes.FirstOrDefaultAsync(n => n.Id == objectId);

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

            await _dbContext.Notes.AddAsync(note);
            await _dbContext.SaveChangesAsync();

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
