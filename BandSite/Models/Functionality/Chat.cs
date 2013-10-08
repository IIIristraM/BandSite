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

        public IEnumerable<Message> GetHistory(string caller, string user)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                foreach (var msg in db.Messages.Content
                                               .Where(m => ((m.UserFrom.UserName == caller) && (m.UserTo.UserName == user)) ||
                                                           ((m.UserFrom.UserName == user) && (m.UserTo.UserName == caller)))
                                               .OrderBy(m => m.Published).ToList())
                {
                    yield return msg;
                }
                db.SaveChanges();
            }
        }

        public IEnumerable<Message> AddMessage(string userFromName, string[] usersToNames, string message)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var userFrom = db.UserProfiles.Content.FirstOrDefault(u => u.UserName == userFromName);
                if (userFrom != null)
                {
                    foreach (var userName in usersToNames)
                    {
                        var userTo = db.UserProfiles.Content.FirstOrDefault(u => u.UserName == userName);
                        if (userTo != null)
                        {
                            var msg = new Message
                            {
                                Text = message,
                                Status = MessageStatus.Undelivered,
                                Published = DateTime.Now,
                                UserFrom = userFrom,
                                UserTo = userTo,
                                Guid = Guid.NewGuid()
                            };
                            db.Messages.Insert(msg);
                            yield return msg;
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        public IEnumerable<Message> MarkReadMessages(string[] msgGuids)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var guids = msgGuids.Select(g => new Guid(g)).ToList();
                foreach (var msg in db.Messages.Content.Where(m => guids.Contains(m.Guid)))
                {
                    msg.Status = MessageStatus.Read;
                    yield return msg;
                }
                db.SaveChanges();
            }
        }
    }
}