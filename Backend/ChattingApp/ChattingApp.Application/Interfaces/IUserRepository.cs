using ChattingApp.Domain.Entities;
using System.Threading.Tasks;

namespace ChattingApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<int> CreateUserAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
