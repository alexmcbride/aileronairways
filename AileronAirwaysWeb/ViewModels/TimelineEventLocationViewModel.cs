using System.ComponentModel.DataAnnotations;

namespace AileronAirwaysWeb.ViewModels
{
    public class TimelineEventLocationViewModel
    {
        [Required]
        public string Location { get; set; }
    }
}
