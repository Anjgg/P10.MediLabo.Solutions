namespace Mls.Assessment.Api.DTOs
{
    public class PatientInfo
    {
        public int Id { get; set; }
        public DateOnly DateNaissance { get; set; }
        public Genre Genre { get; set; } 
    }

    public enum Genre
    {
        M = 0,
        F = 1
    }
}
