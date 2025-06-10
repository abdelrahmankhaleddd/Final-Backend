using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Final.Models;

public class TokenRepository : ITokenRepository
{
    private readonly AppDbContext _context;

    public TokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Token> GetByUserIdAsync(int userId)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.tokens)  // Get tokens from the user's collection
            .FirstOrDefaultAsync();
    }

    public async Task<Token> GetByValueAsync(string tokenValue)
    {
        return await _context.Tokens.FirstOrDefaultAsync(t => t.value == tokenValue);
    }

    public async Task CreateAsync(Token token)
    {
        await _context.Tokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Token token)
    {
        _context.Tokens.Remove(token);
        await _context.SaveChangesAsync();
    }
}
