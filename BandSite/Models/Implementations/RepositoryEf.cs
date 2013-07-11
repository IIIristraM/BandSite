using BandSite.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;

namespace BandSite.Models.Implementations
{
    public class RepositoryEf<Entity> : IRepository<Entity>
        where Entity : class, IEntity
    {
        protected DbSet<Entity> content;

        public RepositoryEf(DbSet<Entity> entitySet)
        {
            content = entitySet;
        }

        public IQueryable<Entity> Content
        {
            get { return (IQueryable<Entity>)content; }
        }

        public Entity Insert(Entity entity)
        {
            return content.Add(entity);
        }

        public Entity Delete(Entity entity)
        {
            return content.Remove(entity);
        }

        public Entity Update(Entity entity)
        {
            content.Where(e => e.Id == entity.Id).Load();
            Entity result = content.Find(entity.Id);
            if (result.TrySetPropertiesFrom(entity))
            {
                return result;
            }
            return null;
        }
    }
}