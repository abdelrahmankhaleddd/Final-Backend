namespace Final.DTOs
{
    public class ResetPasswordRequest
    {
        public string email { get; set; }
        public string newPassword { get; set; }
        public string confirmPassword {  get; set; }
    }
}
