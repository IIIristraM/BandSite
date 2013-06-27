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
        public void Test_1_InsertAlbum()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var albumsCount = _db.Albums.Content.Count();

                var album = _db.Albums.Insert(new Album()
                {
                    Title = "Album " + Guid.NewGuid(),
                    Published = DateTime.Now,
                    Songs = new HashSet<Song>()
                });
                _db.SaveChanges();

                Assert.AreEqual(_db.Albums.Content.Count(), albumsCount + 1);
                Assert.IsNotNull(album.Songs);
            }
        }

        [TestMethod]
        public void Test_2_InsertSong()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var songsCount = _db.Songs.Content.Count();

                var song = _db.Songs.Insert(new Song()
                {
                    Title = "Song " + Guid.NewGuid(),
                    Albums = new HashSet<Album>()
                });
                _db.SaveChanges();

                Assert.AreEqual(_db.Songs.Content.Count(), songsCount + 1);
                Assert.IsNotNull(song.Albums);
            }
        }

        [TestMethod]
        public void Test_3_AddSongToAlbum()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var album = _db.Albums.Insert(new Album()
                {
                    Title = "Album " + Guid.NewGuid(),
                    Published = DateTime.Now,
                    Songs = new HashSet<Song>()
                });
                var songsInAlbum = album.Songs.Count;
                var song = _db.Songs.Insert(new Song()
                {
                    Title = "Song " + Guid.NewGuid(),
                    Albums = new HashSet<Album>()
                });
                var songContainers = song.Albums.Count;
                album.Songs.Add(song);
                _db.SaveChanges();

                Assert.AreEqual(album.Songs.Count, songsInAlbum + 1);
                Assert.AreEqual(song.Albums.Count, songContainers + 1);
            }
        }

        [TestMethod]
        public void Test_4_AddAlbumToSong()
        {
            using (_db = _dbfactory.CreateContext())
            {
                var album = _db.Albums.Insert(new Album()
                {
                    Title = "Album " + Guid.NewGuid(),
                    Published = DateTime.Now,
                    Songs = new HashSet<Song>()
                });
                var songsInAlbum = album.Songs.Count;
                var song = _db.Songs.Insert(new Song()
                {
                    Title = "Song " + Guid.NewGuid(),
                    Albums = new HashSet<Album>()
                });
                var songContainers = song.Albums.Count;
                song.Albums.Add(album);
                _db.SaveChanges();

                Assert.AreEqual(album.Songs.Count, songsInAlbum + 1);
                Assert.AreEqual(song.Albums.Count, songContainers + 1);
            }
        }
    }
}
