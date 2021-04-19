using DotsApi.Models;
using System.Threading.Tasks;

namespace DotsApi.Services
{
    public interface IUserRepository
    {
        Task<User> GetUserById(string id);
        Task<User> GetUserByEmail(string email);
        Task<User> CreateUserAsync(User user, string password);
        Task<User> AuthenticateAsync(string email, string password);
        Task UpdateUserAsync(User userParam, string password = null);
        Task DeleteUserAsync(string id);
    }
}
