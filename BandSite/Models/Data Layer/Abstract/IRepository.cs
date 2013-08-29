using System.Linq;

namespace BandSite.Models.DataLayer
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        IQueryable<TEntity> Content { get; }

        TEntity GetById(int id);
        TEntity Insert(TEntity entity);
        TEntity Delete(TEntity entity);
        TEntity Delete(int id);
        TEntity Update(int id, TEntity entity, bool ignoreNulls = true);
    }
}
