using AileronAirwaysWeb.Models;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Creates an attachment link, so no one has to remember how to do it.
        /// </summary>
        public static string AttachmentLink(this IUrlHelper helper, Attachment attachment)
        {
            return helper.AttachmentLink(attachment.Id);
        }

        /// <summary>
        /// Creates an attachment link, so no one has to remember how to do it.
        /// </summary>
        public static string AttachmentLink(this IUrlHelper helper, string attachmentId)
        {
            return helper.Action("download", "attachments", new { attachmentId });
        }
    }
}
