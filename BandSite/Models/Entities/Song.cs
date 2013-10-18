using BandSite.Models.DataLayer;
using System.Collections.Generic;

namespace BandSite.Models.Entities
{
    public class Song : EntityBase
    {
        private ICollection<Album> _albums;
        private ICollection<PlaylistItem> _playlists;

        public virtual string Title { get; set; }
        public virtual string Text { get; set; }
        public virtual byte[] File { get; set; }
        public virtual string Band { get; set; }
        public virtual ICollection<Album> Albums 
        { 
            get
            {
                return _albums ?? (_albums = new HashSet<Album>());
            }
        }
        public virtual ICollection<PlaylistItem> Playlists
        {
            get
            {
                return _playlists ?? (_playlists = new HashSet<PlaylistItem>());
            }
        }
    }
}