using System.Text.Json.Serialization;

namespace Shop_Microservicios.Models.Api
{
    public class AddOrUpdateCartItemRequest
    {
        [JsonPropertyName("productId")]
        public long ProductId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
