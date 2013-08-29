using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using BandSite.Models.DataLayer;
using BandSite.Models.Entities;
using BandSite.Models.ViewModels;
using BandSite.Models.Functionality;

namespace BandSite.Areas.AdministrativeTools.Controllers
{
    [Authorize]
    public class SongController : Controller
    {
        private IDbContextFactory _dbContextFactory;

        public SongController() {}

        public SongController(IDbContextFactory dbContextFactory)
            : this()
        {
            _dbContextFactory = dbContextFactory;
        }

        //
        // GET: /AdministrativeTools/Song/

        public ActionResult Index()
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                return PartialView(db.Songs.Content.ToList());
            }
        }

        //
        // GET: /AdministrativeTools/Song/Details/5

        public ActionResult Details(int id = 0)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Song song = db.Songs.Content.FirstOrDefault(s => s.Id == id);
                if (song == null)
                {
                    return HttpNotFound();
                }
                return PartialView(new CRUDSongModel
                {
                    Id = song.Id,
                    Title = song.Title,
                    Text = song.Text
                });
            }
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
        public ActionResult Create(CRUDSongModel song)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                if (ModelState.IsValid)
                {
                    var newSong = new Song {Title = song.Title, Text = song.Text};
                    if (song.UploadFile != null)
                    {
                        newSong.File = new byte[song.UploadFile.InputStream.Length];
                        var uploader = new Uploader();
                        uploader.Upload(newSong.File, song.UploadFile.InputStream, User.Identity.Name);
                    }
                    db.Songs.Insert(newSong);
                    db.SaveChanges();
                    return Json(new { hash = "action=index&entity=song" });
                }

                return PartialView();
            }
        }

        //
        // GET: /AdministrativeTools/Song/Edit/5

        public ActionResult Edit(int id = 0)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                CRUDSongModel song = db.Songs.Content
                                              .Where(s => s.Id == id)
                                              .Select(s => new CRUDSongModel
                                              {
                                                  Id = s.Id,
                                                  Title = s.Title,
                                                  Text = s.Text,
                                              }).FirstOrDefault();
                if (song == null)
                {
                    return HttpNotFound();
                }
                return PartialView(song);
            }
        }

        //
        // POST: /AdministrativeTools/Song/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CRUDSongModel song)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                if (ModelState.IsValid)
                {
                    var updatedSong = new Song();
                    if (updatedSong.TrySetPropertiesFrom(song))
                    {
                        if (song.UploadFile != null)
                        {
                            updatedSong.File = new byte[song.UploadFile.InputStream.Length];
                            var uploader = new Uploader();
                            uploader.Upload(updatedSong.File, song.UploadFile.InputStream, User.Identity.Name);
                        }
                        db.Songs.Update(updatedSong.Id, updatedSong);
                        db.SaveChanges();
                        return Json(new { hash = "action=index&entity=song" });
                    }
                }
                return PartialView(song);
            }
        }

        //
        // GET: /AdministrativeTools/Song/Delete/5

        public ActionResult Delete(int id = 0)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Song song = db.Songs.Content.FirstOrDefault(s => s.Id == id);
                if (song == null)
                {
                    return HttpNotFound();
                }
                return PartialView(new CRUDSongModel
                {
                    Id = song.Id,
                    Title = song.Title,
                    Text = song.Text
                });
            }
        }

        //
        // POST: /AdministrativeTools/Song/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Song song = db.Songs.Content.FirstOrDefault(s => s.Id == id);
                db.Songs.Delete(song);
                db.SaveChanges();
                return Json(new { hash = "action=index&entity=song" });
            }
        }

        public ActionResult SongsSearch(string term)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var songs = db.Songs.Content.Where(s => s.Title.Contains(term)).Select(s => new { label = s.Title, value = s.Id }).ToList();
                return Json(songs, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddAlbum(int songId)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Song song = db.Songs.Content.FirstOrDefault(s => s.Id == songId);
                if (song == null)
                {
                    return HttpNotFound();
                }
                var model = new SongAlbumRelationModel { SongId = song.Id };
                return PartialView(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAlbum(SongAlbumRelationModel model)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Album album = db.Albums.Content.FirstOrDefault(a => a.Id == model.AlbumId);
                Song song = db.Songs.Content.FirstOrDefault(s => s.Id == model.SongId);
                if ((album == null) || (song == null))
                {
                    return HttpNotFound();
                }
                song.Albums.Add(album);
                int added = db.SaveChanges();
                return Json(new { hash = "action=edit&entity=song&id=" + song.Id + "&relatedentity=album&added=" + added + "&loader=0" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAlbum(SongAlbumRelationModel model)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Album album = db.Albums.Content.FirstOrDefault(a => a.Id == model.AlbumId);
                Song song = db.Songs.Content.FirstOrDefault(s => s.Id == model.SongId);
                if ((album == null) || (song == null))
                {
                    return HttpNotFound();
                }
                song.Albums.Remove(album);
                var deleted = db.SaveChanges();
                return Json(new { hash = "action=edit&entity=song&id=" + song.Id + "&relatedentity=album&deleted=" + deleted + "&loader=0" });
            }
        }

        public ActionResult ShowAlbum(int songId)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                ViewBag.SongId = songId;
                return PartialView(db.Albums.Content.Where(a => a.Songs.Count(s => s.Id == songId) == 1).ToList());
            }
        }

        public FileStreamResult GetStream(int id)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                Response.Headers.Add("Accept-Ranges", "bytes");
                Response.Cache.SetCacheability(HttpCacheability.Public);
                Response.Cache.SetMaxAge(new TimeSpan(0, 10, 0));
                var query = db.Songs.Content.Where(s => s.Id == id).Select(s => s.File);
                var song = query.FirstOrDefault();
                if (song != null)
                {
#if DEBUG
                    var scsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["BandSiteDB-Debug"].ConnectionString);
