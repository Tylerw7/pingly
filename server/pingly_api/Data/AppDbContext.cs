using Microsoft.EntityFrameworkCore;
using pingly_api.Models; 


namespace pingly_api.Data;

public class AppDbContext : DbContext
{
    // Constructor
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Topic.Name must be globally unique — this is what makes topic
        // names function as identifiers. Postgres creates a unique index
        // that will reject duplicate INSERTs.
        modelBuilder.Entity<Topic>()
            .HasIndex(t => t.Name)
            .IsUnique();

        // When a topic is deleted, delete all its messages too.
        // Postgres will enforce this via ON DELETE CASCADE at the FK level,
        // so the database does the work — no orphaned messages possible.
        modelBuilder.Entity<Topic>()
            .HasMany(t => t.Messages)
            .WithOne()
            .HasForeignKey(m => m.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }



}