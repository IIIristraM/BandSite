using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandSite.Models.Interfaces
{
    public interface IRepository<Entiy> where Entiy : class
    {
        IQueryable<Entiy> Content { get; }

        Entiy Insert(Entiy entity);
        Entiy Delete(Entiy entity);
    }
}
