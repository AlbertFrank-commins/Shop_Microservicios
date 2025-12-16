using Shop.Web.Models.Api;

namespace Shop_Microservicios.Models.ViewModel
{
    public class ProductDetailsViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Thumbnail { get; set; } = "";
        public double Rating { get; set; }
        public List<ProductDto> RelatedProducts { get; set; } = new();
    }
}
