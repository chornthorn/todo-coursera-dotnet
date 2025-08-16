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

    /// <summary>
    /// Users table
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

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

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("User");

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Add unique constraints
            entity.HasIndex(e => e.Username)
                .IsUnique();

            entity.HasIndex(e => e.Email)
                .IsUnique();

            // Add indexes for better query performance
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Role);
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

        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@todoapp.com",
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                IsActive = true,
                CreatedAt = now.AddDays(-10),
                UpdatedAt = now.AddDays(-10)
            },
            new User
            {
                Id = 2,
                Username = "johndoe",
                Email = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "+1-555-0123",
                Role = "User",
                IsActive = true,
                CreatedAt = now.AddDays(-7),
                UpdatedAt = now.AddDays(-7)
            },
            new User
            {
                Id = 3,
                Username = "janedoe",
                Email = "jane.doe@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                PhoneNumber = "+1-555-0124",
                Role = "User",
                IsActive = true,
                CreatedAt = now.AddDays(-5),
                UpdatedAt = now.AddDays(-5)
            }
        );
    }
}
