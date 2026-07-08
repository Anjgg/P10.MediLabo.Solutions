using Microsoft.EntityFrameworkCore;

namespace Patient.API.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Patient> Patients => Set<Models.Patient>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Patient>(entity =>
            {
                entity.ToTable("Patients");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nom).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Prenom).IsRequired().HasMaxLength(50);
                entity.Property(p => p.DateNaissance).IsRequired();
                entity.Property(p => p.Genre)
                      .IsRequired()
                      .HasMaxLength(1)
                      .HasConversion<string>();
                entity.Property(p => p.Adresse).HasMaxLength(200);
                entity.Property(p => p.Telephone).HasMaxLength(20);
            });
        }
}
