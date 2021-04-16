using MongoDB.Bson;
using MongoDB.Driver;
using DotsApi.Helpers;
using DotsApi.Models;
using System;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotsApi.Services
{
    public class NoticesRepository : INoticesRepository
    {
        private readonly DotsDatabaseContext _context = null;

        public NoticesRepository(IOptions<DotsDatabaseSettings> settings)
        {
            _context = new DotsDatabaseContext(settings);
        }

        public async Task<IEnumerable<Notice>> GetNotices(string userId)
        {
            try
            {
                User user = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();

                return user.Notices;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Notice> AddNotice(string userId, Notice noticeDto) //needs to be refactored
        {
            try
            {
                User replacement = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                Notice notice = noticeDto;
                IEnumerable<Notice> noticesColl = replacement.Notices ?? new List<Notice>();

                notice.Id = ObjectId.GenerateNewId().ToString();
                notice.UserId = replacement.Id;
                notice.TimeCreated = DateTime.Now;
                notice.IsCompleted = false;

                replacement.Notices = noticesColl.Append(notice).ToArray();
                await _context.Users.ReplaceOneAsync(u => u.Id == userId, replacement);
                return notice;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
