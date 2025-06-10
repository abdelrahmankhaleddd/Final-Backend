//using System;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;
//using Final.Models;
//using Final.Repositories;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using BCrypt.Net;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using Microsoft.IdentityModel.Tokens;
//using System.Collections.Generic;
//using Final.DTOs;
//using Final.Services;

//namespace Final.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly IUserRepository _userRepository;
//        private readonly IConfiguration _configuration;
//        private readonly IMailingService _emailService;
//        private readonly ISystemActionRepository _sysActionRepository;

//        public AuthController(IUserRepository userRepository, IConfiguration configuration, IMailingService emailService, ISystemActionRepository sysActionRepository)
//        {
//            _userRepository = userRepository;
//            _configuration = configuration;
//            _emailService = emailService;
//            _sysActionRepository = sysActionRepository;
//        }

//        // POST: api/Auth/signup
//        [HttpPost("signup")]
//        public async Task<IActionResult> SignUp([FromBody] User user, [FromServices] ITokenRepository tokenRepository)
//        {
//            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
//                return BadRequest("Invalid user data.");

//            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
//            if (existingUser != null)
//                return Conflict("Email already exists. Please enter another email.");

//            var newUser = await _userRepository.CreateAsync(user);

//            // Generate JWT token for the new user
//            string token = GenerateJwtToken(newUser.Id.ToString(), newUser.Role.ToString());

//            // Check if a token already exists (prevents duplicate tokens)
//            var existingToken = await tokenRepository.GetByUserIdAsync(newUser.Id);
//            if (existingToken != null)
//            {
//                await tokenRepository.DeleteAsync(existingToken);
//            }

//            // Store the new token
//            var tokenEntity = new Token
//            {
//                Value = token,
//                Expires = DateTime.UtcNow.AddHours(7),

//            };

//            await tokenRepository.CreateAsync(tokenEntity);

//            return StatusCode(201, new { message = "Sign-up successful.", token });
//        }

//        // POST: api/Auth/login
//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginRequest loginDto, [FromServices] ITokenRepository tokenRepository)
//        {
//            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
//            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
//                return Unauthorized(new { message = "Invalid email or password" });

//            if (user.IsBlocked)
//                return Forbid("You are blocked. Contact the admin for support.");

//            // Generate JWT token
//            string token = GenerateJwtToken(user.Id.ToString(), user.Role.ToString());

//            // Remove old token if it exists
//            var existingToken = await tokenRepository.GetByUserIdAsync(user.Id);
//            if (existingToken != null)
//            {
//                await tokenRepository.DeleteAsync(existingToken);
//            }

//            // Store the new token
//            var tokenEntity = new Token
//            {
//                Value = token,
//                Expires = DateTime.UtcNow.AddHours(7),

//            };

//            await tokenRepository.CreateAsync(tokenEntity);

//            return Ok(new { message = "Login successful", token });
//        }

//        // POST: api/Auth/logout
//        [HttpPost("logout")]
//        public async Task<IActionResult> Logout([FromBody] string token, [FromServices] ITokenRepository tokenRepository)
//        {
//            var tokenEntity = await tokenRepository.GetByValueAsync(token);
//            if (tokenEntity != null)
//            {
//                await tokenRepository.DeleteAsync(tokenEntity); // Correct method to delete token
//            }

//            return Ok(new { message = "Logged out successfully." });
//        }

//        [HttpPost("forget-password")]
//        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest model)
//        {
//            try
//            {
//                var user = await _userRepository.GetByEmailAsync(model.Email);
//                if (user == null)
//                {
//                    return StatusCode(403, new { message = "Email not found." });
//                }

//                var resetCode = new Random().Next(100000, 999999).ToString();
//                using var sha256 = SHA256.Create();
//                var hashedResetCode = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(resetCode)));

//                user.PasswordResetCode = hashedResetCode;
//                user.PasswordResetExpires = DateTime.UtcNow.AddMinutes(10);
//                user.PasswordResetVerified = false;
//                await _userRepository.UpdateAsync(user);

//                var message = $"Hi {user.UserName},\n\nYour reset code: {resetCode}\n\nValid for 10 min.";
//                await _emailService.SendEmailAsync(user.Email, "Password Reset Code", message);

//                await _sysActionRepository.LogActionAsync($"{user.Email} requested a password reset.");

//                return Ok(new { status = "Success", message = "Mail sent." });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = ex.Message });
//            }
//        }

//        [HttpPost("verify-reset-code")]
//        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequest model)
//        {
//            try
//            {
//                using var sha256 = SHA256.Create();
//                var hashedResetCode = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.ResetCode)));

//                var user = await _userRepository.GetByResetCodeAsync(hashedResetCode);
//                if (user == null || user.PasswordResetExpires < DateTime.UtcNow)
//                {
//                    return StatusCode(404, new { message = "Reset code invalid or expired." });
//                }

//                user.PasswordResetVerified = true;
//                await _userRepository.UpdateAsync(user);

//                await _sysActionRepository.LogActionAsync($"{user.Email} verified reset code: {model.ResetCode}");

//                return Ok(new { status = "Success" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = ex.Message });
//            }
//        }

//        // POST: api/Auth/reset-password
//        [HttpPost("reset-password")]
//        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetDto)
//        {
//            var user = await _userRepository.GetByEmailAsync(resetDto.Email);
//            if (user == null)
//                return NotFound("User not found.");

