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
            if (entity != null)
            {
                return content.Add(entity);
            }
            else
            {
                return null;
            }
        }

        public Entity Delete(Entity entity)
        {
            if (entity != null)
            {
                return Delete(entity.Id);
            }
            else
            {
                return null;
            }
        }

        public Entity Delete(int id)
        {
            var item = content.Find(id);
            if (item != null)
            {
                return content.Remove(item);
            }
            else
            {
                return null;
            }
        }

        public Entity Update(int id, Entity entity)
        {
            Entity result = content.Find(id);
            if (result != null)
            {
                result.TrySetPropertiesFrom(entity);
            }
            return result;
        }
    }
}