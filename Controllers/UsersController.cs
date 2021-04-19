using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using DotsApi.Models;
using DotsApi.Services;
using DotsApi.Helpers;
using DotsApi.Authorization;

namespace DotsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepository;
        private IMapper _mapper;
        private IDotsSecurityTokenHandler _dotsSecurityTokenHandler;
        private IAuthorizationService AuthorizationService { get; }

        public UsersController(
            IUserRepository userRepository, 
            IMapper mapper,
            IDotsSecurityTokenHandler dotsSecurityTokenHandler,
            IAuthorizationService authorizationService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _dotsSecurityTokenHandler = dotsSecurityTokenHandler;
            AuthorizationService = authorizationService;
            
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticateDto model)
        {
            var user = await _userRepository.AuthenticateAsync(model.Email, model.Password);

            if (user == null)
                return BadRequest(new { message = "Email or password is incorrect" });
            try
            {
                string token = _dotsSecurityTokenHandler.CreateToken(user);
            
                return Ok(new
                {
                    Id = user.Id,
                    Email = user.Email,
                    Token = token
                });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto model)
        {
            var user = _mapper.Map<User>(model);
            try
            {
                await _userRepository.CreateUserAsync(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(string id, UpdateDto model)
        {
            var user = _mapper.Map<User>(model);
            user.Id = id;

            try
            {
                AuthorizationResult isAuthorized =
                    await AuthorizationService.AuthorizeAsync(User, id, DotsAuthRequirements.Update);

                if (!isAuthorized.Succeeded)
                    return Forbid();

                await _userRepository.UpdateUserAsync(user, model.Password);

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            try
            {
                AuthorizationResult isAuthorized =
                    await AuthorizationService.AuthorizeAsync(User, id, DotsAuthRequirements.Delete);

                if (!isAuthorized.Succeeded)
                    return Forbid();

                await _userRepository.DeleteUserAsync(id);

                return Ok();
            }
            catch (AppException ex)
            {

                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
