using BandSite.Models.DataLayer;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BandSite.Models.Infrostructure;
using Microsoft.AspNet.SignalR;

namespace BandSite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static IDbContextFactory _dbFactory;

        public static IDbContextFactory DbFactory 
        {
            get 
            { 
                if (_dbFactory == null) 
                { 
                    _dbFactory = new DbContextEfFactory("DefaultConnection"); 
                }
                return _dbFactory;
            }
        }

        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            DependencyResolver.SetResolver(new IocContainer());
            RouteTable.Routes.MapHubs(new HubConfiguration
            {
                Resolver = new IocContainer()
            });
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
    }
}