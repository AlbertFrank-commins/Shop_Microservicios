// CreateOrderRequest.cs
using System.Collections.Generic;

namespace Shop_Microservicios.Models.Api.Orders
{
    public class CreateOrderRequest
    {
        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
