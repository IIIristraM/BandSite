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
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Objects;

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
        public ActionResult Create(CreateSong song)
        {
            if (ModelState.IsValid)
            {
                Song newSong = new Song();
                newSong.Title = song.Title;
                newSong.Text = song.Text;
                newSong.File = new byte[song.UploadFile.InputStream.Length];
                song.UploadFile.InputStream.Read(newSong.File, 0, newSong.File.Length);
                _db.Songs.Insert(newSong);
                _db.SaveChanges();
                return Json(new { hash = "action=index&entity=song" });
            }

            return PartialView();
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

        public FileStreamResult GetStream(int id)
        {
            Response.Headers.Add("Accept-Ranges", "bytes");
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetMaxAge(new TimeSpan(0, 10, 0));
            var query = _db.Songs.Content.Where(s => s.Id == id).Select(s => s.File);
            var song = query.FirstOrDefault();
            if(song != null)
            {
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["BandSiteDB"].ConnectionString);
                SqlConnection conn = new SqlConnection(scsb.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                cmd.Parameters.Add(new SqlParameter("p__linq__0", id));
                SqlDataReader reader = cmd.ExecuteReader( CommandBehavior.SequentialAccess |
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

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class SqlReaderStream : Stream
    {
        private SqlDataReader reader;
        private int columnIndex;
        private long position;

        public SqlReaderStream(
            SqlDataReader reader,
            int columnIndex)
        {
            this.reader = reader;
            this.columnIndex = columnIndex;
        }

        public override long Position
        {
            get { return position; }
            set { throw new NotImplementedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long bytesRead = reader.GetBytes(columnIndex, position, buffer, offset, count);
            position += bytesRead;
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
            if (disposing && null != reader)
            {
                reader.Dispose();
                reader = null;
            }
            base.Dispose(disposing);
        }
    }
}