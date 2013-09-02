using BandSite.Models.DataLayer;
using BandSite.Models.Entities;
using BandSite.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace BandSite.Areas.AdministrativeTools.Controllers
{
    public class AlbumController : Controller
    {
        public AlbumController(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        private IDbContextFactory _dbContextFactory;

        //
        // GET: /AdministrativeTools/Album/

        public ActionResult Index()
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                return PartialView(db.Albums.Content.ToList());
            }
        }

        //
        // GET: /AdministrativeTools/Album/Details/5

        public ActionResult Details(int id = 0)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                CRUDAlbumModel album = db.Albums.Content.Where(a => a.Id == id).Select(a => new CRUDAlbumModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Published = a.Published,
                    Description = a.Description
                }).FirstOrDefault();
                if (album == null)
                {
                    return HttpNotFound();
                }
                return PartialView(album);
            }
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
        public ActionResult Create(CRUDAlbumModel album)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                if (ModelState.IsValid)
                {
                    var newAlbum = new Album();
                    if (newAlbum.TrySetPropertiesFrom(album))
                    {
                        db.Albums.Insert(newAlbum);
                        db.SaveChanges();
                        return Json(new { hash = "action=index&entity=album" });
                    }
                }
                return PartialView(album);
            }
        }

        //
        // GET: /AdministrativeTools/Album/Edit/5

        public ActionResult Edit(int id = 0)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                CRUDAlbumModel album = db.Albums.Content.Where(a => a.Id == id).Select(a => new CRUDAlbumModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Published = a.Published,
                    Description = a.Description
                }).FirstOrDefault();
                if (album == null)
                {
                    return HttpNotFound();
                }
                return PartialView(album);
            }
        }

        //
        // POST: /AdministrativeTools/Album/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CRUDAlbumModel album)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                if (ModelState.IsValid)
                {
                    var updatedAlbum = new Album();
                    if (updatedAlbum.TrySetPropertiesFrom(album))
                    {
                        db.Albums.Update(album.Id, updatedAlbum);
                        db.SaveChanges();
                        return Json(new { hash = "action=index&entity=album" });
                    }
                }
                return PartialView(album);
            }
        }

        //
        // GET: /AdministrativeTools/Album/Delete/5

        public ActionResult Delete(int id = 0)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                CRUDAlbumModel album = db.Albums.Content.Where(a => a.Id == id).Select(a => new CRUDAlbumModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Published = a.Published,
                    Description = a.Description
                }).FirstOrDefault();
                if (album == null)
                {
                    return HttpNotFound();
                }
                return PartialView(album);
            }
        }

        //
        // POST: /AdministrativeTools/Album/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Album album = db.Albums.Content.FirstOrDefault(a => a.Id == id);
                db.Albums.Delete(album);
                db.SaveChanges();
                return Json(new { hash = "action=index&entity=album" });
            }
        }

        public ActionResult AlbumsSearch(string term)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var albums = db.Albums.Content.Where(a => a.Title.Contains(term)).Select(a => new { label = a.Title, value = a.Id }).ToList();
                return Json(albums, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddSong(int albumId)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Album album = db.Albums.Content.FirstOrDefault(a => a.Id == albumId);
                if (album == null)
                {
                    return HttpNotFound();
                }
                var model = new SongAlbumRelationModel { AlbumId = album.Id };
                return PartialView(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSong(SongAlbumRelationModel model)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Album album = db.Albums.Content.FirstOrDefault(a => a.Id == model.AlbumId);
                Song song = db.Songs.Content.FirstOrDefault(s => s.Id == model.SongId);
                if ((album == null) || (song == null))
                {
                    return HttpNotFound();
                }
                album.Songs.Add(song);
                int added = db.SaveChanges();
                return Json(new { hash = "action=edit&entity=album&id=" + album.Id + "&relatedentity=song&added=" + added + "&loader=0" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSong(SongAlbumRelationModel model)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Album album = db.Albums.Content.FirstOrDefault(a => a.Id == model.AlbumId);
                Song song = db.Songs.Content.FirstOrDefault(s => s.Id == model.SongId);
                if ((album == null) || (song == null))
                {
                    return HttpNotFound();
                }
                album.Songs.Remove(song);
                int deleted = db.SaveChanges();
                return Json(new { hash = "action=edit&entity=album&id=" + album.Id + "&relatedentity=song&deleted=" + deleted + "&loader=0" });
            }
        }

        public ActionResult ShowSong(int albumId)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                ViewBag.AlbumId = albumId;
                return PartialView(db.Songs.Content.Where(s => s.Albums.Count(a => a.Id == albumId) == 1).ToList());
            }
        }
    }
}