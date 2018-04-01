using AileronAirwaysWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AileronAirwaysWeb.ViewModels
{
    public class TimelineEventViewModel
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [ScaffoldColumn(false)]
        public string TimelineId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Display(Name = "Event Occured")]
        [Required]
        public DateTime EventDateTime { get; set; }

        [Required]
        public string Location { get; set; }

        public int AttachmentFilesCount { get; set; }
        public int AttachmentImagesCount { get; set; }

        public List<Attachment> Attachments { get; set; }
    }
}
