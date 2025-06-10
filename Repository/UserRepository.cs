using System;
using System.Linq;
using System.Threading.Tasks;
using Final.Models;
using Microsoft.EntityFrameworkCore;

namespace Final.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password, 11);
            Console.WriteLine($"New Hashed Password: {user.password}"); // Debugging line
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }


        public async Task<User> GetByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> GetByResetCodeAsync(string hashedResetCode)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.passwordResetCode == hashedResetCode);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }


    }
}
