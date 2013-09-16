using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace BandSite.Models.Hubs
{
    public class UploaderHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HubConnectionContext> ConnectedUsers = new ConcurrentDictionary<string, HubConnectionContext>();

        public override Task OnConnected()
        {
            ConnectedUsers.AddOrUpdate(Context.User.Identity.Name, Clients, (key, oldValue) => Clients);
            return base.OnConnected();
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

        public override Task OnDisconnected()
        {
            if (ConnectedUsers.ContainsKey(Context.User.Identity.Name))
            {
                HubConnectionContext value;
                ConnectedUsers.TryRemove(Context.User.Identity.Name, out value);
            }
            return base.OnDisconnected();
        }
    }
}