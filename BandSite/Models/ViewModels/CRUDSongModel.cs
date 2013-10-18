using System.ComponentModel.DataAnnotations;
using System.Web;

namespace BandSite.Models.ViewModels
{
    public class CRUDSongModel
    {
        public int Id { get; set; }

        [Required]
        public string Band { get; set; }

        [Display(Name = "Song Title")]
        [Required]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        public HttpPostedFileBase UploadFile { get; set; }
    }
}