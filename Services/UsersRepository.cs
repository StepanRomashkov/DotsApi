using DotsApi.Helpers;
using DotsApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DotsApi.Services
{
    public class UsersRepository: IUsersRepository
    {
        private readonly DotsDatabaseContext _context = null;

        public UsersRepository(IOptions<DotsDatabaseSettings> settings)
        {
            _context = new DotsDatabaseContext(settings);
        }

        public UsersRepository(DotsDatabaseContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            try
            {
                return await _context.Users.Find(user => user.Id == id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.Find(user => user.Email == email)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            if (GetUserByEmailAsync(user.Email).Result != null)
                throw new AppException($"The email {user.Email} is already exist");

            try
            {
                user.Id = ObjectId.GenerateNewId().ToString();
                user.PasswordHash = CreatePasswordHash(password);
                user.Notices = new Notice[] { };

                await _context.Users.InsertOneAsync(user);

                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;
            
            try
            {
                User user = await GetUserByEmailAsync(email);
                
                if (user == null)
                    return null;
                
                if (!VerifyPassword(password, user.PasswordHash))
                    return null;
                
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateUserAsync(User user, string password = null)
        {
            try
            {
                User replacement = await GetUserByIdAsync(user.Id);                
                
                if (replacement == null)
                    throw new AppException("User not found");
                
                if (!string.IsNullOrWhiteSpace(user.Email) && user.Email != replacement.Email)
                {
                    if (GetUserByEmailAsync(user.Email).Result != null)
                        throw new AppException($"The email {user.Email} is already taken");
                    
                    replacement.Email = user.Email;
                }

                if (!string.IsNullOrWhiteSpace(password))
                    replacement.PasswordHash = CreatePasswordHash(password);

                await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, replacement);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteUserAsync(string id)
        {
            try
            {
                await _context.Users.DeleteOneAsync(user => user.Id == id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string CreatePasswordHash(string password)
        {
            string passwordSalt = BCrypt.Net.BCrypt.GenerateSalt(11);
            
            return BCrypt.Net.BCrypt.HashPassword(password, passwordSalt);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
