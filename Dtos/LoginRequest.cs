using System.ComponentModel.DataAnnotations;

namespace Final.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string password { get; set; }
    }
}
