using AileronAirwaysWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AileronAirwaysWeb.Services
{
    public class FlashService : IFlashService
    {
        private readonly ITempDataDictionary _tempData;

        public FlashService(ITempDataDictionaryFactory factory, IHttpContextAccessor contextAccessor)
        {
            _tempData = factory.GetTempData(contextAccessor.HttpContext);
        }

        public void Message(string text)
        {
            Message(text, FlashType.Success);
        }

        public Queue<FlashMessage> GetMessages()
        {
            if (_tempData.TryGetValue("flash-queue", out object data))
            {
                return JsonConvert.DeserializeObject<Queue<FlashMessage>>(data.ToString());
            }
            return new Queue<FlashMessage>();
        }

        public void PutMessages(Queue<FlashMessage> messages)
        {
            _tempData["flash-queue"] = JsonConvert.SerializeObject(messages);
        }

        public void Message(string text, FlashType type)
        {
            var messages = GetMessages();

            messages.Enqueue(new FlashMessage
            {
                Type = type,
                Text = text
            });

            PutMessages(messages);
        }
    }
}
