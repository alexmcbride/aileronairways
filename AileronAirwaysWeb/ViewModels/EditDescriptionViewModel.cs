using System.ComponentModel.DataAnnotations;

namespace AileronAirwaysWeb.ViewModels
{
    public class EditDescriptionViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
