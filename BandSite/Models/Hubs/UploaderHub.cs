using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace BandSite.Models.Hubs
{
    public class UploaderHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HubConnectionContext> ConnectedUsers = new ConcurrentDictionary<string, HubConnectionContext>();

        public void CreateAnchor()
        {
            ConnectedUsers.AddOrUpdate(Context.User.Identity.Name, Clients, (key, oldValue) => Clients);
        }

        public void ShowProgress(string userName, double percentage)
        {
            if(ConnectedUsers.ContainsKey(userName))
            {
                HubConnectionContext context;
                if (ConnectedUsers.TryGetValue(userName, out context))
                {
                    context.Caller.showProgress(percentage);
                }
            }
        }
    }
}