using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    public class PatientViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est requis.")]
        [MaxLength(50)]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est requis.")]
        [MaxLength(50)]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de naissance est requise.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date de naissance")]
        public DateOnly? DateNaissance { get; set; }

        [Required(ErrorMessage = "Le genre est requis.")]
        [Display(Name = "Genre")]
        public Genre? Genre { get; set; }

        [MaxLength(200)]
        [Display(Name = "Adresse")]
        public string? Adresse { get; set; }

        [MaxLength(20)]
        [Display(Name = "Téléphone")]
        public string? Telephone { get; set; }
    }
}
