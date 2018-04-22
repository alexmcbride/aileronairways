using System.Collections.Generic;

namespace AileronAirwaysWeb.Services
{
    /// <summary>
    /// Interface for flash service
    /// </summary>
    public interface IFlashService
    {
        Queue<FlashMessage> GetMessages();
        void Message(string text);
        void Message(string text, FlashType type);
        bool HasMessages { get; }
    }
}
