using System;
using System.Collections.Generic;

namespace Shop_Microservicios.Models.Api.Orders
{
    // ✅ Elimina el enum o déjalo de adorno si quieres, pero no lo uses en OrderResponse
    // public enum OrderStatus
    // {
    //     PENDING,
    //     PAID,
    //     CANCELLED
    // }

    public class OrderItemResponse
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class OrderResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public decimal TotalAmount { get; set; }

        // 🔥 Ahora como string para aceptar "PENDING", "PAID", etc.
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
    }
}
