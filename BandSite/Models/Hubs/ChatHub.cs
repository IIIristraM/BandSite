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
        private readonly int _maxConnectionsPerUser;

        public ChatHub(IChat chat)
        {
            _chat = chat;
            _dateTemplate = "dd-MMM  HH:mm";
            _maxConnectionsPerUser = 3;
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
                    foreach (var user in ConnectedUsers.Keys.Where(k => k != Context.User.Identity.Name))
                    {
                        foreach (var conn in ConnectedUsers[user])
                        {
                            Clients.Client(conn).contactOffline(new string[] {Context.User.Identity.Name });
                        }
                    }
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
            foreach (var user in ConnectedUsers.Keys.Where(k => k != Context.User.Identity.Name))
            {
                foreach (var conn in ConnectedUsers[user])
                {
                    Clients.Client(conn).contactOnline(new string[] {Context.User.Identity.Name});
                }
            }
            Clients.Caller.login(Context.User.Identity.Name, ConnectedUsers.Keys.Where(k => k != Context.User.Identity.Name));
            return base.OnConnected();
        }

        private void MessagesDelivered(string confGuid)
        {
            foreach (var msg in _chat.GetUndeliveredMessages(confGuid))
            {
                msg.Status = (msg.UserFrom.UserName != msg.UserTo.UserName) ? MessageStatus.Unread : MessageStatus.Read;
                if (ConnectedUsers.ContainsKey(msg.UserFrom.UserName))
                {
                    foreach (var conn in ConnectedUsers[msg.UserFrom.UserName])
                    {
                        Clients.Client(conn).messagesDelivered(msg.Conference.Guid);
                    }
                }
            }
        }

        public void LoadHistory(string confGuid)
        {
            MessagesDelivered(confGuid);
            foreach (var msg in _chat.GetHistory(Context.User.Identity.Name, confGuid))
            {
                Clients.Caller.addMessage(msg.Conference.Guid.ToString(), 
                                          new
                                          {
                                              guid = msg.Guid,
                                              text = HttpUtility.HtmlEncode(msg.Text),
                                              sender = msg.UserFrom.UserName,
                                              date = msg.Published.Value.ToString(_dateTemplate),
                                              status = msg.Status.ToString()
                                          });
            }
        }

        public void AddMessage(string confGuid, string message)
        {
            var callbackStatus = MessageStatus.Undelivered;
            Message callbackMsg = null;

            foreach (var msg in _chat.AddMessage(Context.User.Identity.Name, confGuid, message))
            {
                if (msg.UserFrom.UserName != msg.UserTo.UserName)
                {
                    if (ConnectedUsers.ContainsKey(msg.UserTo.UserName))
                    {
                        msg.Status = MessageStatus.Unread;
                    }
                    callbackStatus = (msg.Status != MessageStatus.Undelivered) ? msg.Status : callbackStatus;
                }
                else
                {
                    msg.Status = (callbackStatus == MessageStatus.Undelivered) ? callbackStatus : MessageStatus.Read;
                }
                if (ConnectedUsers.ContainsKey(msg.UserTo.UserName))
                {
                    foreach (var conn in ConnectedUsers[msg.UserTo.UserName])
                    {
                        Clients.Client(conn).addMessage(msg.Conference.Guid.ToString(),
                                                        new
                                                        {
                                                            guid = msg.Guid,
                                                            text = HttpUtility.HtmlEncode(msg.Text),
                                                            sender = msg.UserFrom.UserName,
                                                            date = msg.Published.Value.ToString(_dateTemplate),
                                                            status = msg.Status.ToString()
                                                        });
                    }
                }
            }  
        }

        public void MarkReadMessages(string[] msgGuids)
        {
            foreach (var msg in _chat.GetMessages(msgGuids))
            {
                msg.Status = MessageStatus.Read;
                foreach (var conn in ConnectedUsers[Context.User.Identity.Name])
                {
                    Clients.Client(conn).markReadMessage(msg.Guid);
                }
            }
        }

        public void CreateConference(string title, string[] users)
        {
            var guid = _chat.CreateConference(title, users);
            foreach (var user in users)
            {
                if (ConnectedUsers.ContainsKey(user))
                {
                    foreach (var conn in ConnectedUsers[user])
                    {
                        Clients.Client(conn).createConference(new
                        {
                            guid = guid,
                            title = title,
                            users = users
                        },
                        ConnectedUsers.Keys.Where(k => users.Contains(k) && k != user));
                    }
                }
            }
        }

        public void RemoveUserFromConference(string confGuid, string user) 
        {
            string[] users = _chat.GetUsersInConference(confGuid).Where(u => u.UserName != user).Select(u => u.UserName).ToArray();
            _chat.RemoveUserFromConference(confGuid, user);
            foreach (var u in users)
            {
                if (ConnectedUsers.ContainsKey(u))
                {
                    foreach (var conn in ConnectedUsers[u])
                    {
                        Clients.Client(conn).removeUserFromConference(confGuid, user);
                    }
                }
            }
            if (ConnectedUsers.ContainsKey(user))
            {
                foreach (var conn in ConnectedUsers[user])
                {
                    Clients.Client(conn).removeConference(confGuid);
                }
            }
        }
    }
}