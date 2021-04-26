using DotsApi.Helpers;
using DotsApi.Models;
using DotsApi.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DotsTests
{
    internal class NoticesRepositoryTests
    {
        private IUsersRepository _usersRepository;
        private INoticesRepository _noticesRepository;
        private IMongoCollection<User> _usersCollection;

        [SetUp]
        public void Setup()
        {
            _usersRepository = new UsersRepository(Constants.databaseContext);
            _noticesRepository = new NoticesRepository(Constants.databaseContext);
            _usersCollection = Constants.databaseContext.Users;

            User testSubject = new User();
            testSubject.Id = "60848ae8fb71edf2a7ebf846";
            testSubject.Email = "test@subject.zero";
            testSubject.PasswordHash = "$2a$11$wvypXckqs4LxCCalZMBV9.CIkcUmayQAvvyISAPmlY4/Prs49Wkce";
            testSubject.Notices = new Notice[] { };
            Notice notice = new Notice()
            {
                Id = "608235aea059ac5c9af6da20",
                UserId = testSubject.Id,
                Name = "Test Notice 1",
                TimeCreated = DateTime.UtcNow,
                TimeCompleted = DateTime.UtcNow.AddDays(3),
                IsCompleted = false
            };
            testSubject.Notices = testSubject.Notices.Append(notice);

            if (_usersCollection.Find(u => u.Email == testSubject.Email).FirstOrDefault() == null)
                _usersCollection.InsertOne(testSubject);

        }

        [Test]
        public async Task TestGetNotices()
        {
            try
            {
                IEnumerable<Notice> notices = await _noticesRepository.GetNoticesAsync("60848ae8fb71edf2a7ebf846");

                Assert.NotNull(notices);
                Assert.AreEqual(notices.Count(), 1);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await _usersRepository.DeleteUserAsync("60848ae8fb71edf2a7ebf846");
            }
        }

        [Test]
        public async Task TestAddNotice()
        {
            try
            {
                Notice notice = new Notice()
                {
                    Name = "notice2",
                    TimeCompleted = DateTime.UtcNow.AddDays(2)
                };

                notice = await _noticesRepository.AddNoticeAsync("60848ae8fb71edf2a7ebf846", notice);
                Assert.NotNull(notice.TimeCreated);

                IEnumerable<Notice> notices = await _noticesRepository.GetNoticesAsync("60848ae8fb71edf2a7ebf846");

                Assert.AreEqual(notices.Count(), 2);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await _usersRepository.DeleteUserAsync("60848ae8fb71edf2a7ebf846");
            }
        }

        [Test]
        public async Task TestUpdateNotice()
        {
            Func<string, string, Notice> getNotice = (userId, noticeId) =>
                _usersCollection.Find(u => u.Id == userId).FirstOrDefault()
                    .Notices.Where(n => n.Id == noticeId).FirstOrDefault();

            const string UserId = "60848ae8fb71edf2a7ebf846";
            const string NoticeId = "608235aea059ac5c9af6da20";

            try
            {
                Notice notice = getNotice(UserId, NoticeId);
                Assert.NotNull(notice);

                notice.Name = "Name Changed";
                await _noticesRepository.UpdateNoticeAsync(notice);
                notice = getNotice(UserId, NoticeId);
                Assert.AreEqual(notice.Name, "Name Changed");

                DateTime oldTime = notice.TimeCompleted;
                notice.TimeCompleted = notice.TimeCompleted.AddMinutes(30);
                await _noticesRepository.UpdateNoticeAsync(notice);
                notice = getNotice(UserId, NoticeId);
                Assert.AreNotEqual(oldTime, notice.TimeCompleted);

                notice.Name = "changed again";
                notice.TimeCompleted = oldTime;
                await _noticesRepository.UpdateNoticeAsync(notice);
                notice = getNotice(UserId, NoticeId);
                Assert.AreEqual(notice.Name, "changed again");
                Assert.AreEqual(notice.TimeCompleted, oldTime);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await _usersRepository.DeleteUserAsync("60848ae8fb71edf2a7ebf846");
            }
        }

        [Test]
        public async Task TestDeleteNotice()
        {
            try
            {
                User user = await _usersCollection.Find(u => u.Id == "60848ae8fb71edf2a7ebf846").FirstOrDefaultAsync();
                Assert.AreEqual(user.Notices.Count(), 1);

                await _noticesRepository.DeleteNoticeAsync(user.Id, "608235aea059ac5c9af6da20");
                user = await _usersCollection.Find(u => u.Id == "60848ae8fb71edf2a7ebf846").FirstOrDefaultAsync();
                Assert.Zero(user.Notices.Count());
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                await _usersRepository.DeleteUserAsync("60848ae8fb71edf2a7ebf846");
            }

        }
    }
}
