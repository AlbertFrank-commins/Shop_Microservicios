using Shop.Web.Models.Api;

namespace Shop_Microservicios.Models.Api
{
    public class ProductsResponse
    {
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
        public List<ProductDto> Products { get; set; } = new();
    }
}
