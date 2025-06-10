using System.ComponentModel.DataAnnotations;

namespace Final.Dtos
{
    public class UserSignUpDto
    {

        public string userName { get; set; } // Optional, if required for signup
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string password { get; set; }

        public string GmailAcc { get; set; } // Optional, if required for signup

    }

}
