using AutoMapper;
using DotsApi.Models;
using DotsApi.Services;
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
            string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

            IEnumerable<Notice> notices = await _noticesRepository.GetNotices(userId);

            return Ok(notices);
        }
    }
}
