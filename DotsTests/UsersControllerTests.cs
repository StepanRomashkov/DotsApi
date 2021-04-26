using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using DotsApi.Helpers;
using DotsApi.Models;
using DotsApi.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using System.Threading.Tasks;
using DotsApi.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using DotsApi.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DotsTests
{
    internal class UsersControllerTests
    {
        private UsersController _usersController;
        private IUsersRepository _usersRepository;
        private IMongoCollection<User> _usersCollection;
        private IMapper _mapper;
        private IDotsSecurityTokenHandler _dotsSecurityTokenHandler;
        private IAuthorizationService AuthorizationService { get; }
        private string tokenString = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2MDg0OGFlOGZiNzFlZGYyYTdlYmY4NDYiLCJuYmYiOjE2MTk0NDcyNjUsImV4cCI6MzQ4MTM2NzI2MiwiaWF0IjoxNjE5NDQ3MjY1fQ.7tgxfqL1EBnFdd14dfL9NDoWApS1ZEk3r9DymcA1-lM";


        [SetUp]
        public void Setup()
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Constants.GetSecret()));
            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            DateTime expires = DateTime.UtcNow.AddHours(1);
            
            MapperConfiguration mapperConfig = new MapperConfiguration(c => c.AddProfile(new AutoMapperProfile()));
            _mapper = new Mapper(mapperConfig);

            _dotsSecurityTokenHandler = new DotsSecurityTokenHandler(
                key, signingCredentials, tokenHandler, expires);

            _usersRepository = new UsersRepository(Constants.databaseContext);
            _usersCollection = Constants.databaseContext.Users;

            _usersController = new UsersController(
                _usersRepository,
                _mapper,
                _dotsSecurityTokenHandler,
                AuthorizationService);

            User testSubject = new User();
            testSubject.Id = "60848ae8fb71edf2a7ebf846";
            testSubject.Email = "test@subject.zero";
            testSubject.PasswordHash = "$2a$11$wvypXckqs4LxCCalZMBV9.CIkcUmayQAvvyISAPmlY4/Prs49Wkce";
            testSubject.Notices = new Notice[] { };

            if (_usersCollection.Find(u => u.Email == testSubject.Email).FirstOrDefault() == null)
                _usersCollection.InsertOne(testSubject);

        }

        [Test]
        public async Task TestRegister()
        {
            try
            {
                RegisterDto dto = new RegisterDto()
                {
                    Email = "test",
                    Password = "test"
                };

                await _usersController.RegisterAsync(dto);
                User user = await _usersRepository.GetUserByEmailAsync(dto.Email);
                Assert.IsNotNull(user);
                Assert.AreEqual(dto.Email, user.Email);

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await _usersCollection.DeleteOneAsync(u => u.Email == "test");
            }
        }

        [Test]
        public async Task TestAuthenticate()
        {
            try
            {
                AuthenticateDto dto = new AuthenticateDto()
                {
                    Email = "test@subject.zero",
                    Password = "sU6jEctY"
                };

                OkObjectResult result = await _usersController.AuthenticateAsync(dto) as OkObjectResult;
                BsonDocument elements = result.Value.ToBsonDocument();
                Assert.AreEqual(elements.GetValue("_id").ToString(), "60848ae8fb71edf2a7ebf846");
                Assert.AreEqual(elements.GetValue("Email").ToString(), dto.Email);
                Assert.NotNull(elements.GetValue("Token"));

                dto.Email = "nonExistentEmail";
                var response = await _usersController.AuthenticateAsync(dto);
                Assert.IsInstanceOf<BadRequestObjectResult>(response, "Email or password is incorrect");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Test]
        [Authorize]
        public async Task TestUpdateUser()
        {
            try
            {
                _usersController.ControllerContext =
                    new ControllerContext { HttpContext = new DefaultHttpContext() };

                _usersController.ControllerContext.HttpContext.Request.Headers["Authorization"] =
                    $"Bearer: {tokenString}";

                Func<string, User> getUser = (id) => _usersCollection.Find(u => u.Id == id).FirstOrDefault();
                
                UpdateDto updateDto = new UpdateDto() { Email = "newEmail" };
                
                await _usersController.UpdateUserAsync("60848ae8fb71edf2a7ebf846", updateDto);
                User user = getUser("60848ae8fb71edf2a7ebf846");
                Assert.AreEqual("newEmail", user.Email);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                await _usersCollection.DeleteOneAsync(u => u.Id == "60848ae8fb71edf2a7ebf846");
            }
        }

    }
}
