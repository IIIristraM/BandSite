using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandSite.Models.Implementations
{
    public class UserProfile : IEntity
    {
        private ICollection<Playlist> _playlists;
        private ICollection<Message> _inputMessages;
        private ICollection<Message> _outputMessages;

        public int Id { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<Playlist> Playlists
        {
            get
            {
                return _playlists ?? (_playlists = (new HashSet<Playlist>()));
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

        public bool TrySetPropertiesFrom(object source)
        {
            UserProfile user = source as UserProfile;
            if (user != null)
            {
                this.Id = user.Id;
                this.UserName = user.UserName;
                return true;
            }
            return false;
        }
    }
}