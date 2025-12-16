using System.Net.Http.Json;
using Shop_Microservicios.Models.Api.Recommendation;

namespace Shop_Microservicios.ApiClients
{
    public class RecommendationApiClient
    {
        private readonly HttpClient _http;

        public RecommendationApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task SendEventAsync(RecommendationEventRequest req, CancellationToken ct = default)
        {
            var res = await _http.PostAsJsonAsync("/api/recommendations/events", req, ct);
            res.EnsureSuccessStatusCode();
        }

        public async Task<List<ProductRefDto>> GetTrendingAsync(int limit = 12, CancellationToken ct = default)
            => await _http.GetFromJsonAsync<List<ProductRefDto>>($"/api/recommendations/trending?limit={limit}", ct) ?? new();

        public async Task<List<ProductRefDto>> GetRelatedAsync(long productId, string? category, int limit = 8, CancellationToken ct = default)
        {
            var url = $"/api/recommendations/related/{productId}?limit={limit}";
            if (!string.IsNullOrWhiteSpace(category))
                url += $"&category={Uri.EscapeDataString(category)}";

            return await _http.GetFromJsonAsync<List<ProductRefDto>>(url, ct) ?? new();
        }

        public async Task<List<ProductRefDto>> GetForYouAsync(long userId, int limit = 12, CancellationToken ct = default)
            => await _http.GetFromJsonAsync<List<ProductRefDto>>($"/api/recommendations/for-you?userId={userId}&limit={limit}", ct) ?? new();
    }
}
