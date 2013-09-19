using System.Web.Mvc;
using BandSite.Models.DataLayer;
using BandSite.Models.Entities;
using BandSite.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Moq;
using BandSite.Controllers;

namespace BandSiteTests
{
    [TestClass]
    public class SongControllerTests
    {
        private readonly IDbContextFactory _db = new FakeDbContextFactory();
                                                 //new DbContextEfFactory("BandSiteDB-Test");
        [TestMethod]
        public void MethodIndex()
        {
            var controller = new SongController(_db);
            var result = controller.Index();
            Assert.AreEqual(result.GetType(), typeof(PartialViewResult));
            Assert.AreNotEqual(((PartialViewResult)result).Model as IEnumerable<Song>, null);
        }

        [TestMethod]
        public void MethodDetails()
        {
            using (var db = _db.CreateContext())
            {
                var controller = new SongController(_db);
                var song = db.Songs.Content.First();

                var result = controller.Details(song.Id);

                Assert.AreEqual(result.GetType(), typeof(PartialViewResult));
                Assert.AreNotEqual(((PartialViewResult)result).Model as CRUDSongModel, null);
                Assert.AreEqual((((PartialViewResult)result).Model as CRUDSongModel).Id, song.Id);

                result = controller.Details(0);
                Assert.AreEqual(result.GetType(), typeof(HttpNotFoundResult));
            }
        }

        [TestMethod]
        public void MethodCreatePOST()
        {
            var mockContext = new Mock<ControllerContext>();
            mockContext.Setup(c => c.HttpContext.Request.HttpMethod).Returns("POST");
            var controller = new SongController(_db);
            controller.ControllerContext = mockContext.Object;
            var count = _db.CreateContext().Songs.Content.Count();

            var result = controller.Create(new CRUDSongModel
            {
                Title = "Song over controller"
            });

            Assert.AreEqual(result.GetType(), typeof(JsonResult));
            var json = (result as JsonResult).Data;
            Assert.AreEqual(json.GetType().GetProperty("hash").GetValue(json), "#song/index");
            Assert.AreEqual(count + 1, _db.CreateContext().Songs.Content.Count());
        }

        [TestMethod]
        public void MethodEditPOST()
        {
            using (var db = _db.CreateContext())
            {
                var mockContext = new Mock<ControllerContext>();
                mockContext.Setup(c => c.HttpContext.Request.HttpMethod).Returns("POST");
                var controller = new SongController(_db);
                controller.ControllerContext = mockContext.Object;
                var song = db.Songs.Content.First();

                var result = controller.Edit(new CRUDSongModel
                {
                    Id = song.Id,
                    Title = "Song updated over controller"
                });

                Assert.AreEqual(result.GetType(), typeof (JsonResult));
                var json = (result as JsonResult).Data;
                Assert.AreEqual(json.GetType().GetProperty("hash").GetValue(json), "#song/index");
                Assert.AreEqual(db.Songs.Content.First().Title, "Song updated over controller");
            }
        }

        [TestMethod]
        public void MethodDeletePOST()
        {
            using (var db = _db.CreateContext())
            {
                var mockContext = new Mock<ControllerContext>();
                mockContext.Setup(c => c.HttpContext.Request.HttpMethod).Returns("POST");
                var controller = new SongController(_db);
                controller.ControllerContext = mockContext.Object;
                var count = _db.CreateContext().Songs.Content.Count();
                var song = db.Songs.Content.First();

                var result = controller.DeleteConfirmed(song.Id);

                Assert.AreEqual(result.GetType(), typeof (JsonResult));
                var json = (result as JsonResult).Data;
                Assert.AreEqual(json.GetType().GetProperty("hash").GetValue(json), "#song/index");
                Assert.AreEqual(count - 1, _db.CreateContext().Songs.Content.Count());
            }
        }
    }
}
