using System.ComponentModel.DataAnnotations;

namespace Mls.Notes.Api.DTOs
{
    public class NoteDto
    {
        public string? Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public string Contenu { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; }
    }
}
