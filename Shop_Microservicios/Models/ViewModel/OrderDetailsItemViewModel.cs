// Models/ViewModels/Orders/OrderDetailsItemViewModel.cs
namespace Shop_Microservicios.Models.ViewModels.Orders
{
    public class OrderDetailsItemViewModel
    {
        public long ProductId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
