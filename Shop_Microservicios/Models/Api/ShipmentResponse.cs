namespace Shop_Microservicios.Models.Api.Shipping;

public class ShipmentResponse
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long UserId { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Status { get; set; } // CREATED / SHIPPED / DELIVERED...
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
