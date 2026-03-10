using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Story> Stories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Enable the pgvector extension in the database
            modelBuilder.HasPostgresExtension("vector");

            modelBuilder.Entity<Story>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Map the Vector property to a pgvector column with 768 dimensions
                entity.Property(e => e.Embedding)
                      .HasColumnType("vector(768)");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
