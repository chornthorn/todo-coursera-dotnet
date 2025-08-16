using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace TodoCourseraApp.Middleware;

/// <summary>
/// Simple authentication middleware with static username/password
/// </summary>
public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    // Static credentials for demo purposes
    private const string ValidUsername = "admin";
    private const string ValidPassword = "admin123";

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add request ID to response headers for tracking
        var requestId = Guid.NewGuid().ToString("N")[..8];
        context.Response.Headers["X-Request-ID"] = requestId;

        // Skip authentication for certain paths
        if (ShouldSkipAuthentication(context.Request.Path))
        {
            await _next(context);
            return;
        }

        try
        {
            if (!IsAuthenticated(context))
            {
                await WriteUnauthorizedResponse(context);
                return;
            }

            // Set user identity
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, ValidUsername),
                new Claim(ClaimTypes.AuthenticationMethod, "Basic")
            };
            var identity = new ClaimsIdentity(claims, "Basic");
            context.User = new ClaimsPrincipal(identity);

            _logger.LogDebug("Request authenticated successfully for user: {Username}", ValidUsername);

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during authentication");
            await WriteUnauthorizedResponse(context);
        }
    }

    private static bool ShouldSkipAuthentication(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant();
        
        // Skip authentication for these paths
        var skipPaths = new[]
        {
            "/swagger",
            "/swagger-ui",
            "/api-docs",
            "/redoc",
            "/health",
            "/favicon.ico"
        };

        return skipPaths.Any(skipPath => pathValue?.StartsWith(skipPath) == true);
    }

    private bool IsAuthenticated(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            return false;

        var authValue = authHeader.FirstOrDefault();
        if (string.IsNullOrEmpty(authValue) || !authValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            return false;

        try
        {
            var encodedCredentials = authValue["Basic ".Length..].Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var credentials = decodedCredentials.Split(':', 2);

            return credentials.Length == 2 && 
                   credentials[0] == ValidUsername && 
                   credentials[1] == ValidPassword;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Invalid Basic authentication format");
            return false;
        }
    }

    private static async Task WriteUnauthorizedResponse(HttpContext context)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            error = "Unauthorized",
            message = "Basic authentication required. Use username: admin, password: admin123",
            timestamp = DateTime.UtcNow,
            path = context.Request.Path.Value,
            example = "Authorization: Basic YWRtaW46YWRtaW4xMjM="
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Extension methods for adding authentication middleware to the pipeline
/// </summary>
public static class AuthenticationMiddlewareExtensions
{
    /// <summary>
    /// Adds the custom authentication middleware to the application pipeline
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationMiddleware>();
    }
}
