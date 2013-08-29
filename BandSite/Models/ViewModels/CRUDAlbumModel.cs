using System;
using System.ComponentModel.DataAnnotations;

namespace BandSite.Models.ViewModels
{
    public class CRUDAlbumModel
    {
        public virtual int Id { get; set; }

        [Display(Name = "Album Title")]
        [Required]
        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? Published { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }
}