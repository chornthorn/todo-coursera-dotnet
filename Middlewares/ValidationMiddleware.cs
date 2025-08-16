using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace TodoCourseraApp.Middleware;

/// <summary>
/// Middleware for validating request data and handling validation errors
/// </summary>
public class ValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationMiddleware> _logger;

    public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only validate POST, PUT, and PATCH requests with JSON content
        if (ShouldValidateRequest(context))
        {
            try
            {
                // Enable buffering to allow multiple reads of the request body
                context.Request.EnableBuffering();

                // Read the request body
                var requestBody = await ReadRequestBodyAsync(context.Request);

                if (!string.IsNullOrEmpty(requestBody))
                {
                    // Validate JSON format
                    if (!IsValidJson(requestBody))
                    {
                        await WriteValidationErrorResponse(context, "Invalid JSON format in request body.");
                        return;
                    }

                    // Reset the request body stream position for the next middleware
                    context.Request.Body.Position = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during request validation");
                await WriteValidationErrorResponse(context, "Error processing request data.");
                return;
            }
        }

        // Continue to the next middleware
        await _next(context);
    }

    private static bool ShouldValidateRequest(HttpContext context)
    {
        var method = context.Request.Method.ToUpperInvariant();
        var contentType = context.Request.ContentType?.ToLowerInvariant();

        return (method == "POST" || method == "PUT" || method == "PATCH") &&
               contentType != null &&
               contentType.Contains("application/json");
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0; // Reset position for next read
        return body;
    }

    private static bool IsValidJson(string jsonString)
    {
        try
        {
            JsonDocument.Parse(jsonString);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static async Task WriteValidationErrorResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            error = "Validation Error",
            message = message,
            timestamp = DateTime.UtcNow,
            path = context.Request.Path.Value
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Extension methods for adding validation middleware to the pipeline
/// </summary>
public static class ValidationMiddlewareExtensions
{
    /// <summary>
    /// Adds the validation middleware to the application pipeline
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ValidationMiddleware>();
    }
}
