using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandSite.Models.Implementations
{
    public class Playlist: IEntity
    {
        public virtual int Id { get; set; }
        public virtual int SongId { get; set; }
        public virtual int UserId { get; set; }
        public virtual int Order { get; set; }
        public virtual Song Song { get; set; }
        public virtual UserProfile User { get; set; }

        public bool TrySetPropertiesFrom(object source)
        {
            Playlist playlist = source as Playlist;
            if (playlist != null)
            {
                this.SongId = playlist.SongId;
                this.UserId = playlist.UserId;
                this.Order = playlist.Order;
                return true;
            }
            return false;
        }
    }
}