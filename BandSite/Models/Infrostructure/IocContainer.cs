using BandSite.Models.DataLayer;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BandSite.Models.Infrostructure
{
    public class IocContainer : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public IocContainer()
        {
            _kernel = new StandardKernel();
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            _kernel.Bind<IDbContextFactory>().To<DbContextEfFactory>();
            _kernel.Bind<IDbContext>().To<DbContextEf>();
            _kernel.Bind(typeof(IRepository<>)).To(typeof(RepositoryEf<>));
        }
    }
}
