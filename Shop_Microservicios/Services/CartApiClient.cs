using System.Net.Http.Json;
using Shop_Microservicios.Models.Api;

namespace Shop_Microservicios.Services
{
    public class CartApiClient
    {
        private readonly HttpClient _http;

        public CartApiClient(HttpClient http)
        {
            _http = http;
        }

        // Pequeño helper para no repetir código
        private void SetUserHeader(long userId)
        {
            _http.DefaultRequestHeaders.Remove("X-User-Id");
            _http.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());
        }

        // GET /api/cart
        public async Task<List<CartItemResponse>> GetCartAsync(long userId)
        {
            SetUserHeader(userId);

            var response = await _http.GetAsync("/api/cart");   // 👈 ruta completa

            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Error al obtener carrito. StatusCode={(int)response.StatusCode}, Body={raw}");
            }

            var result = await response.Content.ReadFromJsonAsync<List<CartItemResponse>>();
            return result ?? new List<CartItemResponse>();
        }

        // POST /api/cart/items
        public async Task<List<CartItemResponse>> AddItemAsync(long userId, long productId, int quantity)
        {
            SetUserHeader(userId);

            var request = new AddOrUpdateCartItemRequest
            {
                ProductId = productId,
                Quantity = quantity
            };

            var response = await _http.PostAsJsonAsync("/api/cart/items", request); // 👈

            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Error al añadir item. StatusCode={(int)response.StatusCode}, Body={raw}");
            }

            var result = await response.Content.ReadFromJsonAsync<List<CartItemResponse>>();
            return result ?? new List<CartItemResponse>();
        }

        // DELETE /api/cart/items/{productId}
        public async Task RemoveItemAsync(long userId, long productId)
        {
            SetUserHeader(userId);

            var response = await _http.DeleteAsync($"/api/cart/items/{productId}"); // 👈
            response.EnsureSuccessStatusCode();
        }

        // DELETE /api/cart
        public async Task ClearCartAsync(long userId)
        {
            SetUserHeader(userId);

            var response = await _http.DeleteAsync("/api/cart"); // 👈
            response.EnsureSuccessStatusCode();
        }
    }
}
