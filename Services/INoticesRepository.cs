using DotsApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DotsApi.Services
{
    public interface INoticesRepository
    {
        Task<IEnumerable<Notice>> GetNotices(string userId);
        Task<Notice> AddNotice(User user, Notice noticeDto);
        Task<Notice> AddNotice(string userId, Notice noticeDto);
    }
}
