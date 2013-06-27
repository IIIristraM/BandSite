using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandSite.Models
{
    public class Album
    {
        public virtual int AlbumId { get; set; }
        public virtual string Title { get; set; }
        public virtual DateTime Published { get; set; }
        public virtual string Description { get; set; }
        public virtual ICollection<Song> Songs { get; set; }

        /*public Album()
        {
            Songs = new List<Song>();
        }*/
    }
}