using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace AileronAirwaysWeb.Extensions
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// Replaces newlines with HTML break rule tags.
        /// </summary>
        public static IHtmlContent KeepLines(this IHtmlHelper helper, string content)
        {
            return helper.Raw(content.Replace(Environment.NewLine, "<br>"));
        }
    }
}
