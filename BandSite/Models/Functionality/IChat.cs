using BandSite.Models.Entities;
using System.Collections.Generic;

namespace BandSite.Models.Functionality
{
    public interface IChat
    {
        IEnumerable<Message> GetHistory(string caller, string user);
        IEnumerable<Message> AddMessage(string userFromName, string[] usersToNames, string message);
        IEnumerable<Message> MarkReadMessages(string[] msgGuids);
        void CreateConference(string Title, string[] users);
    }
}
