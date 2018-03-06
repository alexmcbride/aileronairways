using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace AileronAirwaysWeb.ViewModels.EventsViewModel
{
    public class EditViewModel
    {

        public string Description { get; set; }

        [Required]
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }

    }
}
