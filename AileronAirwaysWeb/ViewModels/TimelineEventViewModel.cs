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
    }
}
