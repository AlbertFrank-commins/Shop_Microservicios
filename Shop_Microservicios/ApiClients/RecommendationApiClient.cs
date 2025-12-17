using System.Net.Http;
using System.Net.Http.Json;
using Shop_Microservicios.Models.Api.Recommendation;

namespace Shop_Microservicios.ApiClients
{
    public class RecommendationApiClient : BaseApiClient
    {
        public RecommendationApiClient(HttpClient http)
            : base(http, "Recomendaciones")
        {
        }

        public Task SendEventAsync(RecommendationEventRequest req, CancellationToken ct = default)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, "/api/recommendations/events")
            {
                Content = JsonContent.Create(req)
            };

            return SendAsync(msg, ct);
        }

        public Task<List<ProductRefDto>> GetTrendingAsync(int limit = 12, CancellationToken ct = default)
            => GetAsync<List<ProductRefDto>>($"/api/recommendations/trending?limit={limit}", ct);

        public Task<List<ProductRefDto>> GetRelatedAsync(long productId, string? category, int limit = 8, CancellationToken ct = default)
        {
            var url = $"/api/recommendations/related/{productId}?limit={limit}";
            if (!string.IsNullOrWhiteSpace(category))
                url += $"&category={Uri.EscapeDataString(category)}";

            return GetAsync<List<ProductRefDto>>(url, ct);
        }

        public Task<List<ProductRefDto>> GetForYouAsync(long userId, int limit = 12, CancellationToken ct = default)
            => GetAsync<List<ProductRefDto>>($"/api/recommendations/for-you?userId={userId}&limit={limit}", ct);
    }
}
