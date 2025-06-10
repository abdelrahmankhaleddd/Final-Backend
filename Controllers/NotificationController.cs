using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Final.Models; // Ensure you have your models defined here
using Final.Repositories; // Ensure you have your repositories defined here
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace Final.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    //[Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ISystemActionRepository _sysActionRepository;

        public NotificationController(AppDbContext context, ISystemActionRepository sysActionRepository)
        {
            _context = context;
            _sysActionRepository = sysActionRepository;
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> ShowAllNotifications()
        {
            try
            {
                var allNotifications = await _context.Notifications.ToListAsync();

                // Log the action
                var userId = int.Parse(User.GetUserId());
                await _sysActionRepository.LogActionAsync($"{User.FindFirst(ClaimTypes.Email)?.Value} showed all notifications", userId);

                return Ok(new { AllNotifications = allNotifications });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error.Message });
            }
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> ShowMyNotifications()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var allMyNotifications = await _context.Notifications.Where(n => n.projectOwner == userEmail).ToListAsync();

                // Log the action
                var userId = int.Parse(User.GetUserId());
                await _sysActionRepository.LogActionAsync($"{userEmail} showed all their notifications", userId);

                return Ok(new { AllMyNotifications = allMyNotifications });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteNotification([FromBody] DeleteNotificationDto deleteNotificationDto)
        {
            try
            {
                if (deleteNotificationDto.NotificationID == null)
                {
                    return BadRequest("No notification ID provided.");
                }

                var notificationToDelete = await _context.Notifications.FindAsync(deleteNotificationDto.NotificationID);
                if (notificationToDelete == null)
                {
                    return NotFound("No notification found with this ID.");
                }

                _context.Notifications.Remove(notificationToDelete);
                await _context.SaveChangesAsync();

                // Log the action
                var userId = int.Parse(User.GetUserId());
                await _sysActionRepository.LogActionAsync($"{User.FindFirst(ClaimTypes.Email)?.Value} deleted a notification", userId);

                return Ok(new { message = "Notification deleted successfully.", DeletedNotification = notificationToDelete });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error.Message });
            }
        }

        [Authorize]
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllMyNotifications()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var notificationsToDelete = await _context.Notifications.Where(n => n.projectOwner == userEmail).ToListAsync();

                if (!notificationsToDelete.Any())
                {
                    return NotFound(new { message = "No notifications found for this user." });
                }

                _context.Notifications.RemoveRange(notificationsToDelete);
                await _context.SaveChangesAsync();

                // Log the action
                var userId = int.Parse(User.GetUserId());
                await _sysActionRepository.LogActionAsync($"{userEmail} deleted all their notifications", userId);

                return Ok(new { message = "All notifications deleted successfully.", DeletedNotificationsCount = notificationsToDelete.Count });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize]
        [HttpGet("interactions")]
        public async Task<IActionResult> ShowMyInteractions()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var allMyInteractions = await _context.Notifications.Where(n => n.eventOwner == userEmail).ToListAsync();

                // Log the action
                var userId = int.Parse(User.GetUserId());
                await _sysActionRepository.LogActionAsync($"{userEmail} showed all their interactions", userId);

                return Ok(new { AllMyInteractions = allMyInteractions });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize]
        [HttpDelete("interactions/all")]
        public async Task<IActionResult> DeleteAllMyInteractions()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var interactionsToDelete = await _context.Notifications.Where(n => n.eventOwner == userEmail).ToListAsync();

                if (!interactionsToDelete.Any())
                {
                    return NotFound(new { message = "No interactions found for this user." });
                }

                _context.Notifications.RemoveRange(interactionsToDelete);
                await _context.SaveChangesAsync();

                // Log the action
                var userId = int.Parse(User.GetUserId());
                await _sysActionRepository.LogActionAsync($"{userEmail} deleted all their interactions", userId);

                return Ok(new { message = "All interactions deleted successfully.", DeletedInteractionsCount = interactionsToDelete.Count });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error.Message });
            }
        }
    }

    // DTO for deleting a notification
    public class DeleteNotificationDto
    {
        public int NotificationID { get; set; } // Assuming it's an integer ID, change to string if necessary.
    }
}
