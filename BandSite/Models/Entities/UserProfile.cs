using BandSite.Models.DataLayer;
using System.Collections.Generic;
using System.Linq;

namespace BandSite.Models.Entities
{
    public class UserProfile : EntityBase
    {
        private ICollection<PlaylistItem> _playlists;
        private ICollection<Message> _inputMessages;
        private ICollection<Message> _outputMessages;

        public string UserName { get; set; }
        public virtual ICollection<PlaylistItem> Playlists
        {
            get
            {
                return _playlists ?? (_playlists = (new HashSet<PlaylistItem>()));
            }
        }
        public virtual ICollection<Message> InputMessages 
        {
            get 
            {
                return _inputMessages ?? (_inputMessages = (new HashSet<Message>()));
            }
        }
        public virtual ICollection<Message> OutputMessages
        {
            get
            {
                return _outputMessages ?? (_outputMessages = (new HashSet<Message>()));
            }
        }

        public ICollection<Message> UnreadMessages()
        {
            return InputMessages.Where(m => m.Status == MessageStatus.Unread.ToString()).ToList();
        }

        public ICollection<Message> ReadMessages()
        {
            return InputMessages.Where(m => m.Status == MessageStatus.Read.ToString()).ToList();
        }

        public ICollection<Message> ReadMessages(int count)
        {
            return InputMessages.Where(m => m.Status == MessageStatus.Read.ToString()).Take(count).ToList();
        }
    }
}