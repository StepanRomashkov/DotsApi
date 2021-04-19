using AutoMapper;
using DotsApi.Models;
using DotsApi.Services;
using DotsApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace DotsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NoticesController : ControllerBase
    {
        private INoticesRepository _noticesRepository;
        private IMapper _mapper;

        public NoticesController(
            INoticesRepository noticesRepository,
            IMapper mapper)
        {
            _noticesRepository = noticesRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetNoticesAsync()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

                IEnumerable<Notice> notices = await _noticesRepository.GetNoticesAsync(userId);

                return Ok(notices);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddNoticeAsync(AddNoticeDto model)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                Notice addNoticeDto = _mapper.Map<Notice>(model);

                Notice notice = await _noticesRepository.AddNoticeAsync(userId, addNoticeDto);

                return Ok(notice);
            }
            catch (AppException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNoticeAsync(string id)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                
                await _noticesRepository.DeleteNoticeAsync(userId, id);

                return Ok();
            }
            catch (AppException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
