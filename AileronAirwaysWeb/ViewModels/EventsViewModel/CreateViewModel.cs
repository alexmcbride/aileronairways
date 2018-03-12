using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AileronAirwaysWeb.ViewModels.EventsViewModel
{
    public class CreateViewModel
    {
        [Required/*(ErrorMessage = "Description required")*/]
        [Display(Name = "Description")]
        [RegularExpression(@"^.{2,}$"/*, ErrorMessage = "Description minimum 2 characters required"*/)]
        public string Description { get; set; }
        [Required/*(ErrorMessage = "Title required")*/]
        [Display(Name = "Title")]
        [RegularExpression(@"^.{2,}$"/*, ErrorMessage = "Title minimum 2 characters required"*/)]
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
    }
}
