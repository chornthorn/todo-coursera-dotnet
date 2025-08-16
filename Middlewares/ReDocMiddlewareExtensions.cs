namespace TodoCourseraApp.Middleware;

public static class ReDocMiddlewareExtensions
{
    public static IApplicationBuilder UseReDoc(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ReDocMiddleware>();
    }
}
