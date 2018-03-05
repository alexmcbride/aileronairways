namespace AileronAirwaysWeb.Models
{
    public enum FlashType
    {
        None,
        Success,
        Info,
        Warning,
        Danger
    }

    public class FlashMessage
    {
        public FlashType Type { get; set; }
        public string Text { get; set; }
    }
}
