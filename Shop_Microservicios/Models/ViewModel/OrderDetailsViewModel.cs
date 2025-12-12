// Models/ViewModels/Orders/OrderDetailsViewModel.cs
using Shop_Microservicios.Models.Api.Shipping;
using System;
using System.Collections.Generic;

namespace Shop_Microservicios.Models.ViewModels.Orders
{
    public class OrderDetailsViewModel
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public ShipmentResponse? Shipping { get; set; }
        public List<OrderDetailsItemViewModel> Items { get; set; } = new();
    }
}
