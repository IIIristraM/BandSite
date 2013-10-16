using System.Collections.Generic;
using System.Web.UI.WebControls;
using BandSite.Models.DataLayer;
using BandSite.Models.Entities;
using System;
using System.Linq;

namespace BandSite.Models.Functionality
{
    public class Chat : IChat
    {
        private readonly IDbContextFactory _dbContextFactory;

        public Chat(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IEnumerable<Message> GetHistory(string user, string confGuid)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var conf = db.Conferences.Content.First(c => c.Guid == new Guid(confGuid));
                foreach (var msg in conf.Messages.Where(m => m.UserTo.UserName == user).OrderBy(m => m.Published))
                {
                    yield return msg;
                }
                db.SaveChanges();
            }
        }

        public IEnumerable<Message> AddMessage(string user, string confGuid, string message)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var conf = db.Conferences.Content.First(c => c.Guid == new Guid(confGuid));
                var sender = conf.Users.First(u => u.UserName == user);
                foreach (var usr in MoveSenderToTheEnd(conf.Users.ToList(), sender))
                {
                    yield return db.Messages.Insert(new Message()
                    {
                        Guid = Guid.NewGuid(),
                        Text = message,
                        UserFrom = sender,
                        UserTo = usr,
                        Published = DateTime.Now,
                        Status = MessageStatus.Undelivered,
                        Conference = conf
                    });
                }
                db.SaveChanges();
            }
        }

        public IEnumerable<Message> GetMessages(string[] msgGuids)
        {
            var guids = msgGuids.Select(g => new Guid(g)).ToArray();
            using (var db = _dbContextFactory.CreateContext())
            {
                foreach (var msg in db.Messages.Content.Where(m => guids.Contains(m.Guid)))
                {
                    yield return msg;
                }
                db.SaveChanges();
            }
        }

        public Guid CreateConference(string title, string[] users)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var conf = db.Conferences.Insert(new Conference()
                {
                    Title = title,
                    Guid = Guid.NewGuid()
                });
                foreach (var user in db.UserProfiles.Content.Where(u => users.Contains(u.UserName)))
                {
                    conf.Users.Add(user);
                }
                db.SaveChanges();
                return conf.Guid;
            }
        }

        public IEnumerable<Message> GetUndeliveredMessages(string confGuid)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var conf = db.Conferences.Content.First(c => c.Guid == new Guid(confGuid));
                foreach (var msg in conf.Messages.Where(m => m.Status == MessageStatus.Undelivered))
                {
                    yield return msg;
                }
                db.SaveChanges();
            }
        }

        public void RemoveUserFromConference(string confGuid, string user)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var conf = db.Conferences.Content.First(c => c.Guid == new Guid(confGuid));
                var target = db.UserProfiles.Content.First(u => u.UserName == user);
                conf.Users.Remove(target);
                if (conf.Users.Count == 0)
                {
                    db.Conferences.Delete(conf);
                }
                db.SaveChanges();
            }
        }

        public UserProfile[] GetUsersInConference(string confGuid)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var conf = db.Conferences.Content.First(c => c.Guid == new Guid(confGuid));
                return conf.Users.ToArray();
            }
        }

        private List<UserProfile> MoveSenderToTheEnd(List<UserProfile> users, UserProfile sender)
        {
            var senderIndex = users.IndexOf(sender);
            if (senderIndex != users.Count - 1)
            {
                users.Add(sender);
                users.RemoveAt(senderIndex);
            }
            return users;
        }
    }
}