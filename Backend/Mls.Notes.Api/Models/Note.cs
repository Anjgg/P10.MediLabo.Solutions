using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace Mls.Notes.Api.Models
{
    public class Note
    {
        public ObjectId Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public string Contenu { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}
