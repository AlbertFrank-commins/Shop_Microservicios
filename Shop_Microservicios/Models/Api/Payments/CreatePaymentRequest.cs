namespace Shop_Microservicios.Models.Api.Payments
{
    public class CreatePaymentRequest
    {
        public long OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "CARD";
        public string CardNumber { get; set; } = string.Empty;
    }
}
