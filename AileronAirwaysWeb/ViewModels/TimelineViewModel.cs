using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AileronAirwaysWeb.ViewModels
{
    public class TimelineViewModel
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Display(Name = "Created")]
        public DateTime CreationTimeStamp { get; set; }

        public string Created
        {
            get { return CreationTimeStamp.ToString("o"); }
        }

        public string CreatedPretty
        {
            get { return $"{CreationTimeStamp.ToString("yyyy/MM/dd")} {CreationTimeStamp.ToShortTimeString()}"; }
        }

        public bool IsDeleted { get; set; }

        public int EventsCount { get; set; }
    }
}
