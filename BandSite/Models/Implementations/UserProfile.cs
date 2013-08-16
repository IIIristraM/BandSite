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
        private ICollection<Song> _songs;

        public int Id { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<Song> Songs
        {
            get
            {
                return _songs ?? (_songs = (new HashSet<Song>()));
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