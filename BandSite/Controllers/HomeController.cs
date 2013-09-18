using System.Web.Mvc;

namespace BandSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Navigation()
        {
            return View();
        }

        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult Playlist()
        {
            return PartialView();
        }
    }
}
