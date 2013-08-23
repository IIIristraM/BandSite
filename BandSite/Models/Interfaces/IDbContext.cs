using BandSite.Models.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandSite.Models.Interfaces
{
    public interface IDbContext: IDisposable
    {
        IRepository<Album> Albums { get; set; }
        IRepository<Song> Songs { get; set; }
        IRepository<UserProfile> UserProfiles { get; set; }
        IRepository<Playlist> Playlists { get; set; }
        IRepository<Message> Messages { get; set; }

        int SaveChanges();
    }
}
