using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

public class FileUploadService
{
    private readonly IConfiguration _configuration;

    public FileUploadService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || !IsValidImage(file))
            throw new Exception("Invalid image file.");

        var filePath = Path.Combine("Uploads-Images", GenerateFileName(file.FileName));
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return filePath;
    }

    public async Task<string> UploadTranscriptAsync(IFormFile file)
    {
        if (file == null || !IsValidDocument(file))
            throw new Exception("Invalid transcript file.");

        var filePath = Path.Combine("Uploads-Transcript", GenerateFileName(file.FileName));
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return filePath;
    }

    private static bool IsValidImage(IFormFile file) =>
        new[] { "image/png", "image/jpeg", "image/jpg" }.Contains(file.ContentType);

    private static bool IsValidDocument(IFormFile file) =>
        new[] { "application/pdf", "image/png", "image/jpeg" }.Contains(file.ContentType);

    private static string GenerateFileName(string originalFileName) =>
        $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
}
