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
        protected IRepository<Album> albumsRepo;
        protected IRepository<Song> songsRepo;

        public DbContextEf(string connectionName)
            : base("name=" + connectionName)
        {
            albumsRepo = new RepositoryEf<Album>(Albums);
            songsRepo = new RepositoryEf<Song>(Songs);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>()
                        .HasMany(a => a.Songs)
                        .WithMany(s => s.Albums)
                        .Map(t => t.MapLeftKey("AlbumId")
                        .MapRightKey("SongId")
                        .ToTable("SongAlbum"));
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }

        IRepository<Album> IDbContext.Albums
        {
            get { return albumsRepo; }
            set { albumsRepo = value; }
        }

        IRepository<Song> IDbContext.Songs
        {
            get { return songsRepo; }
            set { songsRepo = value; }
        }

        int IDbContext.SaveChanges()
        {
            return base.SaveChanges();
        }

        void IDisposable.Dispose()
        {
            base.Dispose();
        }
    }
}