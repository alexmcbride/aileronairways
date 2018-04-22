using System.ComponentModel.DataAnnotations;

namespace AileronAirwaysWeb.ViewModels
{
    /// <summary>
    /// Viewmodel for a timeline event with location.
    /// </summary>
    public class TimelineEventLocationViewModel
    {
        [Required]    
        public string Location { get; set; }
        [RegularExpression(@"^(\+|-)?(?:85(?:(?:\.0{1,50})?)|(?:[0-9]|[1-8][0-5])(?:(?:\.[0-9]{1,50})?))$", ErrorMessage = "No details available for this latitude input:valid input is more than -85 and less than 85, data you input does not save")]
        public string latitude { get; set; }
        [RegularExpression(@"^(\\+|-)?(?:180(?:(?:\\.0{1,50})?)|(?:[0-9]|[1-9][0-9]|1[0-7][0-9])(?:(?:\\.[0-9]{1,50})?))$", ErrorMessage = "No details available for this longititude input: valid input is more than - 180 less than 180, data you input does not save")]
        public string longitude { get; set; }
    }
}
