using System;
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

        public string Location { get; set; }

        public int AttachmentFilesCount { get; set; }
        public int AttachmentImagesCount { get; set; }

        
    }

    public class TimelineEventLocationViewModel
    {
        [Required]    
        public string Location { get; set; }
        [RegularExpression(@"^(\+|-)?(?:90(?:(?:\.0{1,15})?)|(?:[0-9]|[1-8][0-9])(?:(?:\.[0-9]{1,15})?))$", ErrorMessage = "No details available for this latitude input:valid input is more than -85 and less than 85, data you input does not save")]
        public string latitude { get; set; }
        [RegularExpression(@"^(\\+|-)?(?:180(?:(?:\\.0{1,15})?)|(?:[0-9]|[1-9][0-9]|1[0-7][0-9])(?:(?:\\.[0-9]{1,15})?))$", ErrorMessage = "No details available for this longititude input: valid input is more than - 180 less than 180, data you input does not save")]
        public string longitude { get; set; }
    }
}
