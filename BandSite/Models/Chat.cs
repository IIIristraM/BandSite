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
            }
        }

        public void Send(string user, string message)
        {
            if (_connectedUsers.ContainsKey(user))
            {
                Clients.Client(_connectedUsers[user]).addMessage(Context.User.Identity.Name, message);
            }
        }
    }
}