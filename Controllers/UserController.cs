using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoCourseraApp.Models;
using TodoCourseraApp.Db;
using System.Net.Mime;

namespace TodoCourseraApp.Controllers;

/// <summary>
/// API endpoints for managing Users with full CRUD operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class UserController : ControllerBase
{
    private readonly TodoDbContext _context;
    private readonly ILogger<UserController> _logger;

    public UserController(TodoDbContext context, ILogger<UserController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all users with optional filtering
    /// </summary>
    /// <param name="isActive">Filter by active status (optional)</param>
    /// <param name="role">Filter by role (optional)</param>
    /// <returns>A list of users matching the criteria</returns>
    /// <response code="200">Returns the list of users</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<User>>> GetUsers(
        [FromQuery] bool? isActive = null,
        [FromQuery] string? role = null)
    {
        var filteredUsers = _context.Users.AsQueryable();

        if (isActive.HasValue)
        {
            filteredUsers = filteredUsers.Where(u => u.IsActive == isActive.Value);
        }

        if (!string.IsNullOrEmpty(role))
        {
            filteredUsers = filteredUsers.Where(u => 
                u.Role.Contains(role, StringComparison.OrdinalIgnoreCase));
        }

        var users = await filteredUsers.OrderBy(u => u.CreatedAt).ToListAsync();
        return Ok(users);
    }

    /// <summary>
    /// Retrieves a specific user by their ID
    /// </summary>
    /// <param name="id">The ID of the user to retrieve</param>
    /// <returns>The user with the specified ID</returns>
    /// <response code="200">Returns the requested user</response>
    /// <response code="404">If the user is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        return Ok(user);
    }

    /// <summary>
    /// Retrieves a specific user by their username
    /// </summary>
    /// <param name="username">The username of the user to retrieve</param>
    /// <returns>The user with the specified username</returns>
    /// <response code="200">Returns the requested user</response>
    /// <response code="404">If the user is not found</response>
    [HttpGet("username/{username}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUserByUsername(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            return NotFound($"User with username '{username}' not found.");
        }

        return Ok(user);
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="createUserDto">The user data to create</param>
    /// <returns>The created user</returns>
    /// <response code="201">Returns the newly created user</response>
    /// <response code="400">If the user data is invalid</response>
    /// <response code="409">If username or email already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if username already exists
        var existingUsername = await _context.Users.AnyAsync(u => u.Username == createUserDto.Username);
        if (existingUsername)
        {
            return Conflict($"Username '{createUserDto.Username}' already exists.");
        }

        // Check if email already exists
        var existingEmail = await _context.Users.AnyAsync(u => u.Email == createUserDto.Email);
        if (existingEmail)
        {
            return Conflict($"Email '{createUserDto.Email}' already exists.");
        }

        var user = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            PhoneNumber = createUserDto.PhoneNumber,
            Role = createUserDto.Role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Updates an existing user completely
    /// </summary>
    /// <param name="id">The ID of the user to update</param>
    /// <param name="updateUserDto">The updated user data</param>
    /// <returns>The updated user</returns>
    /// <response code="200">Returns the updated user</response>
    /// <response code="400">If the user data is invalid</response>
    /// <response code="404">If the user is not found</response>
    /// <response code="409">If username or email already exists for another user</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        // Check if username already exists for another user
        var existingUsername = await _context.Users.AnyAsync(u => u.Username == updateUserDto.Username && u.Id != id);
        if (existingUsername)
        {
            return Conflict($"Username '{updateUserDto.Username}' already exists.");
        }

        // Check if email already exists for another user
        var existingEmail = await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.Id != id);
        if (existingEmail)
        {
            return Conflict($"Email '{updateUserDto.Email}' already exists.");
        }

        // Update the user
        user.Username = updateUserDto.Username;
        user.Email = updateUserDto.Email;
        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.PhoneNumber = updateUserDto.PhoneNumber;
        user.IsActive = updateUserDto.IsActive;
        user.Role = updateUserDto.Role;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(user);
    }

    /// <summary>
    /// Partially updates an existing user
    /// </summary>
    /// <param name="id">The ID of the user to update</param>
    /// <param name="patchData">The partial update data</param>
    /// <returns>The updated user</returns>
    /// <response code="200">Returns the updated user</response>
    /// <response code="400">If the patch data is invalid</response>
    /// <response code="404">If the user is not found</response>
    /// <response code="409">If username or email already exists for another user</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<User>> PatchUser(int id, [FromBody] Dictionary<string, object> patchData)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        try
        {
            foreach (var kvp in patchData)
            {
                switch (kvp.Key.ToLowerInvariant())
                {
                    case "username":
                        if (kvp.Value is string username && !string.IsNullOrEmpty(username))
                        {
                            if (username.Length < 3 || username.Length > 50)
                            {
                                return BadRequest("Username must be between 3 and 50 characters.");
                            }
                            
                            var existingUsername = await _context.Users.AnyAsync(u => u.Username == username && u.Id != id);
                            if (existingUsername)
                            {
                                return Conflict($"Username '{username}' already exists.");
                            }
                            
                            user.Username = username;
                        }
                        break;
                    case "email":
                        if (kvp.Value is string email && !string.IsNullOrEmpty(email))
                        {
                            if (email.Length > 100)
                            {
                                return BadRequest("Email cannot exceed 100 characters.");
                            }
                            
                            var existingEmail = await _context.Users.AnyAsync(u => u.Email == email && u.Id != id);
                            if (existingEmail)
                            {
                                return Conflict($"Email '{email}' already exists.");
                            }
                            
                            user.Email = email;
                        }
                        break;
                    case "firstname":
                        if (kvp.Value is string firstName && !string.IsNullOrEmpty(firstName))
                        {
                            if (firstName.Length > 50)
                            {
                                return BadRequest("First name cannot exceed 50 characters.");
                            }
                            user.FirstName = firstName;
                        }
                        break;
                    case "lastname":
                        if (kvp.Value is string lastName && !string.IsNullOrEmpty(lastName))
                        {
                            if (lastName.Length > 50)
                            {
                                return BadRequest("Last name cannot exceed 50 characters.");
                            }
                            user.LastName = lastName;
                        }
                        break;
                    case "phonenumber":
                        if (kvp.Value is string phoneNumber)
                        {
                            if (phoneNumber.Length > 20)
                            {
                                return BadRequest("Phone number cannot exceed 20 characters.");
                            }
                            user.PhoneNumber = phoneNumber;
                        }
                        break;
                    case "isactive":
                        if (kvp.Value is bool isActive)
                        {
                            user.IsActive = isActive;
                        }
                        break;
                    case "role":
                        if (kvp.Value is string role)
                        {
                            if (role.Length > 20)
                            {
                                return BadRequest("Role cannot exceed 20 characters.");
                            }
                            user.Role = role;
                        }
                        break;
                }
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest($"Invalid patch data: {ex.Message}");
        }
    }

    /// <summary>
    /// Activates a user account
    /// </summary>
    /// <param name="id">The ID of the user to activate</param>
    /// <returns>The updated user</returns>
    /// <response code="200">Returns the activated user</response>
    /// <response code="404">If the user is not found</response>
    [HttpPatch("{id}/activate")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> ActivateUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    /// <summary>
    /// Deactivates a user account
    /// </summary>
    /// <param name="id">The ID of the user to deactivate</param>
    /// <returns>The updated user</returns>
    /// <response code="200">Returns the deactivated user</response>
    /// <response code="404">If the user is not found</response>
    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> DeactivateUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    /// <summary>
    /// Deletes a specific user
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the user was successfully deleted</response>
    /// <response code="404">If the user is not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Gets statistics about users
    /// </summary>
    /// <returns>Statistics about users</returns>
    /// <response code="200">Returns user statistics</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetUserStats()
    {
        var totalUsers = await _context.Users.CountAsync();
        var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
        var inactiveUsers = totalUsers - activeUsers;

        var roleBreakdown = await _context.Users.GroupBy(u => u.Role)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        var recentUsers = await _context.Users.CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30));

        return Ok(new
        {
            totalUsers,
            activeUsers,
            inactiveUsers,
            recentUsers,
            activationRate = totalUsers > 0 ? Math.Round((double)activeUsers / totalUsers * 100, 2) : 0,
            roleBreakdown
        });
    }
}
