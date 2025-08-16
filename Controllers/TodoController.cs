using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoCourseraApp.Models;
using TodoCourseraApp.Db;
using System.Net.Mime;
using System.ComponentModel.DataAnnotations;

namespace TodoCourseraApp.Controllers;

/// <summary>
/// API endpoints for managing Todo items with full CRUD operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class TodoController : ControllerBase
{
    private readonly TodoDbContext _context;
    private readonly ILogger<TodoController> _logger;

    public TodoController(TodoDbContext context, ILogger<TodoController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all todo items with optional filtering
    /// </summary>
    /// <param name="isCompleted">Filter by completion status (optional)</param>
    /// <param name="category">Filter by category (optional)</param>
    /// <param name="priority">Filter by priority level (optional)</param>
    /// <returns>A list of todo items matching the criteria</returns>
    /// <response code="200">Returns the list of todo items</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Todo>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Todo>>> GetTodos(
        [FromQuery] bool? isCompleted = null,
        [FromQuery] string? category = null,
        [FromQuery] int? priority = null)
    {
        var filteredTodos = _context.Todos.AsQueryable();

        if (isCompleted.HasValue)
        {
            filteredTodos = filteredTodos.Where(t => t.IsCompleted == isCompleted.Value);
        }

        if (!string.IsNullOrEmpty(category))
        {
            filteredTodos = filteredTodos.Where(t =>
                t.Category != null && t.Category.Contains(category, StringComparison.OrdinalIgnoreCase));
        }

        if (priority.HasValue)
        {
            filteredTodos = filteredTodos.Where(t => t.Priority == priority.Value);
        }

        var todos = await filteredTodos.OrderBy(t => t.CreatedAt).ToListAsync();
        return Ok(todos);
    }

    /// <summary>
    /// Retrieves a specific todo item by its ID
    /// </summary>
    /// <param name="id">The ID of the todo item to retrieve</param>
    /// <returns>The todo item with the specified ID</returns>
    /// <response code="200">Returns the requested todo item</response>
    /// <response code="404">If the todo item is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Todo>> GetTodoById(int id)
    {
        var todo = await _context.Todos.FindAsync(id);

        if (todo == null)
        {
            return NotFound($"Todo item with ID {id} not found.");
        }

        return Ok(todo);
    }

    /// <summary>
    /// Creates a new todo item
    /// </summary>
    /// <param name="createTodoDto">The todo item data to create</param>
    /// <returns>The created todo item</returns>
    /// <response code="201">Returns the newly created todo item</response>
    /// <response code="400">If the todo item data is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(Todo), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Todo>> CreateTodo([FromBody] CreateTodoDto createTodoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate due date if provided
        if (createTodoDto.DueDate.HasValue && createTodoDto.DueDate.Value < DateTime.UtcNow)
        {
            ModelState.AddModelError(nameof(createTodoDto.DueDate), "Due date cannot be in the past.");
            return BadRequest(ModelState);
        }

        var todo = new Todo
        {
            Title = createTodoDto.Title,
            Description = createTodoDto.Description,
            Priority = createTodoDto.Priority,
            DueDate = createTodoDto.DueDate,
            Category = createTodoDto.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
    }

    /// <summary>
    /// Updates an existing todo item completely
    /// </summary>
    /// <param name="id">The ID of the todo item to update</param>
    /// <param name="updateTodoDto">The updated todo item data</param>
    /// <returns>The updated todo item</returns>
    /// <response code="200">Returns the updated todo item</response>
    /// <response code="400">If the todo item data is invalid</response>
    /// <response code="404">If the todo item is not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Todo>> UpdateTodo(int id, [FromBody] UpdateTodoDto updateTodoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound($"Todo item with ID {id} not found.");
        }

        // Validate due date if provided
        if (updateTodoDto.DueDate.HasValue && updateTodoDto.DueDate.Value < DateTime.UtcNow)
        {
            ModelState.AddModelError(nameof(updateTodoDto.DueDate), "Due date cannot be in the past.");
            return BadRequest(ModelState);
        }

        // Update the todo item
        todo.Title = updateTodoDto.Title;
        todo.Description = updateTodoDto.Description;
        todo.IsCompleted = updateTodoDto.IsCompleted;
        todo.Priority = updateTodoDto.Priority;
        todo.DueDate = updateTodoDto.DueDate;
        todo.Category = updateTodoDto.Category;
        todo.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(todo);
    }

    /// <summary>
    /// Partially updates an existing todo item
    /// </summary>
    /// <param name="id">The ID of the todo item to update</param>
    /// <param name="patchData">The partial update data</param>
    /// <returns>The updated todo item</returns>
    /// <response code="200">Returns the updated todo item</response>
    /// <response code="400">If the patch data is invalid</response>
    /// <response code="404">If the todo item is not found</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Todo>> PatchTodo(int id, [FromBody] Dictionary<string, object> patchData)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound($"Todo item with ID {id} not found.");
        }

        try
        {
            foreach (var kvp in patchData)
            {
                switch (kvp.Key.ToLowerInvariant())
                {
                    case "title":
                        if (kvp.Value is string title && !string.IsNullOrEmpty(title))
                        {
                            if (title.Length > 200)
                            {
                                return BadRequest("Title cannot exceed 200 characters.");
                            }
                            todo.Title = title;
                        }
                        break;
                    case "description":
                        if (kvp.Value is string description)
                        {
                            if (description.Length > 1000)
                            {
                                return BadRequest("Description cannot exceed 1000 characters.");
                            }
                            todo.Description = description;
                        }
                        break;
                    case "iscompleted":
                        if (kvp.Value is bool isCompleted)
                        {
                            todo.IsCompleted = isCompleted;
                        }
                        break;
                    case "priority":
                        if (kvp.Value is int priority)
                        {
                            if (priority < 1 || priority > 5)
                            {
                                return BadRequest("Priority must be between 1 and 5.");
                            }
                            todo.Priority = priority;
                        }
                        break;
                    case "category":
                        if (kvp.Value is string category)
                        {
                            if (category.Length > 50)
                            {
                                return BadRequest("Category cannot exceed 50 characters.");
                            }
                            todo.Category = category;
                        }
                        break;
                    case "duedate":
                        if (kvp.Value is string dueDateStr && DateTime.TryParse(dueDateStr, out var dueDate))
                        {
                            if (dueDate < DateTime.UtcNow)
                            {
                                return BadRequest("Due date cannot be in the past.");
                            }
                            todo.DueDate = dueDate;
                        }
                        break;
                }
            }

            todo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(todo);
        }
        catch (Exception ex)
        {
            return BadRequest($"Invalid patch data: {ex.Message}");
        }
    }

    /// <summary>
    /// Marks a todo item as completed
    /// </summary>
    /// <param name="id">The ID of the todo item to complete</param>
    /// <returns>The updated todo item</returns>
    /// <response code="200">Returns the completed todo item</response>
    /// <response code="404">If the todo item is not found</response>
    [HttpPatch("{id}/complete")]
    [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Todo>> CompleteTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound($"Todo item with ID {id} not found.");
        }

        todo.IsCompleted = true;
        todo.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(todo);
    }

    /// <summary>
    /// Marks a todo item as incomplete
    /// </summary>
    /// <param name="id">The ID of the todo item to mark as incomplete</param>
    /// <returns>The updated todo item</returns>
    /// <response code="200">Returns the updated todo item</response>
    /// <response code="404">If the todo item is not found</response>
    [HttpPatch("{id}/incomplete")]
    [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Todo>> IncompleteTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound($"Todo item with ID {id} not found.");
        }

        todo.IsCompleted = false;
        todo.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(todo);
    }

    /// <summary>
    /// Deletes a specific todo item
    /// </summary>
    /// <param name="id">The ID of the todo item to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the todo item was successfully deleted</response>
    /// <response code="404">If the todo item is not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTodo(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound($"Todo item with ID {id} not found.");
        }

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Deletes all completed todo items
    /// </summary>
    /// <returns>Number of deleted items</returns>
    /// <response code="200">Returns the number of deleted items</response>
    [HttpDelete("completed")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteCompletedTodos()
    {
        var completedTodos = await _context.Todos.Where(t => t.IsCompleted).ToListAsync();
        var deletedCount = completedTodos.Count;

        _context.Todos.RemoveRange(completedTodos);
        await _context.SaveChangesAsync();

        return Ok(new { deletedCount, message = $"Deleted {deletedCount} completed todo items." });
    }

    /// <summary>
    /// Gets statistics about todo items
    /// </summary>
    /// <returns>Statistics about todo items</returns>
    /// <response code="200">Returns todo statistics</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTodoStats()
    {
        var totalTodos = await _context.Todos.CountAsync();
        var completedTodos = await _context.Todos.CountAsync(t => t.IsCompleted);
        var pendingTodos = totalTodos - completedTodos;
        var overdueTodos = await _context.Todos.CountAsync(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow);

        var priorityBreakdown = await _context.Todos.GroupBy(t => t.Priority)
            .ToDictionaryAsync(g => $"Priority{g.Key}", g => g.Count());

        var categoryBreakdown = await _context.Todos.GroupBy(t => t.Category ?? "Uncategorized")
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        return Ok(new
        {
            totalTodos,
            completedTodos,
            pendingTodos,
            overdueTodos,
            completionRate = totalTodos > 0 ? Math.Round((double)completedTodos / totalTodos * 100, 2) : 0,
            priorityBreakdown,
            categoryBreakdown
        });
    }
}
