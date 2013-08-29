using BandSite.Models.DataLayer;
using System.Web.Mvc;

namespace BandSite.Areas.AdministrativeTools.Controllers
{
    [Authorize]
    public class NavigationController : Controller
    {
        //
        // GET: /AdministrativeTools/Navigation/
        public NavigationController()
        {
        }

        public NavigationController(IDbContextFactory dbContextFactory) : this()
        {
            
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
