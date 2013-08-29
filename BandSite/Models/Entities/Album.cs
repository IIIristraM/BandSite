using BandSite.Models.DataLayer;
using System;
using System.Collections.Generic;

namespace BandSite.Models.Entities
{
    public class Album : EntityBase
    {
        private ICollection<Song> _songs;

        public virtual string Title { get; set; }
        public virtual DateTime? Published { get; set; }
        public virtual string Description { get; set; }
        public virtual ICollection<Song> Songs
        {
            get
            {
                return _songs ?? (_songs = (new HashSet<Song>()));
            }
        }
        
    }
}