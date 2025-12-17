namespace Shop_Microservicios.Models.Api.Notification
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Channel { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
