using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;




namespace AileronAirwaysWeb.ViewModels.TimelineViewModels
{
    public class EditViewModel
    {

       
        [Required]
        public string Title { get; set; }

        public string ID { get; set; }
    }
}
