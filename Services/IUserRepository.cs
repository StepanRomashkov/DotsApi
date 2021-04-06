using DotsApi.Models;
using System.Threading.Tasks;

namespace DotsApi.Services
{
    public interface IUserRepository
    {
        Task<User> GetUserById(string id);
        Task<User> GetUserByEmail(string email);
        Task<User> CreateUser(User user, string password);
        Task<User> Authenticate(string email, string password);
        Task UpdateUser(User userParam, string password = null);
        Task DeleteUser(string id);
    }
}
