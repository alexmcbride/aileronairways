﻿using System;
using System.ComponentModel.DataAnnotations;

namespace AileronAirwaysWeb.Models
{
    public class TimelineViewModel
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime CreationTimeStamp { get; set; }

        public bool IsDeleted { get; set; }
    }
}
