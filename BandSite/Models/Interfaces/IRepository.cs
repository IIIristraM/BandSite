using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandSite.Models.Interfaces
{
    public interface IRepository<Entity> where Entity : class, IEntity
    {
        IQueryable<Entity> Content { get; }

        Entity Insert(Entity entity);
        Entity Delete(Entity entity);
        Entity Update(Entity entity);
    }
}
