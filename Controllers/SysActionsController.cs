using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Ensures only authorized users can access this controller
public class SysActionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SysActionsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/sysactions
    [HttpGet]
    public async Task<IActionResult> ShowAllSysActions()
    {
        try
        {
            var allSysActions = await _context.SysActions
                .Include(s => s.user)
                .OrderByDescending(s => s.creationTime)
                .Select(s => new
                {
                    s.Id,
                    s.action,
                    s.creationTime,
                    User = new
                    {
                        s.user.Id,
                        s.user.userName,
                        s.user.email,
                        s.user.image
                    }
                })
                .ToListAsync();

            // Log the admin's action
            var adminId = GetCurrentUserId();
            var adminEmail = GetCurrentUserEmail();
            await LogSystemAction(adminId, $"{adminEmail} (admin) viewed all system actions");

            return Accepted(new { allSysActions });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // GET: api/sysactions/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> ShowSysActionsOfSpecificUser(int userId)
    {
        try
        {
            var userActions = await _context.SysActions
                .Where(s => s.userId == userId)
                .Include(s => s.user)
                .OrderByDescending(s => s.creationTime)
                .Select(s => new
                {
                    s.Id,
                    s.action,
                    s.creationTime,
                    User = new
                    {
                        s.user.Id,
                        s.user.userName,
                        s.user.email,
                        s.user.image
                    }
                })
                .ToListAsync();

            if (!userActions.Any())
            {
                return NotFound(new { message = "No actions found for this user" });
            }

            var user = await _context.Users.FindAsync(userId);

            // Log the admin's action
            var adminId = GetCurrentUserId();
            var adminEmail = GetCurrentUserEmail();
            await LogSystemAction(adminId, $"{adminEmail} (admin) viewed actions of a specific user");

            if (user == null)
            {
                await _context.SysActions
                    .Where(s => s.userId == userId)
                    .ExecuteDeleteAsync();

                return NotFound(new { message = "User not found or deleted. Their actions have been removed." });
            }

            return Accepted(new { userActions });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private async Task LogSystemAction(int userId, string action)
    {
        var sysAction = new SysAction
        {
            userId = userId,
            action = action,
            creationTime = DateTime.UtcNow
        };

        _context.SysActions.Add(sysAction);
        await _context.SaveChangesAsync();
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

    private string GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value;
    }
}