//            if (!(user.PasswordResetVerified ?? false))
//                return BadRequest("Reset code not verified.");

//            user.Password = BCrypt.Net.BCrypt.HashPassword(resetDto.NewPassword);
//            user.PasswordResetCode = user.PasswordResetCode ?? string.Empty;
//            user.PasswordResetExpires = null;
//            user.PasswordResetVerified = false;

//            await _userRepository.UpdateAsync(user);

//            return Ok(new { message = "Password changed successfully." });
//        }

//        // Helper function to generate JWT token
//        private string GenerateJwtToken(string userId, string role)
//        {
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, userId),
//                new Claim(ClaimTypes.Role, role)
//            };

//            var token = new JwtSecurityToken(
//                _configuration["Jwt:Issuer"],
//                _configuration["Jwt:Audience"],
//                claims,
//                expires: DateTime.UtcNow.AddDays(7),
//                signingCredentials: creds
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }


//// Helper function to hash strings
//        private string HashString(string input)
//        {
//            using var sha256 = SHA256.Create();
//            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
//            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
//        }
//    }

//}


using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Final.Models;
using Final.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using Final.DTOs;
using Final.Services;
using Final.Dtos;

namespace Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMailingService _emailService;
        private readonly ISystemActionRepository _sysActionRepository;
        private readonly AppDbContext _context; // Replace with your actual DbContext class



        public AuthController(IUserRepository userRepository, IConfiguration configuration, IMailingService emailService, ISystemActionRepository sysActionRepository, AppDbContext context)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _sysActionRepository = sysActionRepository;
            _context = context;


        }

        // POST: api/Auth/signup
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserSignUpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email or username already exists
            if (await _context.Set<User>().AnyAsync(u => u.email == dto.email || u.userName == dto.userName))
                return BadRequest(new { message = "Username or Email already exists." });

            // Hash the password using BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.password);

            // Create new user
            var user = new User
            {
                userName = dto.userName,
                email = dto.email,
                password = hashedPassword,
                gmailAcc = dto.GmailAcc,
                role = UserRole.student, // Default role

            };

            try
            {
                _context.Set<User>().Add(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "User registered successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the user.", error = ex.Message });
            }
        }


        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.password, user.password))
                return Unauthorized(new { message = "Invalid email or password" });

            if (user.isBlocked)
                return Forbid("You are blocked. Contact the admin for support.");

            string token = GenerateJwtToken(user.Id.ToString(), user.role.ToString(), user.email.ToString());

            // Log action
            await _sysActionRepository.LogActionAsync($"User {user.email} logged in.", user.Id);

            return Ok(new { message = "Login successful", token });
        }

        // POST: api/Auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("access_token");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                await _sysActionRepository.LogActionAsync("User logged out.", userId);
            }

            return Ok("You have been logged out.");
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest model)
        {
            var user = await _userRepository.GetByEmailAsync(model.email);
            if (user == null)
                return StatusCode(403, new { message = "Email not found." });

            var resetCode = new Random().Next(100000, 999999).ToString();
            using var sha256 = SHA256.Create();
            var hashedResetCode = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(resetCode)));

            user.passwordResetCode = hashedResetCode;
            user.passwordResetExpires = DateTime.UtcNow.AddMinutes(10);
            user.passwordResetVerified = false;
            await _userRepository.UpdateAsync(user);

            var message = $"Hi {user.userName},\n\nYour reset code: {resetCode}\n\nValid for 10 min.";
            await _emailService.SendEmailAsync(user.email, "Password Reset Code", message);

            // Log action
            await _sysActionRepository.LogActionAsync($"User {user.email} requested a password reset.", user.Id);

            return Ok(new { status = "Success", message = "Mail sent." });
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequest model)
        {
            using var sha256 = SHA256.Create();
            var hashedResetCode = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.resetCode)));

            var user = await _userRepository.GetByResetCodeAsync(hashedResetCode);
            if (user == null || user.passwordResetExpires < DateTime.UtcNow)
                return StatusCode(404, new { message = "Reset code invalid or expired." });

            user.passwordResetVerified = true;
            await _userRepository.UpdateAsync(user);

            // Log action
            await _sysActionRepository.LogActionAsync($"User {user.email} verified reset code.", user.Id);

            return Ok(new { status = "Success" });
        }

        // POST: api/Auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetDto)
        {
            var user = await _userRepository.GetByEmailAsync(resetDto.email);
            if (user == null)
                return NotFound("User not found.");

            if (!(user.passwordResetVerified ?? false))
                return BadRequest("Reset code not verified.");

            if (resetDto.newPassword != resetDto.confirmPassword)
                return BadRequest("New password and confirmation password do not match.");

            user.password = BCrypt.Net.BCrypt.HashPassword(resetDto.newPassword);
            user.passwordResetCode = string.Empty;
            user.passwordResetExpires = null;
            user.passwordResetVerified = false;

            await _userRepository.UpdateAsync(user);

            // Log action
            await _sysActionRepository.LogActionAsync($"User {user.email} reset their password.", user.Id);

            return Ok(new { message = "Password changed successfully." });
        }


        // Helper function to generate JWT token
        private string GenerateJwtToken(string userId, string role, string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId), 
        new Claim("UserId", userId),  
        new Claim(ClaimTypes.Role, role), 
        new Claim(ClaimTypes.Email, email), 

    };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // Helper function to hash strings
        private string HashString(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

}