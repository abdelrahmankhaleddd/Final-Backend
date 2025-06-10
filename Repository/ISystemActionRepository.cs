using System.Threading.Tasks;

namespace Final.Repositories
{
    public interface ISystemActionRepository
    {
        Task LogActionAsync(string actionDescription, int userId);
    }
}
