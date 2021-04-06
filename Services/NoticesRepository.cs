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
            User user = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();

            return user.Notices;
        }

        public async Task<Notice> AddNotice(User user, Notice noticeDto)
        {
            try
            {
                User replacement = user;
                Notice notice = noticeDto;

                notice.Id = ObjectId.GenerateNewId().ToString();
                notice.UserId = replacement.Id;
                notice.TimeCreated = DateTime.Now;
                notice.IsCompleted = false;

                replacement.Notices.Append(notice);
                await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, replacement);
                return notice;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Notice> AddNotice(string userId, Notice noticeDto)
        {
            try
            {
                User replacement = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                Notice notice = noticeDto;

                notice.Id = ObjectId.GenerateNewId().ToString();
                notice.UserId = replacement.Id;
                notice.TimeCreated = DateTime.Now;
                notice.IsCompleted = false;

                replacement.Notices.Append(notice);
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
