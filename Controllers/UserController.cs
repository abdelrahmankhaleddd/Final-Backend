using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Final.Models; // Replace with your actual model namespace
using Final.Services; // Replace with your email service namespace
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using System.Text.RegularExpressions;
using Final.Repositories;
using Final.Dtos;

namespace Final.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Requires authentication
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context; // Replace with your actual DbContext
        private readonly ILogger<UserController> _logger;
        private readonly IMailingService _emailService;
        private readonly ISystemActionRepository _sysActionRepository; // Assuming you have this
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(AppDbContext context, ILogger<UserController> logger, IMailingService emailService, ISystemActionRepository sysActionRepository, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _sysActionRepository = sysActionRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize] // Requires authentication
        // GET: api/User/MyProfile
        [HttpGet("MyProfile")]
        public async Task<IActionResult> ShowMyProfile()
        {
            try
            {
                var userId = int.Parse(User.GetUserId());
                var myProfile = await _context.Users.FindAsync(userId);

                if (myProfile == null)
                {
                    return NotFound(new { message = "User not found or has been deleted." });
                }

                await _sysActionRepository.LogActionAsync($"User {User.GetUserEmail()} showed his profile.", userId);

                return Ok(new { my_Profile = myProfile });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error showing my profile.");
                return StatusCode(500, new { message = error.Message });
            }
        }

        [Authorize] // Requires authentication
        // PUT: api/User/MyProfile
        [HttpPatch("MyProfile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto model)
        {
            try
            {
                var userId = int.Parse(User.GetUserId());
                var intendedUser = await _context.Users.FindAsync(userId);

                if (intendedUser == null)
                {
                    return NotFound(new { message = "User not found or has been deleted." });
                }

                if (model.email != null && intendedUser.role == UserRole.admin)
                {
                    return BadRequest(new { message = "Admins cannot change their email." });
                }

                intendedUser.gmailAcc = model.gmailAcc ?? intendedUser.gmailAcc;

                // 👇 اتأكد إن العنوان مش null قبل التعديل عليه
                if (intendedUser.addresses == null)
                {
                    intendedUser.addresses = new addresses();
                }

                intendedUser.addresses.Country = model.Country ?? intendedUser.addresses.Country;
                intendedUser.addresses.cityOrTown = model.cityOrTown ?? intendedUser.addresses.cityOrTown;
                intendedUser.addresses.details = model.details ?? intendedUser.addresses.details;

                intendedUser.email = model.email ?? intendedUser.email;
                intendedUser.userName = model.userName ?? intendedUser.userName;
                intendedUser.bio = model.bio ?? intendedUser.bio;
                intendedUser.age = model.age ?? intendedUser.age;

                await _context.SaveChangesAsync();

                await _emailService.SendEmailAsync(intendedUser.gmailAcc, "Profile Updated", $"Your profile was updated with: {Newtonsoft.Json.JsonConvert.SerializeObject(model)}");
                await _sysActionRepository.LogActionAsync($"User {User.GetUserEmail()} updated his profile with: {Newtonsoft.Json.JsonConvert.SerializeObject(model)}", userId);

                return Ok(new { message = "Profile updated", updatedFields = model });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error updating my profile.");
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize] // Requires authentication
        [HttpPatch("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto model)
        {
            try
            {
                if (model.newPassword != model.confirmNewPassword)
                {
                    return BadRequest(new { message = "New password and confirmation do not match." });
                }

                var userId = int.Parse(User.GetUserId());
                var logedUser = await _context.Users.FindAsync(userId);

                if (logedUser == null)
                {
                    return NotFound(new { message = "User not found or has been deleted." });
                }

                bool validPassword = BCrypt.Net.BCrypt.Verify(model.oldPassword, logedUser.password);
                if (!validPassword)
                {
                    return StatusCode(403, new { message = "Invalid old password" });
                }

                logedUser.password = BCrypt.Net.BCrypt.HashPassword(model.newPassword);
                await _context.SaveChangesAsync();

                await _emailService.SendEmailAsync(logedUser.gmailAcc, "Password Changed", $"Your password has been changed.");
                await _sysActionRepository.LogActionAsync($"User {User.GetUserEmail()} updated their password.", userId);

                return Ok(new { message = "Password Updated !!" });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error updating my password.");
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize] // Requires authentication
        [HttpDelete("DeleteTranscript")]
        public async Task<IActionResult> DeleteTranscript()
        {
            try
            {
                var userId = int.Parse(User.GetUserId());
                var logedUser = await _context.Users.FindAsync(userId);

                if (logedUser == null)
                {
                    return NotFound(new { message = "User not found or has been deleted." });
                }

                if (!string.IsNullOrEmpty(logedUser.transcript))
                {
                    string transcriptPath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads-transcript", Path.GetFileName(logedUser.transcript));

                    if (System.IO.File.Exists(transcriptPath))
                    {
                        System.IO.File.Delete(transcriptPath);
                    }
                    else
                    {
                        _logger.LogWarning($"Transcript file not found: {transcriptPath}");
                        return StatusCode(200, new { message = "No transcript to delete" });
                    }

                }
                else
                {
                    return StatusCode(200, new { message = "No transcript to delete" });
                }

                logedUser.transcript = null;
                await _context.SaveChangesAsync();

                await _sysActionRepository.LogActionAsync($"User {User.GetUserEmail()} deleted their transcript.", userId);

                return Ok(new { message = "Transcript deleted" });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error deleting my transcript.");
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize] // Requires authentication
        [HttpPost("UploadTranscript")]
        public async Task<IActionResult> UploadTranscript(IFormFile transcript)
        {
            try
            {
                var userId = int.Parse(User.GetUserId());

                if (transcript == null || transcript.Length == 0)
                {
                    return BadRequest("No file uploaded or invalid file extension.");
                }

                string fileExtension = Path.GetExtension(transcript.FileName).ToLowerInvariant();
                if (!new[] { ".pdf", ".png", ".jpg", ".jpeg" }.Contains(fileExtension))
                {
                    return BadRequest("Invalid file extension. Allowed extensions are .pdf, .png, .jpg, .jpeg");
                }

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads-transcript");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + transcript.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await transcript.CopyToAsync(fileStream);
                }

                var newTranscriptPath = $"{Request.Scheme}://{Request.Host}/Uploads-transcript/{uniqueFileName}";

                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found or has been deleted." });
                }

                if (!string.IsNullOrEmpty(user.transcript))
                {
                    string oldTranscriptPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads-transcript", Path.GetFileName(user.transcript));

                    if (System.IO.File.Exists(oldTranscriptPath))
                    {
                        System.IO.File.Delete(oldTranscriptPath);
                    }
                }

                user.transcript = newTranscriptPath;

                await _context.SaveChangesAsync();

                await _sysActionRepository.LogActionAsync($"User {User.GetUserEmail()} uploaded a new transcript.", userId);

                return StatusCode(201, new { message = "Transcript saved", newTranscriptPath });

            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error uploading transcript.");
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize]
        [HttpPatch("UploadAndUpdateImage")]
        public async Task<IActionResult> UploadAndUpdateImage(IFormFile image)
        {
            try
            {
                // 1. Basic file validation
                if (image == null || image.Length == 0)
                {
                    return BadRequest(new { message = "No file selected or the file is empty." });
                }

                // 2. Check file size (max 5MB)
                if (image.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Image size must not exceed 5MB." });
                }

                // 3. Check file extension
                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".webp", ".gif" };
                var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new
                    {
                        message = "File extension is not allowed.",
                        allowedExtensions
                    });
                }

                // 4. Get user data
                var userId = int.Parse(User.GetUserId());
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found or has been deleted." });
                }

                // 5. Create uploads folder if it doesn't exist
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profile-images");
                Directory.CreateDirectory(uploadsFolder);

                // 6. Generate a unique file name
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // 7. Save the new file
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // 8. Delete old image if exists
                if (!string.IsNullOrEmpty(user.image))
                {
                    try
                    {
                        var oldImageRelativePath = new Uri(user.image).AbsolutePath.TrimStart('/');
                        var oldImageFullPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImageRelativePath);

                        if (System.IO.File.Exists(oldImageFullPath))
                        {
                            System.IO.File.Delete(oldImageFullPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deleting old image");
                        // Continue even if deletion fails
                    }
                }

                // 9. Update image path in the database
                var newImageRelativePath = $"/uploads/profile-images/{uniqueFileName}";
                user.image = $"{Request.Scheme}://{Request.Host}{newImageRelativePath}";

                await _context.SaveChangesAsync();

                // 10. Return response with cache-busting timestamp
                return Ok(new
                {
                    success = true,
                    message = "Image uploaded successfully.",
                    imagePath = $"{user.image}?t={DateTime.Now.Ticks}",
                    user = user
                });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error uploading image");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while processing the image.",
                    detailed = error.Message
                });
            }
        }



        [HttpPost("BlockUser")]
        [Authorize(Roles = "admin")] // Example: Only admins can block users
        public async Task<IActionResult> BlockUser([FromBody] BlockUserModel request)
        {
            try
            {
                if (!int.TryParse(request.userId, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID format." });
                }

                if (userId == 0)
                {
                    return BadRequest(new { message = "User ID is required." });
                }

                var userData = await _context.Users.FindAsync(userId);
                if (userData == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                userData.isBlocked = true;
                await _context.SaveChangesAsync();

                string emailSubject = "Account Blocked";
                string emailText = $"Your account has been blocked by an administrator. Please contact support.";
                try
                {
                    await _emailService.SendEmailAsync(userData.gmailAcc, emailSubject, emailText);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error sending email");
                }
                await _sysActionRepository.LogActionAsync($"Admin {User.GetUserEmail()} blocked user {userData.email}.", int.Parse(User.GetUserId())); // adminID
                _logger.LogInformation($"Admin {User.GetUserEmail()} blocked user {userData.email}.");

                return StatusCode(202, new { message = "User blocked successfully", user_state = "blocked" });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error blocking user.");
                return StatusCode(500, new { message = error.Message });
            }
        }

        [HttpPost("UnblockUser")]
        [Authorize(Roles = "admin")] // Example: Only admins can block users
        public async Task<IActionResult> UnblockUser([FromBody] BlockUserModel request)
        {
            try
            {

                if (!int.TryParse(request.userId, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID format." });
                }

                if (userId == 0)
                {
                    return BadRequest(new { message = "User ID is required." });
                }

                var userData = await _context.Users.FindAsync(userId);
                if (userData == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                userData.isBlocked = false;
                await _context.SaveChangesAsync();

                await _emailService.SendEmailAsync(userData.gmailAcc, "Account Unblocked", $"Your account has been unblocked by an administrator.");
                await _sysActionRepository.LogActionAsync($"Admin {User.GetUserEmail()} unblocked user {userData.email}.", int.Parse(User.GetUserId()));
                _logger.LogInformation($"Admin {User.GetUserEmail()} unblocked user {userData.email}.");

                return StatusCode(202, new { message = "User unblocked successfully", user_state = "unblocked" });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error blocking user.");
                return StatusCode(500, new { message = error.Message });
            }
        }

        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
        {
            try
            {
                if (!int.TryParse(request.userId, out int userId))
                {
                    return BadRequest(new { message = "Invalid user ID format." });
                }

                if (userId == 0)
                {
                    return BadRequest(new { message = "User ID is required." });
                }

                var userData = await _context.Users.FindAsync(userId);
                if (userData == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                _context.Users.Remove(userData);
                await _context.SaveChangesAsync();

                string emailSubject = "Account deleted";
                string emailText = $"Your account has been deleted by an administrator.";

                try
                {
                    await _emailService.SendEmailAsync(userData.gmailAcc, emailSubject, emailText);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error sending email");
                }
                await _sysActionRepository.LogActionAsync($"Admin {User.GetUserEmail()} deleted user {userData.email}.", int.Parse(User.GetUserId()));
                _logger.LogInformation($"Admin {User.GetUserEmail()} deleted user {userData.email}.");

                return StatusCode(202, new { message = "User deleted." });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error deleting user.");
                return StatusCode(500, new { message = error.Message });
            }
        }

        [HttpGet("SpecificUser/{userId}")]
        public async Task<IActionResult> GetSpecificUser(string userId)
        {
            try
            {

                if (!int.TryParse(userId, out int userIdInt))
                {
                    return BadRequest(new { message = "Invalid user ID format." });
                }

                if (userId == null)
                {
                    return BadRequest(new { message = "User ID is required." });
                }
                var specficUser = await _context.Users.FindAsync(userIdInt);
                if (specficUser == null)
                {
                    return NotFound(new
                    {
                        message =
                            $"there is no user found with this id : {userId}"
                    });
                }

                await _sysActionRepository.LogActionAsync($"User {User.GetUserEmail()} viewed user {specficUser.email}.", int.Parse(User.GetUserId()));

                var userProjects = await _context.Projects
                    .Where(p => p.ownerId == userIdInt && p.status == projectStatus.accepted)
                    .ToListAsync();
                return Ok(new { user_data = specficUser, user_Projects = userProjects });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error getting specific user.");
                return StatusCode(500, new { message = error.Message });
            }
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                IQueryable<User> usersQuery = _context.Users;

                var allUsers = await usersQuery.ToListAsync();
                if (allUsers == null || allUsers.Count == 0)
                {
                    return NotFound(new { message = $"There are no users founded" });
                }

                int documentCount = await _context.Users.CountAsync();
                await _sysActionRepository.LogActionAsync($"Admin {User.GetUserEmail()} showed all users.", int.Parse(User.GetUserId()));

                return Ok(new
                {
                    message = $"Number of documents: {documentCount}",
                    ALL_users_data = allUsers
                });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error getting all users.");
                return StatusCode(500, new { message = error.Message });
            }
        }

        [HttpPost("CreateUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var Email = request.email;
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.email == Email);
                if (existingUser != null)
                {
                    return Conflict(new
                    {
                        error = "email is already exists..please enter another email"
                    });
                }
                var newuser = new User
                {
                    email = request.email,
                    userName = request.userName,
                    role = UserRole.student,
                    password = BCrypt.Net.BCrypt.HashPassword(request.password),
                    gmailAcc = request.Gmail_Acc
                };
                _context.Users.Add(newuser);
                await _context.SaveChangesAsync();

                await _sysActionRepository.LogActionAsync($"Admin {User.GetUserEmail()} created new user with data: {Newtonsoft.Json.JsonConvert.SerializeObject(request)}", int.Parse(User.GetUserId()));

                return StatusCode(201, new { message = "new user is created", data = newuser });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error creating user.");
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize] // Requires authentication
        [HttpPost("SearchUser")]
        public async Task<IActionResult> SearchUser([FromBody] SearchUserRequest request)
        {
            try
            {
                string search = request.search;
                if (string.IsNullOrEmpty(search))
                {
                    return StatusCode(404, new { message = "Please enter a user Name or Email to search" });
                }
                var searchOnUser = await _context.Users.Where(u => u.userName.Contains(search) || u.email.Contains(search)).ToListAsync();

                if (searchOnUser.Count == 0)
                {
                    return StatusCode(404, new
                    {
                        message =
                            "There is no user found with this name or email"
                    });
                }
                return Ok(searchOnUser);

            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error searching for users.");
                return StatusCode(500, new { message = error.Message });
            }
        }

        

    }

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this System.Security.Claims.ClaimsPrincipal user)
        {
            return user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetUserEmail(this System.Security.Claims.ClaimsPrincipal user)
        {
            return user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        }
    }
}
