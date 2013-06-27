using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BandSite.Models.Implementations
{
    public class RepositoryEf<Entiy> : IRepository<Entiy>
        where Entiy : class
    {
        protected DbSet<Entiy> content;

        public RepositoryEf(DbSet<Entiy> entitySet)
        {
            content = entitySet;
        }

        public IQueryable<Entiy> Content
        {
            get { return content; }
        }

        public Entiy Insert(Entiy entity)
        {
            return content.Add(entity);
        }

        public Entiy Delete(Entiy entity)
        {
            return content.Remove(entity);
        }
    }
}