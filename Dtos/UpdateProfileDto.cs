namespace Final.Dtos
{
    public class UpdateProfileDto
    {
        public string Country { get; set; }
        public string cityOrTown { get; set; }
        public string details { get; set; }
        public string email { get; set; }
        public string gmailAcc { get; set; } // ✅ ضيفي دي
        public string userName { get; set; }
        public string? bio { get; set; }
        public int? age { get; set; }
    }
}
