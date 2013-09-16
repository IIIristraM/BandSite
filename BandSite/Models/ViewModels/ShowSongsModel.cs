using System.Collections.Generic;
using BandSite.Models.Entities;

namespace BandSite.Models.ViewModels
{
    public class ShowSongsModel
    {
        public int AlbumId { get; set; }
        public IEnumerable<Song> Songs { get; set; }
        public bool Editable { get; set; }
    }
}