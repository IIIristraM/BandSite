using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BandSite.Models.Entities;
using BandSite.Models.Functionality;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;

namespace BandSite.Models.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, List<string>> ConnectedUsers = new ConcurrentDictionary<string, List<string>>();
        private  readonly  IChat _chat;
        private readonly string _dateTemplate;
        private readonly int _macConnectionsPerUser;

        public ChatHub(IChat chat)
        {
            _chat = chat;
            _dateTemplate = "dd-MMM  HH:mm";
            _macConnectionsPerUser = 3;
        }

        public override Task OnDisconnected()
        {
            if (ConnectedUsers.ContainsKey(Context.User.Identity.Name))
            {
                var list = ConnectedUsers[Context.User.Identity.Name];
                if (list.Count > 1)
                {
                    list.Remove(Context.ConnectionId);
                }
                else
                {
                    List<string> value;
                    ConnectedUsers.TryRemove(Context.User.Identity.Name, out value);
                    Clients.Others.contactOffline(Context.User.Identity.Name);
                }
            }
            return base.OnDisconnected();
        }

        public override Task OnConnected()
        {
            var list = ConnectedUsers.GetOrAdd(Context.User.Identity.Name, new List<string>());
            list.Add(Context.ConnectionId);
            if (list.Count > 3) 
            {
                Clients.Client(list[0]).disconnect();
                list.RemoveAt(0); 
            }
            Clients.Others.contactOnline(Context.User.Identity.Name);
            Clients.Caller.login(Context.User.Identity.Name, ConnectedUsers.Select(u => u.Key).ToArray());
            return base.OnConnected();
        }

        public void LoadHistoryWith(string user)
        {
            foreach (var msg in _chat.GetHistory(Context.User.Identity.Name, user))
            {
                if ((msg.Status == MessageStatus.Undelivered) && (msg.UserTo.UserName == Context.User.Identity.Name))
                {
                    msg.Status = MessageStatus.Unread;
                }
                if (ConnectedUsers.ContainsKey(user))
                {
                    foreach (var conn in ConnectedUsers[user])
                    {
                        Clients.Client(conn).messagesDelivered(Context.User.Identity.Name);
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

        public void AddMessage(string users, string message)
        {
            var list = users.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            MessageStatus callbackStatus = MessageStatus.Undelivered;
            foreach (var msg in _chat.AddMessage(Context.User.Identity.Name, list, message))
            {
                if (ConnectedUsers.ContainsKey(msg.UserTo.UserName) && (msg.UserTo.UserName != Context.User.Identity.Name))
                {
                    msg.Status = MessageStatus.Unread;
                    callbackStatus = MessageStatus.Unread;
                    foreach (var conn in ConnectedUsers[msg.UserTo.UserName])
                    {
                        Clients.Client(conn).addMessage((list.Length == 1) ? Context.User.Identity.Name : users,
                                                        Context.User.Identity.Name,
                                                        new
                                                        {
                                                            guid = msg.Guid.ToString(),
                                                            text = HttpUtility.HtmlEncode(msg.Text),
                                                            date = msg.Published.Value.ToString(_dateTemplate),
                                                            status = msg.Status.ToString()
                                                        });
                    }
                }
            }
            foreach (var conn in ConnectedUsers[Context.User.Identity.Name])
            {
                Clients.Client(conn).addMessage(users,
                                                Context.User.Identity.Name,
                                                new
                                                {
                                                    guid = Guid.NewGuid().ToString(),
                                                    text = HttpUtility.HtmlEncode(message),
                                                    date = DateTime.Now.ToString(_dateTemplate),
                                                    status = (callbackStatus == MessageStatus.Undelivered) ? callbackStatus.ToString() : ""
                                                });
            }     
        }

        public void MarkReadMessages(string[] msgGuids)
        {
            foreach (var msg in _chat.MarkReadMessages(msgGuids))
            {
                foreach (var conn in ConnectedUsers[Context.User.Identity.Name])
                {
                    Clients.Client(conn).markReadMessage(msg.Guid.ToString());
                }
            }
        }

        public void CreateConference(string title, string[] users)
        {
            _chat.CreateConference(title, users);
            foreach (var user in users)
            {
                if (ConnectedUsers.ContainsKey(user))
                {
                    foreach (var conn in ConnectedUsers[user])
                    {
                        Clients.Client(conn).createConference(title, users);
                    }
                }
            }
        }
    }
}