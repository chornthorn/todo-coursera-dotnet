using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoCourseraApp.Models;

/// <summary>
/// Represents a User with validation attributes
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Username (required, unique, max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email address (required, unique, max 100 characters)
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name (required, max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name (required, max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number (optional, max 20 characters)
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Indicates whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date and time when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date and time when the user was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User's role (default: User)
    /// </summary>
    [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters")]
    public string Role { get; set; } = "User";

    /// <summary>
    /// Full name property (computed)
    /// </summary>
    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>
/// DTO for creating a new user
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// Username (required, max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email address (required, max 100 characters)
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name (required, max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name (required, max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number (optional, max 20 characters)
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's role (optional, default: User)
    /// </summary>
    [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters")]
    public string Role { get; set; } = "User";
}

/// <summary>
/// DTO for updating an existing user
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// Username (required, max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email address (required, max 100 characters)
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// First name (required, max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name (required, max 50 characters)
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number (optional, max 20 characters)
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Indicates whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// User's role (optional, default: User)
    /// </summary>
    [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters")]
    public string Role { get; set; } = "User";
}
