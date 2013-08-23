using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace BandSite.Models
{
    public class Uploader : Hub
    {
        private static Dictionary<string, HubConnectionContext> _connectedUsers = new Dictionary<string, HubConnectionContext>();

        public void CreateAnchor()
        {
            lock (_connectedUsers)
            {
                if (!_connectedUsers.ContainsKey(Context.User.Identity.Name))
                {
                    _connectedUsers.Add(Context.User.Identity.Name, Clients);
                }
                else
                {
                    _connectedUsers[Context.User.Identity.Name] = Clients;
                }
            }
        }

        public byte[] Upload(byte[] buffer, Stream stream, string userName)
        {
            var offset = 0;
            while (stream.Position < stream.Length)
            {
                stream.Read(buffer, offset, 4096);
                offset += 4096;
                var percentage = (double)stream.Position / stream.Length * 100;
                _connectedUsers[userName].Caller.showProgress(Math.Round(percentage, 2));
            }
            return buffer;
        }
    }
}