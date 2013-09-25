using System;
using System.Linq;
using BandSite.Models.DataLayer;
using BandSite.Models.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BandSiteTests
{
    [TestClass]
    public class DataLayerTests
    {
        private readonly IDbContextFactory _db = //new FakeDbContextFactory();
                                                 new DbContextEfFactory("Test");

        #region Song Repository

        [TestMethod]
        public void Song_Add()
        {
            using (var context = _db.CreateContext())
            {
                var song = new Song
                {
                    Title = "Song" + Guid.NewGuid(),
                    Text = "rejgpo rtsituh sptjhstoihs ijsiothp sitjhs",
                    File = new byte[1000]
                };
                var oldCount = context.Songs.Content.Count();

                context.Songs.Insert(song);
                context.SaveChanges();

                Assert.AreEqual(oldCount + 1, context.Songs.Content.Count());
            }
        }

        [TestMethod]
        public void Song_Add_Null()
        {
            using (var context = _db.CreateContext())
            {
                var result = context.Songs.Insert(null);
                Assert.AreEqual(result, null);
            }
        }

       [TestMethod]
        public void Song_Update()
        {
            using (var context = _db.CreateContext())
            {
                var song = context.Songs.Content.First();
                var update = new Song
                {
                    Title = "Song" + Guid.NewGuid(),
                    Text = "xxxxxxxxxxxxxxx"
                };

                context.Songs.Update(song.Id, update);
                context.SaveChanges();

                song = context.Songs.Content.First();
                Assert.AreEqual(song.Title, update.Title);
                Assert.AreEqual(song.Text, update.Text);
            }
        }

        [TestMethod]
       public void Song_Update_Unexisting_Item()
        {
            using (var context = _db.CreateContext())
            {
                var update = new Song
                {
                    Title = "Song" + Guid.NewGuid(),
                    Text = "xxxxxxxxxxxxxxx"
                };

                var result = context.Songs.Update(0, update);

                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void Song_Update_With_Null()
        {
            using (var context = _db.CreateContext())
            {
                var song = context.Songs.Content.First();
                var result = context.Songs.Update(song.Id, null);
                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void Song_Delete_Null()
        {
            using (var context = _db.CreateContext())
            {
                var result = context.Songs.Delete(null);
                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void Song_Delete()
        {
            using (var context = _db.CreateContext())
            {
                var song = context.Songs.Content.First();
                var oldCount = context.Songs.Content.Count();

                context.Songs.Delete(song);
                context.SaveChanges();

                Assert.AreEqual(oldCount - 1, context.Songs.Content.Count());
            }
        }
        
        #endregion

        #region Album Repository

        [TestMethod]
        public void Album_Add()
        {
            using (var context = _db.CreateContext())
            {
                var album = new Album
                {
                    Title = "Album" + Guid.NewGuid(),
                    Description = "rejgpo rtsituh sptjhstoihs ijsiothp sitjhs",
                    Published = DateTime.Now
                };
                var oldCount = context.Albums.Content.Count();

                context.Albums.Insert(album);
                context.SaveChanges();

                Assert.AreEqual(oldCount + 1, context.Albums.Content.Count());
            }
        }

        [TestMethod]
        public void Album_Add_Null()
        {
            using (var context = _db.CreateContext())
            {
                var result = context.Albums.Insert(null);
                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void Album_Update()
        {
            using (var context = _db.CreateContext())
            {
                var album = context.Albums.Content.First();
                var update = new Album
                {
                    Title = "Album" + Guid.NewGuid(),
                    Description = "xxxxxxxxxxxxxxx"
                };

                context.Albums.Update(album.Id, update);
                context.SaveChanges();

                album = context.Albums.Content.First();
                Assert.AreEqual(album.Title, update.Title);
                Assert.AreEqual(album.Description, update.Description);
            }
        }

        [TestMethod]
        public void Album_Update_Unexisting_Item()
        {
            using (var context = _db.CreateContext())
            {
                var update = new Album
                {
                    Title = "Album" + Guid.NewGuid(),
                    Description = "xxxxxxxxxxxxxxx"
                };

                var result = context.Albums.Update(0, update);

                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void Album_Update_With_Null()
        {
            using (var context = _db.CreateContext())
            {
                var album = context.Albums.Content.First();
                var result = context.Albums.Update(album.Id, null);
                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void Album_Delete_Null()
        {
            using (var context = _db.CreateContext())
            {
                var result = context.Albums.Delete(null);
                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void Album_Delete()
        {
            using (var context = _db.CreateContext())
            {
                var album = context.Albums.Content.First();
                var oldCount = context.Albums.Content.Count();

                context.Albums.Delete(album);
                context.SaveChanges();

                Assert.AreEqual(oldCount - 1, context.Albums.Content.Count());
            }
        }

        #endregion

        #region UserProfile Repository

        [TestMethod]
        public void UserProfile_Add()
        {
            using (var context = _db.CreateContext())
            {
                var userProfile = new UserProfile
                {
                    UserName = "UserProfile" + Guid.NewGuid(),
                };
                var oldCount = context.UserProfiles.Content.Count();

                context.UserProfiles.Insert(userProfile);
                context.SaveChanges();

                Assert.AreEqual(oldCount + 1, context.UserProfiles.Content.Count());
            }
        }

        [TestMethod]
        public void UserProfile_Add_Null()
        {
            using (var context = _db.CreateContext())
            {
                var result = context.UserProfiles.Insert(null);
                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void UserProfile_Update()
        {
            using (var context = _db.CreateContext())
            {
                var userProfile = context.UserProfiles.Content.First();
                var update = new UserProfile
                {
                    UserName = "UserProfile" + Guid.NewGuid()
                };

                context.UserProfiles.Update(userProfile.Id, update);
                context.SaveChanges();

                userProfile = context.UserProfiles.Content.First();
                Assert.AreEqual(userProfile.UserName, update.UserName);
            }
        }

        [TestMethod]
        public void UserProfile_Update_Unexisting_Item()
        {
            using (var context = _db.CreateContext())
            {
                var update = new UserProfile
                {
                    UserName = "UserProfile" + Guid.NewGuid()
                };

                var result = context.UserProfiles.Update(0, update);

                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void UserProfile_Update_With_Null()
        {
            using (var context = _db.CreateContext())
            {
                var userProfile = context.UserProfiles.Content.First();
                var result = context.UserProfiles.Update(userProfile.Id, null);
                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void UserProfile_Delete()
        {
            using (var context = _db.CreateContext())
            {
                var userProfile = context.UserProfiles.Content.First();
                foreach (var msg in context.Messages.Content.Where(m => (m.UserFromId == userProfile.Id) || (m.UserToId == userProfile.Id)))
                {
                    context.Messages.Delete(msg);
                }
                var oldCount = context.UserProfiles.Content.Count();

                context.UserProfiles.Delete(userProfile);
                context.SaveChanges();

                Assert.AreEqual(oldCount - 1, context.UserProfiles.Content.Count());
            }
        }

        [TestMethod]
        public void UserProfile_Delete_Null()
        {
            using (var context = _db.CreateContext())
            {
                var result = context.UserProfiles.Delete(null);
                Assert.AreEqual(result, null);
            }
        }

        #endregion

        #region Message Repository

        [TestMethod]
        public void Message_Add()
        {
            using (var context = _db.CreateContext())
            {
                var userFrom = context.UserProfiles.Content.OrderBy(u => u.Id).First();
                var userTo = context.UserProfiles.Content.OrderBy(u => u.Id).Skip(1).First();
                var message = new Message
                {
                    Text = "rejgpo rtsituh sptjhstoihs ijsiothp sitjhs",
                    Published = DateTime.Now,
                    UserFrom = userFrom,
                    UserTo = userTo,
                    Status = MessageStatus.Unread.ToString()
                };
                var oldCount = context.Messages.Content.Count();

                context.Messages.Insert(message);
                context.SaveChanges();

                Assert.AreEqual(oldCount + 1, context.Messages.Content.Count());
            }
        }

        [TestMethod]
        public void Message_Add_Null()
        {
            using (var context = _db.CreateContext())
            {
                var result = context.Messages.Insert(null);
                Assert.AreEqual(result, null);
            }
        }

       [TestMethod]
        public void Message_Update()
        {
            using (var context = _db.CreateContext())
            {
                var message = context.Messages.Content.First();
                var update = new Message
                {
                    Text = "xxxxxxxxxxxxxxx",
                    Status = MessageStatus.Read.ToString()
                };

                context.Messages.Update(message.Id, update);
                context.SaveChanges();

                message = context.Messages.Content.First();
                Assert.AreEqual(message.Text, update.Text);
                Assert.AreEqual(message.Status, update.Status);
            }
        }

        [TestMethod]
       public void Message_Update_Unexisting_Item()
        {
            using (var context = _db.CreateContext())
            {
                var update = new Message
                {
                    Text = "xxxxxxxxxxxxxxx"
                };

                var result = context.Messages.Update(0, update);

                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void Message_Update_With_Null()
        {
            using (var context = _db.CreateContext())
            {
                var message = context.Messages.Content.First();
                var result = context.Messages.Update(message.Id, null);
                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void Message_Delete()
        {
            using (var context = _db.CreateContext())
            {
                var message = context.Messages.Content.First();
                var oldCount = context.Messages.Content.Count();

                context.Messages.Delete(message);
                context.SaveChanges();

                Assert.AreEqual(oldCount - 1, context.Messages.Content.Count());
            }
        }

        [TestMethod]
        public void Message_Delete_Null()
        {
            using (var context = _db.CreateContext())
            {
                var result = context.Messages.Delete(null);
                Assert.AreEqual(result, null);
            }
        }

        #endregion

        #region PlaylistItem Repository

        [TestMethod]
        public void PlaylistItem_Add()
        {
            using (var context = _db.CreateContext())
            {
                var user = context.UserProfiles.Content.OrderBy(u => u.Id).First();
                var song = context.Songs.Content.First();
                var playlistItem = new PlaylistItem
                {
                    Order = 1,
                    User = user,
                    Song = song
                };
                var oldCount = context.PlaylistItems.Content.Count();

                context.PlaylistItems.Insert(playlistItem);
                context.SaveChanges();

                Assert.AreEqual(oldCount + 1, context.PlaylistItems.Content.Count());
            }
        }

        [TestMethod]
        public void PlaylistItem_Add_Null()
        {
            using (var context = _db.CreateContext())
            {
                var result = context.PlaylistItems.Insert(null);
                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void PlaylistItem_Update()
        {
            using (var context = _db.CreateContext())
            {
                var playlistItem = context.PlaylistItems.Content.First();
                var update = new PlaylistItem
                {
                    Order = 2
                };

                context.PlaylistItems.Update(playlistItem.Id, update);
                context.SaveChanges();

                playlistItem = context.PlaylistItems.Content.First();
                Assert.AreEqual(playlistItem.Order, update.Order);
            }
        }

        [TestMethod]
        public void PlaylistItem_Update_Unexisting_Item()
        {
            using (var context = _db.CreateContext())
            {
                var update = new PlaylistItem
                {
                    Order = 2
                };

                var result = context.PlaylistItems.Update(0, update);

                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void PlaylistItem_Update_With_Null()
        {
            using (var context = _db.CreateContext())
            {
                var playlistItem = context.PlaylistItems.Content.First();
                var result = context.PlaylistItems.Update(playlistItem.Id, null);
                Assert.AreEqual(result, null);
            }
        }

        [TestMethod]
        public void PlaylistItem_Delete()
        {
            using (var context = _db.CreateContext())
            {
                var playlistItem = context.PlaylistItems.Content.First();
                var oldCount = context.PlaylistItems.Content.Count();

                context.PlaylistItems.Delete(playlistItem);
                context.SaveChanges();

                Assert.AreEqual(oldCount - 1, context.PlaylistItems.Content.Count());
            }
        }

        [TestMethod]
        public void PlaylistItem_Delete_Null()
        {
            using (var context = _db.CreateContext())
            {
                var result = context.PlaylistItems.Delete(null);
                Assert.AreEqual(result, null);
            }
        }

        #endregion
    }
}
