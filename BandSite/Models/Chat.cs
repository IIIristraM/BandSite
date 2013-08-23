using BandSite.Models.Implementations;
using BandSite.Models.Interfaces;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandSite.Models
{
    public class Chat : Hub
    {
        private static Dictionary<string, string> _connectedUsers = new Dictionary<string, string>();
        private IDbContext _db = MvcApplication.DbFactory.CreateContext();

        public void Register()
        {
            lock (_connectedUsers)
            {
                if (!_connectedUsers.ContainsKey(Context.User.Identity.Name))
                {
                    _connectedUsers.Add(Context.User.Identity.Name, Context.ConnectionId);
                }
                else
                {
                    _connectedUsers[Context.User.Identity.Name] = Context.ConnectionId;
                }
                Clients.Others.onOnline(new string[] { Context.User.Identity.Name });
                Clients.Caller.onOnline(_connectedUsers.Select(u => u.Key).ToArray());
            }
            var user = _db.UserProfiles.Content.Where(u => u.UserName == Context.User.Identity.Name).First();
            var topReadMsgs = user.ReadMessages(3);
            foreach (var msg in topReadMsgs)
            {
                Clients.Caller.addMessage(msg.UserFrom.UserName, HttpUtility.HtmlEncode(msg.Text));
            }
            var unreadMsgs = user.UnreadMessages();
            foreach (var msg in unreadMsgs)
            {
                msg.Status = MessageStatus.Read.ToString();
                Clients.Caller.addMessage(msg.UserFrom.UserName, HttpUtility.HtmlEncode(msg.Text));
            }
            _db.SaveChanges();
        }

        public void Send(string users, string message)
        {
            var userFrom = _db.UserProfiles.Content.Where(u => u.UserName == Context.User.Identity.Name).First();
            var list = users.Split(new char[] {'#'}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < list.Length; i++)
            {
                var name = list[i];
                var userTo = _db.UserProfiles.Content.Where(u => u.UserName == name).First();
                var msgOut = new Message()
                {
                    Text = message,
                    Status = MessageStatus.Write.ToString(),
                    Published = DateTime.Now,
                    UserTo = userTo
                };
                userFrom.OutputMessages.Add(msgOut);
                var msgIn = new Message()
                {
                    Text = message,
                    Status = MessageStatus.Read.ToString(),
                    Published = DateTime.Now,
                    UserFrom = userFrom
                };
                if (_connectedUsers.ContainsKey(list[i]))
                {
                    Clients.Client(_connectedUsers[list[i]]).addMessage(Context.User.Identity.Name, HttpUtility.HtmlEncode(message));
                }
                else
                {
                    msgIn.Status = MessageStatus.Unread.ToString();
                }
                userTo.InputMessages.Add(msgIn);
            }
            _db.SaveChanges();
        }

        public void Logout()
        {
            lock (_connectedUsers)
            {
                if (_connectedUsers.ContainsKey(Context.User.Identity.Name))
                {
                    _connectedUsers.Remove(Context.User.Identity.Name);
                }
                Clients.Others.onOffline(Context.User.Identity.Name);
            }
        }
    }
}