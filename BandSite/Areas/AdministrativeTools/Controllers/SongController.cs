using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BandSite.Models;
using BandSite.Models.Implementations;
using BandSite.Models.Interfaces;

namespace BandSite.Areas.AdministrativeTools.Controllers
{
    [Authorize]
    public class SongController : Controller
    {
        private IDbContext _db = MvcApplication.DbFactory.CreateContext();

        //
        // GET: /AdministrativeTools/Song/

        public ActionResult Index()
        {
            return PartialView(_db.Songs.Content.ToList());
        }

        //
        // GET: /AdministrativeTools/Song/Details/5

        public ActionResult Details(int id = 0)
        {
            Song song = _db.Songs.Content.Where(s => s.Id == id).FirstOrDefault();
            if (song == null)
            {
                return HttpNotFound();
            }
            return PartialView(song);
        }

        //
        // GET: /AdministrativeTools/Song/Create

        public ActionResult Create()
        {
            return PartialView();
        }

        //
        // POST: /AdministrativeTools/Song/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Song song)
        {
            if (ModelState.IsValid)
            {
                _db.Songs.Insert(song);
                _db.SaveChanges();
                return Json(new { hash = "action=index&entity=song" });
            }

            return PartialView(song);
        }

        //
        // GET: /AdministrativeTools/Song/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Song song = _db.Songs.Content.Where(s => s.Id == id).FirstOrDefault();
            if (song == null)
            {
                return HttpNotFound();
            }
            return PartialView(song);
        }

        //
        // POST: /AdministrativeTools/Song/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Song song)
        {
            if (ModelState.IsValid)
            {
                _db.Songs.Update(song);
                _db.SaveChanges();
                return Json(new { hash = "action=index&entity=song" });
            }
            return PartialView(song);
        }

        //
        // GET: /AdministrativeTools/Song/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Song song = _db.Songs.Content.Where(s => s.Id == id).FirstOrDefault();
            if (song == null)
            {
                return HttpNotFound();
            }
            return PartialView(song);
        }

        //
        // POST: /AdministrativeTools/Song/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Song song = _db.Songs.Content.Where(s => s.Id == id).FirstOrDefault();
            _db.Songs.Delete(song);
            _db.SaveChanges();
            return Json(new { hash = "action=index&entity=song" });
        }

        public ActionResult SongsSearch(string term)
        {
            var songs = _db.Songs.Content.Where(s => s.Title.Contains(term)).Select(s => new { label = s.Title, value = s.Id});
            return Json(songs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddAlbum(int songId)
        {
            Song song = _db.Songs.Content.Where(s => s.Id == songId).FirstOrDefault();
            if (song == null)
            {
                return HttpNotFound();
            }
            AddAlbumModel model = new AddAlbumModel() { SongId = song.Id };
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAlbum(AddAlbumModel model)
        {
            Album album = _db.Albums.Content.Where(a => a.Id == model.AlbumId).FirstOrDefault();
            Song song = _db.Songs.Content.Where(s => s.Id == model.SongId).FirstOrDefault();
            if ((album == null) || (song == null))
            {
                return HttpNotFound();
            }
            song.Albums.Add(album);
            int added = _db.SaveChanges();
            return Json(new { hash = "action=edit&entity=song&id=" + song.Id + "&relatedentity=album&added=" + added + "&loader=0" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAlbum(DeleteAlbumModel model)
        {
            Album album = _db.Albums.Content.Where(a => a.Id == model.AlbumId).FirstOrDefault();
            Song song = _db.Songs.Content.Where(s => s.Id == model.SongId).FirstOrDefault();
            if ((album == null) || (song == null))
            {
                return HttpNotFound();
            }
            song.Albums.Remove(album);
            var deleted = _db.SaveChanges();
            return Json(new { hash = "action=edit&entity=song&id=" + song.Id + "&relatedentity=album&deleted=" + deleted + "&loader=0" });
        }

        public ActionResult ShowAlbum(int songId)
        {
            ViewBag.SongId = songId;
            return PartialView(_db.Albums.Content.Where(a => a.Songs.Where(s => s.Id == songId).Count() == 1).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}