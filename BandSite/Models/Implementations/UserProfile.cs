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

        public int Id { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<Playlist> Playlists
        {
            get
            {
                return _playlists ?? (_playlists = (new HashSet<Playlist>()));
            }
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