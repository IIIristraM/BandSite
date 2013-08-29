using System.Data.Entity;
using System.Linq;

namespace BandSite.Models.DataLayer
{
    public class RepositoryEf<TEntity> : IRepository<TEntity>
        where TEntity : EntityBase
    {
        private readonly DbSet<TEntity> _content;

        public RepositoryEf(DbSet<TEntity> entitySet)
        {
            _content = entitySet;
        }

        public IQueryable<TEntity> Content
        {
            get { return _content; }
        }

        public TEntity GetById(int id)
        {
            var item = _content.Find(id);
            if (item != null)
            {
                return item;
            }
            return null;
        }

        public TEntity Insert(TEntity entity)
        {
            if (entity != null)
            {
                return _content.Add(entity);
            }
            return null;
        }

        public TEntity Delete(TEntity entity)
        {
            if (entity != null)
            {
                return Delete(entity.Id);
            }
            return null;
        }

        public TEntity Delete(int id)
        {
            var item = _content.Find(id);
            if (item != null)
            {
                return _content.Remove(item);
            }
            return null;
        }

        public TEntity Update(int id, TEntity entity, bool ignoreNulls = true)
        {
            if (entity != null)
            {
                var result = _content.Find(id);
                if (result != null)
                {
                    result.TrySetPropertiesFrom(entity, ignoreNulls);
                }
                return result;
            }
            return null;
        }
    }
}