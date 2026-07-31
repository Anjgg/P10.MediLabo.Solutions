using Microsoft.EntityFrameworkCore;
using Mls.Notes.Api.Models;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Mls.Notes.Api.Data
{
    public class NoteDbContext : DbContext
    {
        public NoteDbContext(DbContextOptions<NoteDbContext> options)
            : base(options)
        {
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }

        public DbSet<Note> Notes => Set<Note>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>().ToCollection("Notes");
        }
    }
}
