namespace Shop_Microservicios.Models.Api.Shipping
{
    public class CreateShipmentRequest
    {
        public long OrderId { get; set; }

        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";

        public string AddressLine1 { get; set; } = "";
        public string City { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string Address { get; internal set; }
    }
}
