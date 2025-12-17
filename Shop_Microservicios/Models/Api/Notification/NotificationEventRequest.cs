namespace Shop_Microservicios.Models.Api.Notification
{
    public class NotificationEventRequest
    {
        public long UserId { get; set; }   // 👈 IMPORTANTE: long
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Channel { get; set; } = "IN_APP";
        public string? Email { get; set; }
        public string RequestId { get; set; } = string.Empty;
        public string Source { get; set; } = "MVC";
    }
}
