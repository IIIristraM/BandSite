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
            if (ConnectedUsers.ContainsKey(Context.User.Identity.Name))
            {
                string value;
                ConnectedUsers.TryRemove(Context.User.Identity.Name, out value);
                Clients.Others.contactOffline(Context.User.Identity.Name);
                Clients.Caller.logout();
            }
            return base.OnDisconnected();
        }

        public override Task OnConnected()
        {
            ConnectedUsers.AddOrUpdate(Context.User.Identity.Name, Context.ConnectionId, (key, oldValue) => Context.ConnectionId);

            Clients.Others.contactOnline(new[] { Context.User.Identity.Name });
            Clients.Caller.login(ConnectedUsers.Select(u => u.Key).ToArray());
           
            return base.OnConnected();
        }

        public void LoadHistoryWith(string user)
        {
            foreach (var msg in _chat.GetHistory(Context.User.Identity.Name, user))
            {
                Clients.Caller.addMessage(user, msg.UserFrom.UserName, HttpUtility.HtmlEncode(msg.Text), msg.Published.Value.ToString(_dateTemplate));
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
                if (!ConnectedUsers.ContainsKey(msg.UserTo.UserName))
                {
                    msg.Status = MessageStatus.Unread.ToString();
                }
                else
                {
                    Clients.Client(ConnectedUsers[msg.UserTo.UserName]).addMessage(Context.User.Identity.Name, Context.User.Identity.Name, HttpUtility.HtmlEncode(message), msg.Published.Value.ToString(_dateTemplate));
                }
                Clients.Caller.addMessage(users, Context.User.Identity.Name, HttpUtility.HtmlEncode(message), msg.Published.Value.ToString(_dateTemplate));
            }
        }
    }
}