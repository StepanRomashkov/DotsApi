using DotsApi.Helpers;
using DotsApi.Models;
using DotsApi.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DotsTests
{
    internal class UsersRepositoryTests
    {
        private IUsersRepository _usersRepository;
        private IMongoCollection<User> _usersCollection;

        [SetUp]
        public void Setup()
        {
            _usersRepository = new UsersRepository(Constants.databaseContext);
            _usersCollection = Constants.databaseContext.Users;

            User testSubject = new User();
            testSubject.Id = "60848ae8fb71edf2a7ebf846";
            testSubject.Email = "test@subject.zero";
            testSubject.PasswordHash = "$2a$11$wvypXckqs4LxCCalZMBV9.CIkcUmayQAvvyISAPmlY4/Prs49Wkce";

            if (_usersCollection.Find(u => u.Email == testSubject.Email).FirstOrDefault() == null)
                _usersCollection.InsertOne(testSubject);
        }

        [Test]
        public async Task TestGetUserById()
        {
            User user = await _usersRepository.GetUserByIdAsync("60848ae8fb71edf2a7ebf846");

            Assert.IsNotNull(user);
            Assert.AreEqual(user.Email, "test@subject.zero");
        }

        [Test]
        public async Task TestGetUserByEmail()
        {
            User user = await _usersRepository.GetUserByEmailAsync("test@subject.zero");

            Assert.IsNotNull(user);
            Assert.AreEqual(user.Id, "60848ae8fb71edf2a7ebf846");
        }

        [Test]
        public async Task TestCreateUser()
        {
            User user = new User();
            user.Email = "test@subject.zero";
            string password = "sU6jEctY";

            try
            {
                Assert.ThrowsAsync<AppException>( () => _usersRepository
                    .CreateUserAsync(new User() { Email = "test@subject.zero" }, "sU6jEctY"), 
                    "The email test@subject.zero is already exist");

                bool isExist = await _usersCollection.Find(u => u.Email == user.Email).FirstOrDefaultAsync() != null;
                
                if (isExist)
                    await _usersCollection.DeleteOneAsync(u => u.Email == user.Email);

                User result = await _usersRepository.CreateUserAsync(user, password);

                Assert.AreEqual(user.Email, result.Email);
                Assert.IsNotNull(result.PasswordHash);
                Assert.IsEmpty(result.Notices);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                await _usersCollection.DeleteOneAsync(u => u.Email == user.Email);
            }
        }

        [Test]
        public async Task TestAuthenticate()
        {
            string email = "test@subject.zero";
            string password = "sU6jEctY";

            try
            {
                Assert.Null(await _usersRepository.AuthenticateAsync(null, password));
                Assert.Null(await _usersRepository.AuthenticateAsync(email, null));
                Assert.Null(await _usersRepository.AuthenticateAsync(null, null));
                Assert.Null(await _usersRepository.AuthenticateAsync("wrongEmail", password));
                Assert.Null(await _usersRepository.AuthenticateAsync(email, "wrongPassword"));

                User result = await _usersRepository.AuthenticateAsync(email, password);

                Assert.IsNotNull(result);
                Assert.AreEqual(result.Id, "60848ae8fb71edf2a7ebf846");
                Assert.AreEqual(result.Email, email);
                Assert.True(BCrypt.Net.BCrypt.Verify(password, result.PasswordHash));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Test]
        public async Task TestUpdateUser()
        {
            try
            {
                User replacement = await _usersRepository.GetUserByEmailAsync("test@subject.zero");
                Assert.IsNotNull(replacement);

                replacement.Id = ObjectId.GenerateNewId().ToString();
                Assert.ThrowsAsync<AppException>(() => _usersRepository
                    .UpdateUserAsync(replacement),
                    "User not found");
                replacement.Id = "60848ae8fb71edf2a7ebf846";

                User existingUser = new User()
                {
                    Email = "existingEmail"
                };
                await _usersRepository.CreateUserAsync(existingUser, "anyPassword");
                
                replacement.Email = "existingEmail";
                Assert.ThrowsAsync<AppException>(() => _usersRepository
                    .UpdateUserAsync(replacement),
                    $"The email {replacement.Email} is already taken");

                replacement.Email = "newEmail";

                await _usersRepository.UpdateUserAsync(replacement);
                User updatedUser = await _usersCollection.Find(u => u.Email == "newEmail").FirstOrDefaultAsync();
                Assert.NotNull(updatedUser);
                Assert.AreEqual(updatedUser.Id, replacement.Id);

                await _usersRepository.UpdateUserAsync(updatedUser, "newPassword");
                updatedUser = await _usersCollection.Find(u => u.Id == "60848ae8fb71edf2a7ebf846").FirstOrDefaultAsync();
                Assert.True(BCrypt.Net.BCrypt.Verify("newPassword", updatedUser.PasswordHash));

                replacement.Email = "test@subject.zero";

                await _usersRepository.UpdateUserAsync(replacement, "oldPassword");
                updatedUser = await _usersCollection.Find(u => u.Id == "60848ae8fb71edf2a7ebf846").FirstOrDefaultAsync();
                Assert.AreEqual(updatedUser.Email, "test@subject.zero");
                Assert.True(BCrypt.Net.BCrypt.Verify("oldPassword", updatedUser.PasswordHash));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                await _usersCollection.DeleteOneAsync(u => u.Id == "60848ae8fb71edf2a7ebf846");
                await _usersCollection.DeleteOneAsync(u => u.Email == "existingEmail");
            }
        }

        [Test]
        public async Task TestDeleteUser()
        {
            User user = await _usersCollection.Find(u => u.Id == "60848ae8fb71edf2a7ebf846").FirstOrDefaultAsync();

            Assert.NotNull(user);
            await _usersRepository.DeleteUserAsync("60848ae8fb71edf2a7ebf846");
            Assert.Null(await _usersCollection.Find(u => u.Id == "60848ae8fb71edf2a7ebf846").FirstOrDefaultAsync());
        }
    }
}
