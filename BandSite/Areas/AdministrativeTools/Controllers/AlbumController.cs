using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BandSite.Models.Implementations;
using BandSite.Models.Interfaces;
using BandSite.Models;

namespace BandSite.Areas.AdministrativeTools.Controllers
{
    public class AlbumController : Controller
    {
        private IDbContext db = MvcApplication.DbFactory.CreateContext();

        //
        // GET: /AdministrativeTools/Album/

        public ActionResult Index()
        {
            return PartialView(db.Albums.Content.ToList());
        }

        //
        // GET: /AdministrativeTools/Album/Details/5

        public ActionResult Details(int id = 0)
        {
            Album album = db.Albums.Content.Where(a => a.Id == id).FirstOrDefault();
            if (album == null)
            {
                return HttpNotFound();
            }
            return PartialView(album);
        }

        //
        // GET: /AdministrativeTools/Album/Create

        public ActionResult Create()
        {
            return PartialView();
        }

        //
        // POST: /AdministrativeTools/Album/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Album album)
        {
            if (ModelState.IsValid)
            {
                db.Albums.Insert(album);
                db.SaveChanges();
                return Json(new { hash = "action=index&entity=album"});
            }
            return PartialView(album);
        }

        //
        // GET: /AdministrativeTools/Album/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Album album = db.Albums.Content.Where(a => a.Id == id).FirstOrDefault();
            if (album == null)
            {
                return HttpNotFound();
            }
            return PartialView(album);
        }

        //
        // POST: /AdministrativeTools/Album/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Album album)
        {
            if (ModelState.IsValid)
            {
                db.Albums.Update(album);
                db.SaveChanges();
                return Json(new { hash = "action=index&entity=album" });
            }
            return PartialView(album);
        }

        //
        // GET: /AdministrativeTools/Album/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Album album = db.Albums.Content.Where(a => a.Id == id).FirstOrDefault();
            if (album == null)
            {
                return HttpNotFound();
            }
            return PartialView(album);
        }

        //
        // POST: /AdministrativeTools/Album/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Content.Where(a => a.Id == id).FirstOrDefault();
            db.Albums.Delete(album);
            db.SaveChanges();
            return Json(new { hash = "action=index&entity=album" });
        }

        public ActionResult AlbumsSearch(string term)
        {
            var albums = db.Albums.Content.Where(a => a.Title.Contains(term)).Select(a => new { label = a.Title, value = a.Id });
            return Json(albums, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddSong(int albumId)
        {
            Album album = db.Albums.Content.Where(a => a.Id == albumId).FirstOrDefault();
            if (album == null)
            {
                return HttpNotFound();
            }
            AddSongModel model = new AddSongModel() { AlbumId = album.Id };
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSong(AddSongModel model)
        {
            Album album = db.Albums.Content.Where(a => a.Id == model.AlbumId).FirstOrDefault();
            Song song = db.Songs.Content.Where(s => s.Id == model.SongId).FirstOrDefault();
            if ((album == null) || (song == null))
            {
                return HttpNotFound();
            }
            album.Songs.Add(song);
            int added = db.SaveChanges();
            return Json(new { hash = "action=edit&entity=album&id=" + album.Id + "&relatedentity=song&added=" + added + "&loader=0" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSong(DeleteSongModel model)
        {
            Album album = db.Albums.Content.Where(a => a.Id == model.AlbumId).FirstOrDefault();
            Song song = db.Songs.Content.Where(s => s.Id == model.SongId).FirstOrDefault();
            if ((album == null) || (song == null))
            {
                return HttpNotFound();
            }
            album.Songs.Remove(song);
            int deleted = db.SaveChanges();
            return Json(new { hash = "action=edit&entity=album&id=" + album.Id + "&relatedentity=song&deleted=" + deleted + "&loader=0" });
        }

        public ActionResult ShowSong(int albumId)
        {
            ViewBag.AlbumId = albumId;
            return PartialView(db.Songs.Content.Where(s => s.Albums.Where(a => a.Id == albumId).Count() == 1).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}