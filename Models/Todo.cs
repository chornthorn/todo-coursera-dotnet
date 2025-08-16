using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoCourseraApp.Models;

/// <summary>
/// Represents a Todo item with validation attributes
/// </summary>
public class Todo
{
    /// <summary>
    /// Unique identifier for the todo item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Title of the todo item (required, max 200 characters)
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the todo item (max 1000 characters)
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether the todo item is completed
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Priority level of the todo item
    /// </summary>
    [Range(1, 5, ErrorMessage = "Priority must be between 1 (low) and 5 (high)")]
    public int Priority { get; set; } = 1;

    /// <summary>
    /// Date and time when the todo item was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the todo item was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional due date for the todo item
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Category or tag for organizing todo items
    /// </summary>
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? Category { get; set; }
}

/// <summary>
/// DTO for creating a new todo item
/// </summary>
public class CreateTodoDto
{
    /// <summary>
    /// Title of the todo item (required, max 200 characters)
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the todo item (max 1000 characters)
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Priority level of the todo item
    /// </summary>
    [Range(1, 5, ErrorMessage = "Priority must be between 1 (low) and 5 (high)")]
    public int Priority { get; set; } = 1;

    /// <summary>
    /// Optional due date for the todo item
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Category or tag for organizing todo items
    /// </summary>
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? Category { get; set; }
}

/// <summary>
/// DTO for updating an existing todo item
/// </summary>
public class UpdateTodoDto
{
    /// <summary>
    /// Title of the todo item (required, max 200 characters)
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the todo item (max 1000 characters)
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether the todo item is completed
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Priority level of the todo item
    /// </summary>
    [Range(1, 5, ErrorMessage = "Priority must be between 1 (low) and 5 (high)")]
    public int Priority { get; set; } = 1;

    /// <summary>
    /// Optional due date for the todo item
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Category or tag for organizing todo items
    /// </summary>
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? Category { get; set; }
}
