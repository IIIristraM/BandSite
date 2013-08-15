using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BandSite.Models
{
    public class CreateSong
    {
        public int Id { get; set; }

        [Display(Name = "Song Title")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [Required]
        public HttpPostedFileBase UploadFile { get; set; }
    }
}