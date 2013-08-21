using System.Web.Mvc;

namespace BandSite.Areas.AdministrativeTools
{
    public class AdministrativeToolsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AdministrativeTools";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AdministrativeTools_default",
                "AdministrativeTools/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
