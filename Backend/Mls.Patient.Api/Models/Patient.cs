using System.ComponentModel.DataAnnotations;

namespace Mls.Patient.Api.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        public DateOnly DateNaissance { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [MaxLength(200)]
        public string? Adresse { get; set; }

        [MaxLength(20)]
        public string? Telephone { get; set; }
    }

    public enum Genre
    {
        M = 0,
        F = 1
    }
}
