using System;

namespace Shop_Microservicios.Models.Api
{
    public class CartItemResponse
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

