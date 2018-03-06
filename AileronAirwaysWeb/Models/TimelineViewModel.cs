﻿using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AileronAirwaysWeb.Models
{
    public class TimelineViewModel
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Display(Name = "Created")]
        public DateTime CreationTimeStamp { get; set; }

        public string PrettyCreation
        {
            get { return CreationTimeStamp.ToShortDateString() + " " + CreationTimeStamp.ToShortTimeString(); }
        }

        public bool IsDeleted { get; set; }
    }
}
