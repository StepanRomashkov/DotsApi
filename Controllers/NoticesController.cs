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
        public async Task<IActionResult> GetNotices()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

                IEnumerable<Notice> notices = await _noticesRepository.GetNotices(userId);

                return Ok(notices);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddNotice(AddNoticeDto model)
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
                Notice addNoticeDto = _mapper.Map<Notice>(model);

                Notice notice = await _noticesRepository.AddNotice(userId, addNoticeDto);

                return Ok(notice);
            }
            catch (AppException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
