namespace Final.Services
{
    public interface IMailingService
    {
        Task SendEmailAsync(string mailto, string subject, string body, IList<IFormFile> attachments = null);
    }
}
