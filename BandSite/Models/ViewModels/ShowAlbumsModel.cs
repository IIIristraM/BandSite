using System.Collections.Generic;
using BandSite.Models.Entities;

namespace BandSite.Models.ViewModels
{
    public class ShowAlbumsModel
    {
        public int SongId { get; set; }
        public IEnumerable<Album> Albums { get; set; }
        public bool Editable { get; set; }
    }
}
