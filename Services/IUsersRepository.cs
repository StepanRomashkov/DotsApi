using DotsApi.Models;
using System.Threading.Tasks;

namespace DotsApi.Services
{
    public interface IUsersRepository
    {
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user, string password);
        Task<User> AuthenticateAsync(string email, string password);
        Task UpdateUserAsync(User userParam, string password = null);
        Task DeleteUserAsync(string id);
    }
}
