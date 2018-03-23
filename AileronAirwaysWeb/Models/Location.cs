using System.ComponentModel.DataAnnotations;

namespace AileronAirwaysWeb.Models
{
    public class Location
    {
        [Required]
        public double Longitude { get; set; }

        [Required]
        public double Latitude { get; set; }
    }
}
