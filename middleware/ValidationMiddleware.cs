using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IValidator<object> validator)
    {
        var validationResult = await validator.ValidateAsync(context.Request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });

            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(errors));
            return;
        }

        await _next(context);
    }
}
