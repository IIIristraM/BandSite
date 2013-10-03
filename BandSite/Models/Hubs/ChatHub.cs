using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BandSite.Models.Entities;
using BandSite.Models.Functionality;
using Microsoft.AspNet.SignalR;

namespace BandSite.Models.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();
        private  readonly  IChat _chat;
        private readonly string _dateTemplate;

        public override Task OnDisconnected()
        {
            Logout();
            return base.OnDisconnected();
        }

        public override Task OnConnected()
        {
            Login();
            return base.OnConnected();
        }

        public void Logout()
        {
            if (ConnectedUsers.ContainsKey(Context.User.Identity.Name))
            {
                string value;
                ConnectedUsers.TryRemove(Context.User.Identity.Name, out value);
                Clients.Others.contactOffline(Context.User.Identity.Name);
                Clients.Caller.logout();
            }
        }

        public void Login()
        {
            ConnectedUsers.AddOrUpdate(Context.User.Identity.Name, Context.ConnectionId, (key, oldValue) => Context.ConnectionId);

            Clients.Others.contactOnline(new[] { Context.User.Identity.Name });
            Clients.Caller.login(ConnectedUsers.Select(u => u.Key).ToArray());
        }

        public void LoadHistoryWith(string user)
        {
            foreach (var msg in _chat.GetHistory(Context.User.Identity.Name, user))
            {
                if (msg.Status.ToString() == MessageStatus.Undelivered.ToString())
                {
                    msg.Status = MessageStatus.Unread;
                    if (ConnectedUsers.ContainsKey(msg.UserFrom.UserName))
                    {
                        Clients.Client(ConnectedUsers[msg.UserFrom.UserName]).messageDelivered(msg.Guid.ToString());
                    }
                }

                Clients.Caller.addMessage(user, 
                                          msg.UserFrom.UserName,
                                          new
                                          {
                                              guid = msg.Guid.ToString(),
                                              text = HttpUtility.HtmlEncode(msg.Text),
                                              date = msg.Published.Value.ToString(_dateTemplate),
                                              status = ((msg.UserFrom.UserName != Context.User.Identity.Name) || 
                                                       (msg.Status == MessageStatus.Undelivered))
                                                       ? msg.Status.ToString() : ""
                                          });
            }
        }

        public ChatHub(IChat chat)
        {
            _chat = chat;
            _dateTemplate = "dd-MMM  HH:mm";
        }

        public void AddMessage(string users, string message)
        {
            var list = users.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var msg in _chat.AddMessage(Context.User.Identity.Name, list, message))
            {
                if (ConnectedUsers.ContainsKey(msg.UserTo.UserName))
                {
                    msg.Status = MessageStatus.Unread;
                    Clients.Client(ConnectedUsers[msg.UserTo.UserName]).addMessage(Context.User.Identity.Name, 
                                                                                   Context.User.Identity.Name, 
                                                                                   new {
                                                                                       guid = msg.Guid.ToString(),
                                                                                       text = HttpUtility.HtmlEncode(msg.Text),
                                                                                       date = msg.Published.Value.ToString(_dateTemplate),
                                                                                       status = msg.Status.ToString()
                                                                                   });
                }
                Clients.Caller.addMessage(users, 
                                          Context.User.Identity.Name,
                                          new
                                          {
                                              guid = msg.Guid.ToString(),
                                              text = HttpUtility.HtmlEncode(msg.Text),
                                              date = msg.Published.Value.ToString(_dateTemplate),
                                              status = (msg.Status == MessageStatus.Undelivered) ? msg.Status.ToString() : ""
                                          });
            }
        }
    }
}