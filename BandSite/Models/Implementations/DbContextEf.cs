using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BandSite.Models.Implementations
{
    public class DbContextEf : DbContext, IDbContext
    {
        public DbContextEf()
            : this("BandSiteDB")
        {
        }

        public DbContextEf(string connectionName)
            : base("name=" + connectionName)
        {
            ((IDbContext)this).Albums = new RepositoryEf<Album>(Albums);
            ((IDbContext)this).Songs = new RepositoryEf<Song>(Songs);
            ((IDbContext)this).UserProfiles = new RepositoryEf<UserProfile>(UserProfiles);
            ((IDbContext)this).Playlists = new RepositoryEf<Playlist>(Playlists);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>().HasKey(u => u.Id);
            modelBuilder.Entity<Album>().HasKey(a => a.Id);
            modelBuilder.Entity<Song>().HasKey(s => s.Id);

            modelBuilder.Entity<Album>()
                        .HasMany(a => a.Songs)
                        .WithMany(s => s.Albums)
                        .Map(t => t.MapLeftKey("AlbumId")
                        .MapRightKey("SongId")
                        .ToTable("SongAlbum"));
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

        #region IDbContext Implementation

        IRepository<Album> IDbContext.Albums { get; set; }
        IRepository<Song> IDbContext.Songs { get; set; }
        IRepository<UserProfile> IDbContext.UserProfiles { get; set; }
        IRepository<Playlist> IDbContext.Playlists { get; set; }

        int IDbContext.SaveChanges()
        {
            return base.SaveChanges();
        }

        void IDisposable.Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}