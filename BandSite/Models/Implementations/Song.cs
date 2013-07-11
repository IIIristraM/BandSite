using BandSite.Models.Implementations;
using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandSite.Models.Implementations
{
    public class Song : IEntity
    {
        private ICollection<Album> _albums;

        public virtual int Id { get; set; }

        [Display(Name = "Song Title")]
        public virtual string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public virtual string Text { get; set; }

        public virtual ICollection<Album> Albums 
        { 
            get
            {
                return _albums ?? (_albums = new HashSet<Album>());
            }
        }

        public bool TrySetPropertiesFrom(object source)
        {
            Song song = source as Song;
            if (song != null)
            {
                this.Id = song.Id;
                this.Text = song.Text;
                this.Title = song.Title;
                return true;
            }
            return false;
        }
    }
}