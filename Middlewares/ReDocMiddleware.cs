using Microsoft.AspNetCore.Http;

namespace TodoCourseraApp.Middleware;

public class ReDocMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;

    public ReDocMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api-docs"))
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(
                Path.Combine(_environment.WebRootPath, "api-docs", "index.html"));
            return;
        }

        await _next(context);
    }
}
