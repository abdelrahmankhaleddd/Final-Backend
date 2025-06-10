using Final.Models;

public class AdminAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AdminAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Items["User"] is not User user || user.role != UserRole.admin)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new { message = "Unauthorized user" });
            return;
        }

        await _next(context);
    }
}
