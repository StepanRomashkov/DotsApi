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

        public async Task<IEnumerable<Notice>> GetNoticesAsync(string userId)
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

        public async Task<Notice> AddNoticeAsync(string userId, Notice noticeDto)
        {
            try
            {
                FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, userId);
                Notice notice = noticeDto;

                notice.Id = ObjectId.GenerateNewId().ToString();
                notice.UserId = userId;
                notice.TimeCreated = DateTime.Now;
                notice.IsCompleted = false;

                UpdateDefinition<User> updateAddNotice = Builders<User>
                    .Update.Push<Notice>(u => u.Notices, notice);

                await _context.Users.UpdateOneAsync(filter, updateAddNotice);

                return notice;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteNoticeAsync(string userId, string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            UpdateDefinition<User> updateDeleteNotice = Builders<User>
                .Update.PullFilter(u => u.Notices, n => n.Id == id);

            await _context.Users.UpdateOneAsync(filter, updateDeleteNotice);
        }
    }
}
