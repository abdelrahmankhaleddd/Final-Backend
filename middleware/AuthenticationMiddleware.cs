using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Final.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, AppDbContext dbContext, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            if (!context.Request.Cookies.TryGetValue("access_token", out var tokenValue))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = "Unauthorized user...please login" });
                return;
            }

            // Ensure the token is properly formatted (handling optional "Bearer" prefix)
            var tokenParts = tokenValue.Split(" ");
            var token = tokenParts.Length > 1 ? tokenParts[1] : tokenParts[0];

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid or missing token" });
                return;
            }

            var secretKey = _configuration["Jwt:SecretKey"];
            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true, // Ensures expired tokens are rejected
                ClockSkew = TimeSpan.Zero // Prevents minor time discrepancies
            };

            tokenHandler.ValidateToken(token, tokenValidationParams, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid token payload" });
                return;
            }

            // Fetch user from DB
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = "Unauthorized user" });
                return;
            }

            // Attach user to context
            context.Items["User"] = user;
            await _next(context);
        }
        catch (SecurityTokenExpiredException)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Token expired. Please log in again." });
        }
        catch (SecurityTokenException)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid token." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuthenticationMiddleware encountered an error.");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { message = "An error occurred while processing authentication." });
        }
    }
}
