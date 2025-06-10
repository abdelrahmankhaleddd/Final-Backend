
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Final.Models;
using Final.Services;
using Final.Repositories;

namespace Final.Controllers
{
    [Route("api/chats")]
    [ApiController]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMailingService _mailingService;
        private readonly ISystemActionRepository _sysActionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ChatsController> _logger;

        public ChatsController(
            AppDbContext context,
            IMailingService mailingService,
            ISystemActionRepository sysActionRepository,
            IUserRepository userRepository,
            ILogger<ChatsController> logger
        )
        {
            _context = context;
            _mailingService = mailingService;
            _sysActionRepository = sysActionRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                _logger.LogError("User ID is missing from claims.");
                return null;
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                _logger.LogError($"Invalid user ID format: {userIdString}");
                return null;
            }

            return userId;
        }

        [HttpGet("all")]
        public async Task<IActionResult> ShowAllMyChats()
        {
            var loggedUserId = GetCurrentUserId();
            if (!loggedUserId.HasValue)
            {
                return Unauthorized(new { message = "Invalid User" });
            }

            var allChats = await _context.Chats
                .Where(c => c.senderId == loggedUserId || c.receiverId == loggedUserId)
                .Include(c => c.AllMessages)
                .ToListAsync();

            if (allChats == null || !allChats.Any())
            {
                return NotFound(new { message = "No chats found." });
            }

            return Ok(new { ChatsNumbers = allChats.Count, MyChats = allChats });
        }

        [HttpGet("{chatID}")]
        public async Task<IActionResult> ShowSpecificChat(int chatID)
        {
            var specificChat = await _context.Chats
                .Include(c => c.AllMessages)
                .FirstOrDefaultAsync(c => c.chatNums == chatID);

            if (specificChat == null)
            {
                return NotFound(new { message = "Chat not found." });
            }

            return Ok(new { specificChat });
        }

        [HttpDelete("message/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var loggedUserId = GetCurrentUserId();
            if (!loggedUserId.HasValue)
            {
                return Unauthorized(new { message = "Invalid User" });
            }

            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return NotFound(new { message = "Message not found." });
            }

            if (message.messageOwnerId != loggedUserId.Value)
            {
                return Forbid();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Message deleted successfully." });
        }


        [HttpPost("start")]
        public async Task<IActionResult> StartChat([FromBody] StartChatDto startChatDto)
        {
            var loggedUserId = GetCurrentUserId();
            if (!loggedUserId.HasValue)
            {
                return Unauthorized(new { message = "Invalid User" });
            }

            if (!int.TryParse(startChatDto.ReceiverID, out int receiverId))
            {
                return BadRequest(new { message = "Invalid Receiver ID." });
            }

            var receiver = await _userRepository.GetByIdAsync(receiverId);
            if (receiver == null)
            {
                return NotFound(new { message = "Receiver not found." });
            }

            var existingChat = await _context.Chats.FirstOrDefaultAsync(c =>
                (c.senderId == loggedUserId && c.receiverId == receiverId) ||
                (c.receiverId == loggedUserId && c.senderId == receiverId));

            if (existingChat != null)
            {
                return Ok(new { message = "A chat already exists.", ExistChat = existingChat });
            }

            var newChat = new Chat
            {
                senderId = loggedUserId.Value,
                receiverId = receiverId,
                chatName = $"{receiver.email}|_|{User.FindFirst(ClaimTypes.Email)?.Value}"
            };

            _context.Chats.Add(newChat);
            await _context.SaveChangesAsync();

            await _sysActionRepository.LogActionAsync($"User {User.FindFirst(ClaimTypes.Email)?.Value} started a new chat with {receiver.email}", loggedUserId.Value);

            return CreatedAtAction(nameof(StartChat), new { ChatID = newChat.chatNums }, new { message = "New chat created.", NewChat = newChat });
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessagePrivate([FromBody] SendMessageDto sendMessageDto)
        {
            var loggedUserId = GetCurrentUserId();
            if (!loggedUserId.HasValue)
            {
                return Unauthorized(new { message = "Invalid User" });
            }

            if (!int.TryParse(sendMessageDto.ReceiverID, out int receiverId))
            {
                return BadRequest(new { message = "Invalid Receiver ID." });
            }

            if (string.IsNullOrEmpty(sendMessageDto.IntendedMessage))
            {
                return BadRequest(new { message = "Message cannot be empty." });
            }

            var receiver = await _userRepository.GetByIdAsync(receiverId);
            if (receiver == null)
            {
                return NotFound(new { message = "Receiver not found." });
            }

            var intendedChat = await _context.Chats.Include(c => c.AllMessages).FirstOrDefaultAsync(c =>
                (c.senderId == loggedUserId && c.receiverId == receiverId) ||
                (c.receiverId == loggedUserId && c.senderId == receiverId));

            if (intendedChat == null)
            {
                return NotFound(new { message = "No chat found between you and this user." });
            }

            var newMessage = new Message
            {
                theMessage = sendMessageDto.IntendedMessage,
                messageOwnerId = loggedUserId.Value
            };

            intendedChat.AllMessages.Add(newMessage);
            await _context.SaveChangesAsync();

            return Accepted(new { message = "Message sent successfully.", IntendedChat = intendedChat });
        }
    }

    public class StartChatDto
    {
        [Required]
        public string ReceiverID { get; set; }
    }

    public class SendMessageDto
    {
        [Required]
        public string ReceiverID { get; set; }

        [Required]
        public string IntendedMessage { get; set; }
    }
}