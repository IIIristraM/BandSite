using BandSite.Models.Entities;
using System;

namespace BandSite.Models.DataLayer
{
    public interface IDbContext: IDisposable
    {
        IRepository<Album> Albums { get; set; }
        IRepository<Song> Songs { get; set; }
        IRepository<UserProfile> UserProfiles { get; set; }
        IRepository<PlaylistItem> PlaylistItems { get; set; }
        IRepository<Message> Messages { get; set; }

        int SaveChanges();
    }
}
