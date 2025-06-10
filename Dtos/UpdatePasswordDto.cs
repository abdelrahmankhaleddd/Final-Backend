using System.ComponentModel.DataAnnotations;

namespace Final.Dtos
{
    public class UpdatePasswordDto
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmNewPassword { get; set; }
    }
}
