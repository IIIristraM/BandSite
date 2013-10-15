using System.Web.Mvc;

namespace BandSite.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Navigation()
        {
            return View();
        }

        public ActionResult Index()
        {
            return PartialView();
        }

        [AllowAnonymous]
        public ActionResult Playlist()
        {
            return PartialView();
        }
    }
}
