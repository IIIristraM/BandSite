﻿using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using BandSite.Models.DataLayer;
using BandSite.Models.Entities;
using BandSite.Models.Functionality;
using BandSite.Models.Hubs;
using BandSite.Models.ViewModels;

namespace BandSite.Controllers
{
    [Authorize]
    public class SongController : Controller
    {
        private readonly IDbContextFactory _dbContextFactory;

        public SongController(IDbContextFactory dbContextFactory)
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
                ViewBag.Albums = db.Albums.Content.Where(a => a.Songs.Count(s => s.Id == id) == 1).ToList();
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
                        var uploaderHub = new UploaderHub();
                        var uploader = new Uploader();
                        foreach (var result in uploader.UploadPartial(newSong.File, song.UploadFile.InputStream, User.Identity.Name))
                        {
                            uploaderHub.ShowProgress(User.Identity.Name, result);
                        }
                    }
                    db.Songs.Insert(newSong);
                    db.SaveChanges();
                    return Json(new { hash = "#song/index" });
                }
                return Json(new { hash = "#song/create" });
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
                ViewBag.Albums = db.Albums.Content.Where(a => a.Songs.Count(s => s.Id == id) == 1).ToList();
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
                        /*if (song.UploadFile != null)
                        {
                            updatedSong.File = new byte[song.UploadFile.InputStream.Length];
                            var uploader = new UploaderHub();
                            uploader.Upload(updatedSong.File, song.UploadFile.InputStream, User.Identity.Name);
                        }*/
                        db.Songs.Update(song.Id, updatedSong);
                        db.SaveChanges();
                        return Json(new { hash = "#song/index" });
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
                ViewBag.Albums = db.Albums.Content.Where(a => a.Songs.Count(s => s.Id == id) == 1).ToList();
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
                return Json(new { hash = "#song/index" });
            }
        }

        [AllowAnonymous]
        public ActionResult SongsSearch(string term)
        {
            if (Request.IsAuthenticated)
            {
                using (var db = _dbContextFactory.CreateContext())
                {
                    var songs =
                        db.Songs.Content.Where(s => s.Title.Contains(term))
                            .Select(s => new {label = s.Title, value = s.Id})
                            .ToList();
                    return Json(songs, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var songs = new object[]
                {
                    new {label = "You need to log in", value = ""}
                };
                return Json(songs, JsonRequestBehavior.AllowGet);
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
                db.SaveChanges();
                return Json(new { hash = "#song/edit/" + song.Id });
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
                db.SaveChanges();
                return Json(new { hash = "#song/edit/" + song.Id });
            }
        }

        [AllowAnonymous]
        public FileStreamResult GetStream(int id)
        {
            using (var db = _dbContextFactory.CreateContext())
            {
                var query = db.Songs.Content.Where(s => s.Id == id).Select(s => s.File);
                var song = query.FirstOrDefault();
                if (song != null)
                {
                    var scsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
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
                        SetResponse(song.Length);
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

        private void SetResponse(int contentLength)
        {
            Response.Headers.Add("Accept-Ranges", "bytes");

            var range = Request.Headers["Range"];
            if (!String.IsNullOrEmpty(range) &&
                (range.IndexOf("-") == range.Length - 1))
            {
                var left = range.IndexOf("=") + 1;
                var right = range.IndexOf("-");
                int startPoint = Int32.Parse(range.Substring(left, right - left));

                Response.Headers.Add("Content-Range", "bytes " + startPoint + "-" + (contentLength - 1) + "/" + contentLength);
                Response.Headers.Add("Content-Length", (contentLength - startPoint).ToString());
                Response.StatusCode = (int)HttpStatusCode.PartialContent;
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