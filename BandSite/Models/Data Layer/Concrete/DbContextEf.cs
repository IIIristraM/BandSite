using BandSite.Models.Entities;
using System;
using System.Data.Entity;

namespace BandSite.Models.DataLayer
{
    public class DbContextEf : DbContext, IDbContext
    {
        public DbContextEf() : this("DefaultConnection") {}

        public DbContextEf(string connectionName)
            : base("name=" + connectionName)
        {
            ((IDbContext)this).Albums = new RepositoryEf<Album>(Albums);
            ((IDbContext)this).Songs = new RepositoryEf<Song>(Songs);
            ((IDbContext)this).UserProfiles = new RepositoryEf<UserProfile>(UserProfiles);
            ((IDbContext)this).PlaylistItems = new RepositoryEf<PlaylistItem>(PlaylistItems);
            ((IDbContext)this).Messages = new RepositoryEf<Message>(Messages);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>().HasKey(u => u.Id);
            modelBuilder.Entity<Album>().HasKey(a => a.Id);
            modelBuilder.Entity<Song>().HasKey(s => s.Id);
            modelBuilder.Entity<PlaylistItem>().HasKey(p => p.Id);
            modelBuilder.Entity<Message>().HasKey(m => m.Id);

            modelBuilder.Entity<Album>()
                        .HasMany(a => a.Songs)
                        .WithMany(s => s.Albums)
                        .Map(t => t.MapLeftKey("AlbumId")
                        .MapRightKey("SongId")
                        .ToTable("SongAlbum"));

            modelBuilder.Entity<Message>()
                        .HasRequired(m => m.UserFrom)
                        .WithMany(u => u.OutputMessages)
                        .HasForeignKey(m => m.UserFromId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                        .HasRequired(m => m.UserTo)
                        .WithMany(u => u.InputMessages)
                        .HasForeignKey(m => m.UserToId)
                        .WillCascadeOnDelete(false);
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<PlaylistItem> PlaylistItems { get; set; }
        public DbSet<Message> Messages { get; set; }

        #region IDbContext Implementation

        IRepository<Album> IDbContext.Albums { get; set; }
        IRepository<Song> IDbContext.Songs { get; set; }
        IRepository<UserProfile> IDbContext.UserProfiles { get; set; }
        IRepository<PlaylistItem> IDbContext.PlaylistItems { get; set; }
        IRepository<Message> IDbContext.Messages { get; set; }

        int IDbContext.SaveChanges()
        {
            return base.SaveChanges();
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        #endregion
    }
}