using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Story> Stories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<Story>()
            .Property(s => s.Embedding)
            .HasColumnType("vector(768)");

        modelBuilder.Entity<Story>()
            .HasIndex(s => s.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
    }
}