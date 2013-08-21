using BandSite.Models.Implementations;
using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BandSite.Controllers
{
    public class HomeController : Controller
    {
        IDbContext _db;

        public ActionResult Index()
        {
            using (_db = new DbContextEfFactory("BandSiteDB").CreateContext())
            {
                try
                {
                    var count = _db.Albums.Content.Count();
                    ViewBag.Message = "Albums Count: " + count;
                }
                catch (Exception e)
                {
                    string message = e.Message;
                }
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
