using System.Web.Mvc;
using BandSite.Models.DataLayer;
using Microsoft.Ajax.Utilities;

namespace BandSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public HomeController() {}
        public HomeController(IDbContextFactory dbContextFactory):this() {}

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
