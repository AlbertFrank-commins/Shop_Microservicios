using System;

namespace Shop_Microservicios.Models.Api.Payments
{
    public class PaymentResponse
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long UserId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;   // APPROVED / DECLINED
        public DateTime CreatedAt { get; set; }
    }
}
