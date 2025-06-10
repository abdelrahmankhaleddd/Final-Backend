using Final.Models;

public interface ITokenRepository
{
    Task<Token> GetByUserIdAsync(int userId);  // ✅ Add this method
    Task<Token> GetByValueAsync(string tokenValue);
    Task CreateAsync(Token token);
    Task DeleteAsync(Token token);
}