#else
                    SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["BandSiteDB"].ConnectionString);
#endif
                    var conn = new SqlConnection(scsb.ConnectionString);
                    conn.Open();
                    var cmd = new SqlCommand(query.ToString(), conn);
                    cmd.Parameters.Add(new SqlParameter("p__linq__0", id));
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess |
                                                              CommandBehavior.SingleResult |
                                                              CommandBehavior.SingleRow |
                                                              CommandBehavior.CloseConnection);
                    if (reader.Read())
                    {
                        Stream content = new SqlReaderStream(reader, 0);
                        return File(content, "audio/mp3");
                    }
                    return null;
                }
                return null;
            }
        }

        public ActionResult PopulateUsersPlaylist(int userId, int songId)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                UserProfile user = userId == -1 ? db.UserProfiles.Content.FirstOrDefault(u => u.UserName == User.Identity.Name) : db.UserProfiles.Content.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    var song = db.Songs.Content.FirstOrDefault(s => s.Id == songId);
                    if ((song != null) && (user.Playlists.All(p => p.SongId != songId)))
                    {
                        try
                        {
                            db.PlaylistItems.Insert(new PlaylistItem { SongId = songId, UserId = user.Id, Order = user.Playlists.Count() + 1 });
                            db.SaveChanges();
                        }
                        catch
                        {
                            return Json(new { status = "fail", error = "somthing go wrong" });
                        }
                        return Json(new { status = "success" });
                    }
                    return Json(new { status = "fail", error = "song not found" });
                }
                return Json(new { status = "fail", error = "user not found" });
            }
        }

        public ActionResult RemoveFromUsersPlaylist(int userId, int songId)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                UserProfile user = userId == -1 ? db.UserProfiles.Content.FirstOrDefault(u => u.UserName == User.Identity.Name) : db.UserProfiles.Content.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    var song = db.Songs.Content.FirstOrDefault(s => s.Id == songId);
                    if (song != null)
                    {
                        try
                        {
                            var playlist = db.PlaylistItems.Content.FirstOrDefault(p => (p.SongId == songId) && (p.UserId == user.Id));
                            if (playlist != null)
                            {
                                db.PlaylistItems.Delete(playlist);
                                foreach (var pl in db.PlaylistItems.Content.Where(p => p.Order > playlist.Order)) { pl.Order--; }
                                db.SaveChanges();
                            }
                        }
                        catch
                        {
                            return Json(new { status = "fail", error = "somthing go wrong" });
                        }
                        return Json(new { status = "success" });
                    }
                    return Json(new { status = "fail", error = "song not found" });
                }
                return Json(new { status = "fail", error = "user not found" });
            }
        }
    }

    public class SqlReaderStream : Stream
    {
        private SqlDataReader _reader;
        private readonly int _columnIndex;
        private long _position;

        public SqlReaderStream(
            SqlDataReader reader,
            int columnIndex)
        {
            _reader = reader;
            _columnIndex = columnIndex;
        }

        public override long Position
        {
            get { return _position; }
            set { throw new NotImplementedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long bytesRead = _reader.GetBytes(_columnIndex, _position, buffer, offset, count);
            _position += bytesRead;
            return (int)bytesRead;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && null != _reader)
            {
                _reader.Dispose();
                _reader = null;
            }
            base.Dispose(disposing);
        }
    }
}