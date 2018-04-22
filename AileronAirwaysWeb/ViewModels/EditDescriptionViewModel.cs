using System.ComponentModel.DataAnnotations;

namespace AileronAirwaysWeb.ViewModels
{
    /// <summary>
    /// ViewModel used when editing an event description.
    /// </summary>
    public class EditDescriptionViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
