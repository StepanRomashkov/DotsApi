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

        public NoticesRepository(DotsDatabaseContext context)
        {
            _context = context;
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

        public async Task UpdateNoticeAsync(Notice updateNoticeDto)
        {
            List<UpdateDefinition<User>> updateList = new List<UpdateDefinition<User>>();
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, updateNoticeDto.UserId);
            User user = await _context.Users.Find(filter).FirstOrDefaultAsync();
            Notice notice = user.Notices.Where(n => n.Id == updateNoticeDto.Id).FirstOrDefault();

            FilterDefinition<User> filterNotice = Builders<User>.Filter
                .And(filter, Builders<User>.Filter.ElemMatch(n => n.Notices, n => n.Id == updateNoticeDto.Id));

            if (!string.IsNullOrWhiteSpace(updateNoticeDto.Name) && notice.Name != updateNoticeDto.Name)
            {
                UpdateDefinition<User> updateName = Builders<User>.Update
                    .Set(u => u.Notices.ElementAt(-1).Name, updateNoticeDto.Name);
                
                updateList.Add(updateName);
            }

            if (updateNoticeDto.TimeCompleted != DateTime.MinValue 
                && notice.TimeCompleted != updateNoticeDto.TimeCompleted)
            {
                UpdateDefinition<User> updateTime = Builders<User>.Update
                    .Set(u => u.Notices.ElementAt(-1).TimeCompleted, updateNoticeDto.TimeCompleted);
                
                updateList.Add(updateTime);
            }

            UpdateDefinition<User> updateFinal = Builders<User>.Update.Combine(updateList);

            if(updateList.Count != 0)
                await _context.Users.UpdateOneAsync(filterNotice, updateFinal);
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
