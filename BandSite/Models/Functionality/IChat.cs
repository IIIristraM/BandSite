using BandSite.Models.Entities;
using System;
using System.Collections.Generic;

namespace BandSite.Models.Functionality
{
    public interface IChat
    {
        IEnumerable<Message> GetHistory(string user, string confGuid);
        IEnumerable<Message> AddMessage(string user, string confGuid, string message);
        IEnumerable<Message> GetMessages(string[] msgGuids);
        IEnumerable<Message> GetUndeliveredMessages(string confGuid);
        Guid CreateConference(string title, string[] users);
    }
}
