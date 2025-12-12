namespace Shop_Microservicios.Models
{
    public class CartViewItem
    {
        public long ProductId { get; set; }
        public string? Title { get; set; }
        public string? Thumbnail { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
