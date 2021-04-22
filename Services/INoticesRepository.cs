using DotsApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DotsApi.Services
{
    public interface INoticesRepository
    {
        Task<IEnumerable<Notice>> GetNoticesAsync(string userId);
        Task<Notice> AddNoticeAsync(string userId, Notice noticeDto);
        Task UpdateNoticeAsync(Notice noticeDto);
        Task DeleteNoticeAsync(string userId, string id);
    }
}
