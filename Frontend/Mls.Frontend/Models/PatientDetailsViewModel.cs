using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class PatientDetailsViewModel
    {
        public PatientViewModel Patient { get; set; } = new();

        public List<NoteViewModel> Notes { get; set; } = new();

        [Required(ErrorMessage = "La note ne peut pas être vide.")]
        [Display(Name = "Nouvelle note")]
        public string NouvelleNote { get; set; } = string.Empty;
    }
}
