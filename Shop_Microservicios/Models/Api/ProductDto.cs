namespace Shop.Web.Models.Api;

public class ProductDto
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public string Category { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Thumbnail { get; set; } = "";
    public double Rating { get; set; }   // si DummyJSON lo trae
}

public class DummyJsonProductsResponse
{
    public List<ProductDto> Products { get; set; }
}
