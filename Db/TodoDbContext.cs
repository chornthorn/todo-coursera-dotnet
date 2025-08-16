using Microsoft.EntityFrameworkCore;
using TodoCourseraApp.Models;

namespace TodoCourseraApp.Db;

/// <summary>
/// Entity Framework DbContext for Todo application
/// </summary>
public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Todo items table
    /// </summary>
    public DbSet<Todo> Todos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Todo entity
        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Category)
                .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            entity.Property(e => e.Priority)
                .IsRequired()
                .HasDefaultValue(1);

            entity.Property(e => e.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Add indexes for better query performance
            entity.HasIndex(e => e.IsCompleted);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.DueDate);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var now = DateTime.UtcNow;

        modelBuilder.Entity<Todo>().HasData(
            new Todo
            {
                Id = 1,
                Title = "Learn ASP.NET Core",
                Description = "Complete the ASP.NET Core tutorial and build a comprehensive API",
                Priority = 3,
                Category = "Learning",
                IsCompleted = false,
                CreatedAt = now.AddDays(-5),
                UpdatedAt = now.AddDays(-5),
                DueDate = now.AddDays(10)
            },
            new Todo
            {
                Id = 2,
                Title = "Build Todo API",
                Description = "Create a comprehensive Todo API with CRUD operations, validation, and middleware",
                Priority = 4,
                Category = "Development",
                IsCompleted = true,
                CreatedAt = now.AddDays(-3),
                UpdatedAt = now.AddDays(-1)
            },
            new Todo
            {
                Id = 3,
                Title = "Write Documentation",
                Description = "Document the API endpoints, authentication methods, and usage examples",
                Priority = 2,
                Category = "Documentation",
                IsCompleted = false,
                CreatedAt = now.AddDays(-2),
                UpdatedAt = now.AddDays(-2),
                DueDate = now.AddDays(7)
            },
            new Todo
            {
                Id = 4,
                Title = "Set up CI/CD Pipeline",
                Description = "Configure automated testing and deployment pipeline",
                Priority = 3,
                Category = "DevOps",
                IsCompleted = false,
                CreatedAt = now.AddDays(-1),
                UpdatedAt = now.AddDays(-1),
                DueDate = now.AddDays(14)
            },
            new Todo
            {
                Id = 5,
                Title = "Code Review",
                Description = "Review and refactor the codebase for best practices",
                Priority = 2,
                Category = "Quality",
                IsCompleted = false,
                CreatedAt = now,
                UpdatedAt = now,
                DueDate = now.AddDays(5)
            }
        );
    }
}
