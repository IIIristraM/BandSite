using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace BandSite.Models.Functionality
{
    public class Uploader : Hub
    {
        private static readonly ConcurrentDictionary<string, HubConnectionContext> ConnectedUsers = new ConcurrentDictionary<string, HubConnectionContext>();

        public void CreateAnchor()
        {
            ConnectedUsers.AddOrUpdate(Context.User.Identity.Name, Clients, (key, oldValue) => Clients);
        }

        public byte[] Upload(byte[] buffer, Stream stream, string userName)
        {
            var offset = 0;
            while (stream.Position < stream.Length)
            {
                stream.Read(buffer, offset, 4096);
                offset += 4096;
                var percentage = (double)stream.Position / stream.Length * 100;
                if(ConnectedUsers.ContainsKey(userName))
                {
                    HubConnectionContext context;
                    if (ConnectedUsers.TryGetValue(userName, out context))
                    {
                        context.Caller.showProgress(Math.Round(percentage, 2));
                    }
                }
            }
            return buffer;
        }
    }
}