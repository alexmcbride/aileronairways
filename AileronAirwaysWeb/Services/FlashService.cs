using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AileronAirwaysWeb.Services
{
    /// <summary>
    /// Services for making those cool little flash messages appear at the top of pages. It stores 
    /// messages in the temp session data between page loads. TempData automatically gets wiped.
    /// </summary>
    public class FlashService : IFlashService
    {
        private readonly ITempDataDictionary _tempData;

        /// <summary>
        /// Gets if there are any messages to display.
        /// </summary>
        public bool HasMessages
        {
            get
            {
                return GetMessages().Any();
            }
        }

        /// <summary>
        /// Injected constructor.
        /// </summary>
        public FlashService(ITempDataDictionaryFactory factory, IHttpContextAccessor contextAccessor)
        {
            _tempData = factory.GetTempData(contextAccessor.HttpContext);
        }

        /// <summary>
        /// Gets a queue of flash messages.
        /// </summary>
        /// <returns></returns>
        public Queue<FlashMessage> GetMessages()
        {
            if (_tempData.TryGetValue("flash-queue", out object data))
            {
                return JsonConvert.DeserializeObject<Queue<FlashMessage>>(data.ToString());
            }
            return new Queue<FlashMessage>();
        }

        /// <summary>
        /// Saves a queue of flash messages to the session.
        /// </summary>
        /// <param name="messages"></param>
        public void PutMessages(Queue<FlashMessage> messages)
        {
            _tempData["flash-queue"] = JsonConvert.SerializeObject(messages);
        }

        /// <summary>
        /// Adds a new flash message.
        /// </summary>
        public void Message(string text)
        {
            Message(text, FlashType.Success);
        }

        /// <summary>
        /// Adds a new flash message with optional message type.
        /// </summary>
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

    /// <summary>
    /// Flash message type.
    /// </summary>
    public enum FlashType
    {
        None,
        Success,
        Info,
        Warning,
        Danger
    }

    /// <summary>
    /// Wee class to store a flash message.
    /// </summary>
    public class FlashMessage
    {
        public FlashType Type { get; set; }
        public string Text { get; set; }
    }
}
