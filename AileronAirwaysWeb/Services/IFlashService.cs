using AileronAirwaysWeb.Models;
using System.Collections.Generic;

namespace AileronAirwaysWeb.Services
{
    public interface IFlashService
    {
        Queue<FlashMessage> GetMessages();
        void Message(string text);
        void Message(string text, FlashType type);
        bool HasMessages { get; }
    }

}
