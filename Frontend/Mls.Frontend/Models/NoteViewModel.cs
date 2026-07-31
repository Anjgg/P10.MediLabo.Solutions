using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class NoteViewModel
    {
        public string? Id { get; set; }

        public int PatientId { get; set; }

        [Required(ErrorMessage = "La note ne peut pas être vide.")]
        [Display(Name = "Note")]
        public string Contenu { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; }
    }
}
