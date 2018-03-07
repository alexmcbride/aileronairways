using AileronAirwaysWeb.Controllers;
using AileronAirwaysWeb.Models;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string AttachmentLink(this IUrlHelper helper, Attachment attachment)
        {
            return helper.AttachmentLink(attachment.Id);
        }

        public static string AttachmentLink(this IUrlHelper helper, string attachmentId)
        {
            return helper.Action("download", "attachments", new { attachmentId });
        }
    }
}
