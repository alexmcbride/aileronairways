using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.ViewModels.EventsViewModel
{
    public class CreateViewModel
    {
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
    }
}
