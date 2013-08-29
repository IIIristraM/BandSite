using System;
using System.Web.Mvc;
using BandSite.Models.DataLayer;

namespace BandSite.Models.Infrostructure
{
    public class CustomControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            IDbContextFactory dbContext = MvcApplication.DbFactory;
            IController controller = Activator.CreateInstance(controllerType, new[] { dbContext }) as Controller;
            return controller;
        }
    } 
}