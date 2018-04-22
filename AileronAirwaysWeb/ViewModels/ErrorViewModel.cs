using System;

namespace AileronAirwaysWeb.ViewModels
{
    /// <summary>
    /// Viewmodel for displaying an error.
    /// </summary>
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}