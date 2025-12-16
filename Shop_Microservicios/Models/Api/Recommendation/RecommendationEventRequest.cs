namespace Shop_Microservicios.Models.Api.Recommendation
{
    public class RecommendationEventRequest
    {
        public long? UserId { get; set; }
        public string EventType { get; set; } = ""; // VIEW | ADD_TO_CART | PURCHASE
        public long ProductId { get; set; }
        public string? Category { get; set; }
        public string? RequestId { get; set; }
        public string? Source { get; set; } = "MVC";
    }
}
