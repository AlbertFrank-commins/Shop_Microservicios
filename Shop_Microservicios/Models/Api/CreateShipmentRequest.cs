namespace Shop_Microservicios.Models.Api.Shipping;

public class CreateShipmentRequest
{
    public long OrderId { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
