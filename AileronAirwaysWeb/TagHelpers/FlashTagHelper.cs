using AileronAirwaysWeb.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AileronAirwaysWeb.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "flash-messages")]
    public class FlashTagHelper : TagHelper
    {
        private readonly IFlashService _flashService;

        public FlashTagHelper(IFlashService flashService)
        {
            _flashService = flashService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var messages = _flashService.GetMessages();
            foreach (var message in messages)
            {
                string type = message.Type.ToString();
                output.Content.AppendHtml($"<div class=\"alert alert-{type.ToLower()} alert-dismissible\" role=\"alert\">");
                output.Content.AppendHtml("<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>");
                output.Content.AppendHtml($"<strong>{type}!</strong> {message.Text}");
                output.Content.AppendHtml("</div>");
            }
        }
    }
}
