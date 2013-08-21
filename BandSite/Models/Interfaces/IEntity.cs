using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandSite.Models.Interfaces
{
    public interface IEntity
    {
        int Id { get; set; }

        bool TrySetPropertiesFrom(object source);
        
    }
}
