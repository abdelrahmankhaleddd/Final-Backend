using System.Threading.Tasks;
using Final.Models;

namespace Final.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByResetCodeAsync(string hashedResetCode);
        Task UpdateAsync(User user);
    }
}