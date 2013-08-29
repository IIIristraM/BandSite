using BandSite.Models.DataLayer;
using BandSite.Models.Entities;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace BandSite.Models.Functionality
{
    public class Chat : Hub
    {
        private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();
        private readonly IDbContext _db = MvcApplication.DbFactory.CreateContext();

        public void Register()
        {
            ConnectedUsers.AddOrUpdate(Context.User.Identity.Name, Context.ConnectionId, (key, oldValue) => Context.ConnectionId);

            Clients.Others.onOnline(new[] { Context.User.Identity.Name });
            Clients.Caller.onOnline(ConnectedUsers.Select(u => u.Key).ToArray());

            GetUnreadMessages();
        }

        public void GetUnreadMessages()
        {
            var user = _db.UserProfiles.Content.FirstOrDefault(u => u.UserName == Context.User.Identity.Name);
            if (user != null)
            {
                var unreadMsgs = user.UnreadMessages();
                foreach (var msg in unreadMsgs)
                {
                    msg.Status = MessageStatus.Read.ToString();
                    Clients.Caller.addMessage(msg.UserFrom.UserName, HttpUtility.HtmlEncode(msg.Text));
                }
                _db.SaveChanges();
            }
        }

        public void Send(string users, string message)
        {
            var userFrom = _db.UserProfiles.Content.FirstOrDefault(u => u.UserName == Context.User.Identity.Name);
            if (userFrom != null)
            {
                var list = users.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var user in list)
                {
                    var name = user;
                    var userTo = _db.UserProfiles.Content.FirstOrDefault(u => u.UserName == name);
                    if (userTo != null)
                    {
                        var msgOut = new Message
                        {
                            Text = message,
                            Status = MessageStatus.Write.ToString(),
                            Published = DateTime.Now,
                            UserTo = userTo
                        };
                        userFrom.OutputMessages.Add(msgOut);
                        var msgIn = new Message
                        {
                            Text = message,
                            Status = MessageStatus.Read.ToString(),
                            Published = DateTime.Now,
                            UserFrom = userFrom
                        };
                        if (ConnectedUsers.ContainsKey(user))
                        {
                            string connectionId;
                            if (ConnectedUsers.TryGetValue(user, out connectionId))
                            {
                                Clients.Client(connectionId).addMessage(Context.User.Identity.Name, HttpUtility.HtmlEncode(message));
                            }
                        }
                        else
                        {
                            msgIn.Status = MessageStatus.Unread.ToString();
                        }
                        userTo.InputMessages.Add(msgIn);
                    }
                }
                _db.SaveChanges();
            }
        }

        public void Logout()
        {
            if (ConnectedUsers.ContainsKey(Context.User.Identity.Name))
            {
                string value;
                ConnectedUsers.TryRemove(Context.User.Identity.Name, out value);
                Clients.Others.onOffline(Context.User.Identity.Name);
            }
        }
    }
}