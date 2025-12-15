namespace Shop_Microservicios.Models.ViewModels.Shipping
{
    public class CreateShipmentViewModel
    {
        public long OrderId { get; set; }
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
    }
}
