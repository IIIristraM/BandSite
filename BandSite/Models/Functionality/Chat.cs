using System.Collections.Generic;
using BandSite.Models.Entities;
using System;
using System.Linq;

namespace BandSite.Models.Functionality
{
    public class Chat : IChat
    {
        public IEnumerable<Message> GetUnreadMessages(string userName)
        {
            using (var db = MvcApplication.DbFactory.CreateContext())
            {
                var user = db.UserProfiles.Content.FirstOrDefault(u => u.UserName == userName);
                if (user != null)
                {
                    var unreadMsgs = user.UnreadMessages();
                    foreach (var msg in unreadMsgs)
                    {
                        msg.Status = MessageStatus.Read.ToString();
                        yield return msg;
                    }
                    db.SaveChanges();
                }
            }
        }

        public IEnumerable<Message> Send(string userFromName, string[] usersToNames, string message)
        {
            using (var db = MvcApplication.DbFactory.CreateContext())
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
                                Status = MessageStatus.Read.ToString(),
                                Published = DateTime.Now,
                                UserFrom = userFrom,
                                UserTo = userTo
                            };
                            db.Messages.Insert(msg);
                            yield return msg;
                        }
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}