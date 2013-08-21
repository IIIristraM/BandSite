using BandSite.Models;
using BandSite.Models.Implementations;
using BandSite.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandSite.Tests.Models
{
    [TestClass]
    public class DataAccessTests
    {
        IDbContext _db;
        IDbContextFactory _dbfactory;

        [TestInitialize]
        public void InitializeDb()
        {
            _dbfactory = new DbContextEfFactory("BandSiteDB");
        }

        [TestMethod]
        [TestCategory("CRUD_Albums_Songs")]
        public void InsertAlbum()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var albumsCount = _db.Albums.Content.Count();

                var album = _db.Albums.Insert(new Album()
                {
                    Title = "Album " + Guid.NewGuid(),
                    Published = DateTime.Now
                });
                _db.SaveChanges();

                Assert.AreEqual(_db.Albums.Content.Count(), albumsCount + 1);
                Assert.IsNotNull(album.Songs);
            }
        }

        [TestMethod]
        [TestCategory("CRUD_Albums_Songs")]
        public void InsertSong()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var songsCount = _db.Songs.Content.Count();

                var song = _db.Songs.Insert(new Song()
                {
                    Title = "Song " + Guid.NewGuid()
                });
                _db.SaveChanges();

                Assert.AreEqual(_db.Songs.Content.Count(), songsCount + 1);
                Assert.IsNotNull(song.Albums);
            }
        }

        [TestMethod]
        [TestCategory("CRUD_Albums_Songs")]
        public void AddSongToAlbum()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var album = _db.Albums.Insert(new Album()
                {
                    Title = "Album " + Guid.NewGuid(),
                    Published = DateTime.Now
                });
                var songsInAlbum = album.Songs.Count();
                var song = _db.Songs.Insert(new Song()
                {
                    Title = "Song " + Guid.NewGuid()
                });
                var songContainers = song.Albums.Count();
                album.Songs.Add(song);
                _db.SaveChanges();

                Assert.AreEqual(album.Songs.Count(), songsInAlbum + 1);
                Assert.AreEqual(song.Albums.Count(), songContainers + 1);
            }
        }

        [TestMethod]
        [TestCategory("CRUD_Albums_Songs")]
        public void DeleteSong()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var song = _db.Songs.Content.Where(s => s.Albums.Count() > 0).First();
                var songsCount = _db.Songs.Content.Count();

                _db.Songs.Delete(song);
                _db.SaveChanges();

                Assert.AreEqual(_db.Songs.Content.Count(), songsCount - 1);
                Assert.AreEqual(
                    _db.Albums.Content.Where(
                         a => a.Songs.Where(s => s.Id == song.Id).Count() > 0
                    ).Count(),
                    0
                );
            }
        }

        [TestMethod]
        [TestCategory("CRUD_Albums_Songs")]
        public void AddAlbumToSong()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var album = _db.Albums.Insert(new Album()
                {
                    Title = "Album " + Guid.NewGuid(),
                    Published = DateTime.Now
                });
                var songsInAlbum = album.Songs.Count();
                var song = _db.Songs.Insert(new Song()
                {
                    Title = "Song " + Guid.NewGuid()
                });
                var songContainers = song.Albums.Count();
                song.Albums.Add(album);
                _db.SaveChanges();

                Assert.AreEqual(album.Songs.Count(), songsInAlbum + 1);
                Assert.AreEqual(song.Albums.Count(), songContainers + 1);
            }
        }

        [TestMethod]
        [TestCategory("CRUD_Albums_Songs")]
        public void DeleteAlbum()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var album = _db.Albums.Content.OrderByDescending(a => a.Id).First();
                var albumsCount = _db.Albums.Content.Count();

                _db.Albums.Delete(album);
                _db.SaveChanges();

                Assert.AreEqual(_db.Albums.Content.Count(), albumsCount - 1);
                Assert.AreEqual(
                    _db.Songs.Content.Where(
                         s => s.Albums.Where(a => a.Id == album.Id).Count() > 0
                    ).Count(),
                    0
                );
            }
        }

        [TestMethod]
        [TestCategory("CRUD_Albums_Songs")]
        public void ResetAlbumSongs()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var album = _db.Albums.Insert(new Album()
                {
                    Title = "Album " + Guid.NewGuid(),
                    Published = DateTime.Now
                });
                var song1 = _db.Songs.Insert(new Song()
                {
                    Title = "Song " + Guid.NewGuid()
                });
                album.Songs.Add(song1);
                _db.SaveChanges();

                Assert.AreEqual(song1.Albums.Count(), 1);

                var song2 = _db.Songs.Insert(new Song()
                {
                    Title = "Song " + Guid.NewGuid()
                });

                album.Songs.Clear();
                album.Songs.Add(song2);
                _db.SaveChanges();

                Assert.AreEqual(song1.Albums.Count(), 0);
                Assert.AreEqual(song2.Albums.Count(), 1);
            }
        }

        [TestMethod]
        [TestCategory("CRUD_Albums_Songs")]
        public void ResetSongAlbums()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var song = _db.Songs.Insert(new Song()
                {
                    Title = "Song " + Guid.NewGuid()
                });
                var album1 = _db.Albums.Insert(new Album()
                {
                    Title = "Album " + Guid.NewGuid(),
                    Published = DateTime.Now
                });
                song.Albums.Add(album1);
                _db.SaveChanges();

                Assert.AreEqual(album1.Songs.Count(), 1);

                var album2 = _db.Albums.Insert(new Album()
                {
                    Title = "Album " + Guid.NewGuid(),
                    Published = DateTime.Now
                });

                song.Albums.Clear();
                song.Albums.Add(album2);
                _db.SaveChanges();

                Assert.AreEqual(album1.Songs.Count(), 0);
                Assert.AreEqual(album2.Songs.Count(), 1);
            }
        }
    }
}
