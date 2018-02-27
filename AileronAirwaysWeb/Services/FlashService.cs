using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AileronAirwaysWeb.Services
{
    public interface IFlashService
    {
        void Flash(string type, string text);
    }

    [HtmlTargetElement("div", Attributes = "flash-messages")]
    public class FlashTagHelper : TagHelper
    {
        private readonly ITempDataDictionary _tempData;

        public FlashTagHelper(ITempDataDictionaryFactory factory, IHttpContextAccessor contextAccessor)
        {
            _tempData = factory.GetTempData(contextAccessor.HttpContext);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var messages = _tempData.Get<Queue<Message>>("flash-queue");
            if (messages != null && messages.Any())
            {
                output.Content.AppendHtml("<ul>");
                foreach (var message in messages)
                {
                    output.Content.AppendHtml($"<li>{message.Text}</li>");
                }
                output.Content.AppendHtml("</ul>");
            }
        }
    }

    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value)
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key)
        {
            if (tempData.TryGetValue(key, out object value))
            {
                return JsonConvert.DeserializeObject<T>(value.ToString());
            }
            return default(T);
        }
    }

    public class Message
    {
        public string Type { get; set; }
        public string Text { get; set; }
    }

    public class FlashService : IFlashService
    {
        private readonly ITempDataDictionary _tempData;

        public FlashService(ITempDataDictionaryFactory factory, IHttpContextAccessor contextAccessor)
        {
            _tempData = factory.GetTempData(contextAccessor.HttpContext);
        }

        public void Flash(string type, string text)
        {
            var messages = _tempData.Get<Queue<Message>>("flash-queue");
            if (messages == null)
            {
                messages = new Queue<Message>();
            }
            messages.Enqueue(new Message
            {
                Type = type,
                Text = text
            });
            _tempData.Put("flash-queue", messages);
        }
    }
}
