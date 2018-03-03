using AileronAirwaysWeb.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AileronAirwaysWeb.TagHelpers
{
    [HtmlTargetElement("flash-messages")]
    public class FlashMessagesTagHelper : TagHelper
    {
        private readonly IFlashService _flashService;

        public FlashMessagesTagHelper(IFlashService flashService)
        {
            _flashService = flashService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Switch custom tag to div with class.
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "flash-messages");
            output.TagMode = TagMode.StartTagAndEndTag;

            // Get messages and add a new Bootstrap alert for each one.
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
