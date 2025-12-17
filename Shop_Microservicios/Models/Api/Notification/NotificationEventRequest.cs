public class NotificationEventRequest
{
    public int UserId { get; set; }
    public string Type { get; set; } = "";       // "RECEIPT_EMAIL"
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public string Channel { get; set; } = "";    // "EMAIL" o "IN_APP"
    public string? Email { get; set; }           // requerido si Channel="EMAIL"
    public string RequestId { get; set; } = "";  // idempotencia
    public string Source { get; set; } = "MVC";
}
