using System;
using System.Collections.Concurrent;
using System.Linq;
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

        public ChatHub(IChat chat)
        {
            _chat = chat;
        }

        public void Register()
        {
            ConnectedUsers.AddOrUpdate(Context.User.Identity.Name, Context.ConnectionId, (key, oldValue) => Context.ConnectionId);

            Clients.Others.onOnline(new[] { Context.User.Identity.Name });
            Clients.Caller.onOnline(ConnectedUsers.Select(u => u.Key).ToArray());

            foreach (var msg in _chat.GetUnreadMessages(Context.User.Identity.Name))
            {
                Clients.Caller.addMessage(msg.UserFrom.UserName, HttpUtility.HtmlEncode(msg.Text));
            }
        }

        public void Send(string users, string message)
        {
            var list = users.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var msg in _chat.Send(Context.User.Identity.Name, list, message))
            {
                 if (ConnectedUsers.ContainsKey(msg.UserTo.UserName))
                 {
                     string connectionId;
                     if (ConnectedUsers.TryGetValue(msg.UserTo.UserName, out connectionId))
                     {
                         Clients.Client(connectionId).addMessage(Context.User.Identity.Name, HttpUtility.HtmlEncode(message));
                     }
                 }
                 else
                 {
                     msg.Status =  MessageStatus.Unread.ToString();
                 }
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