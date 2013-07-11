using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandSite.Models.Implementations
{
    public class Album : IEntity
    {
        private ICollection<Song> _songs;

        public virtual int Id { get; set; }

        [Display(Name = "Album Title")]
        public virtual string Title { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public virtual DateTime Published { get; set; }

        [DataType(DataType.MultilineText)]
        public virtual string Description { get; set; }

        public virtual ICollection<Song> Songs
        {
            get 
            {
                return _songs ?? (_songs = (new HashSet<Song>()));
            }
        }

        public bool TrySetPropertiesFrom(object source)
        {
            Album album = source as Album;
            if (album != null)
            {
                this.Id = album.Id;
                this.Published = album.Published;
                this.Description = album.Description;
                this.Title = album.Title;
                return true;
            }
            return false;
        }
    }
}