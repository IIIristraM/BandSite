using BandSite.Models.Entities;
using System.Collections.Generic;

namespace BandSite.Models.Functionality
{
    public interface IChat
    {
        IEnumerable<Message> GetUnreadMessages(string userName);
        IEnumerable<Message> Send(string userFromName, string[] usersToNames, string message);
    }
}
