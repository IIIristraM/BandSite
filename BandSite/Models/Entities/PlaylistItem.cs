using BandSite.Models.DataLayer;

namespace BandSite.Models.Entities
{
    public class PlaylistItem: EntityBase
    {
        public virtual int SongId { get; set; }
        public virtual int UserId { get; set; }
        public virtual int Order { get; set; }
        public virtual Song Song { get; set; }
        public virtual UserProfile User { get; set; }
    }
}