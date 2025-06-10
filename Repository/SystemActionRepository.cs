using Final.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Final.Repositories
{
    public class SystemActionRepository : ISystemActionRepository
    {
        private readonly AppDbContext _context;

        public SystemActionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(string actionDescription, int userId)
        {
            var sysAction = new SysAction
            {
                action = actionDescription,
                userId = userId,
                date = DateTime.UtcNow,
                creationTime = DateTime.UtcNow,
                lastModified = DateTime.UtcNow
            };

            await _context.SysActions.AddAsync(sysAction);
            await _context.SaveChangesAsync();
        }
    }
}
