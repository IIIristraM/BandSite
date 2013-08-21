using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BandSite.Areas.AdministrativeTools.Controllers
{
    [Authorize]
    public class NavigationController : Controller
    {
        //
        // GET: /AdministrativeTools/Navigation/

        public ActionResult Index()
        {
            return View();
        }
    }
}
