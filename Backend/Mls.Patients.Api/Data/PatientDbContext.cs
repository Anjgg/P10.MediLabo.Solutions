using Microsoft.EntityFrameworkCore;
using Mls.Patients.Api.Models;

namespace Mls.Patients.Api.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients => Set<Patient>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
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


            modelBuilder.Entity<Patient>().HasData(
                    new Patient { Id = 1, Nom = "TestNone", Prenom = "Test", DateNaissance = new DateOnly(1966, 12, 31), Genre = Genre.F, Adresse = "1 Brookside St", Telephone = "100-222-3333" },
                    new Patient { Id = 2, Nom = "TestBorderline", Prenom = "Test", DateNaissance = new DateOnly(1945, 6, 24), Genre = Genre.M, Adresse = "2 High St", Telephone = "200-333-4444" },
                    new Patient { Id = 3, Nom = "TestInDanger", Prenom = "Test", DateNaissance = new DateOnly(2004, 6, 18), Genre = Genre.M, Adresse = "3 Club Road", Telephone = "300-444-5555" },
                    new Patient { Id = 4, Nom = "TestEarlyOnse", Prenom = "Test", DateNaissance = new DateOnly(2002, 06, 28), Genre = Genre.F, Adresse = "4 Valley Dr", Telephone = "400-555-6666" });


        }
    }
}
