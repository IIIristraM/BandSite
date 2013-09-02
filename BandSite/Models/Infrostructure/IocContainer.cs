using BandSite.Models.DataLayer;
using BandSite.Models.Functionality;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BandSite.Models.Infrostructure
{
    public class IocContainer : Microsoft.AspNet.SignalR.DefaultDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        private readonly IKernel _kernel;

        public IocContainer()
        {
            _kernel = new StandardKernel();
            AddBindings();
        }

        public override object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType) ?? base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType).Concat(base.GetServices(serviceType) ?? new List<object>(0));
        }

        private void AddBindings()
        {
            _kernel.Bind<IDbContextFactory>().To<DbContextEfFactory>();
            _kernel.Bind<IDbContext>().To<DbContextEf>();
            _kernel.Bind(typeof(IRepository<>)).To(typeof(RepositoryEf<>));

            _kernel.Bind<IChat>().To<Chat>();
        }

        #region System.Web.Mvc.IDependencyResolver

        object System.Web.Mvc.IDependencyResolver.GetService(Type serviceType)
        {
            return GetService(serviceType);
        }

        IEnumerable<object> System.Web.Mvc.IDependencyResolver.GetServices(Type serviceType)
        {
            return GetServices(serviceType);
        }

        #endregion
    }
}
