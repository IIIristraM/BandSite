using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandSite.Models
{
    public class Song
    {
        public virtual int SongId { get; set; }
        public virtual string Title { get; set; }
        public virtual string Text { get; set; }
        public virtual ICollection<Album> Albums { get; set; }

        /*public Song()
        {
            Albums = new List<Album>();
        }*/
    }
}