// ApiClients/OrdersApiClient.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shop_Microservicios.Models.Api.Orders;

namespace Shop_Microservicios.ApiClients
{
    public class OrdersApiClient
    {
        private readonly HttpClient _httpClient;

        public OrdersApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OrderResponse?> CreateOrderAsync(long userId, CreateOrderRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/orders");
            httpRequest.Headers.Add("X-User-Id", userId.ToString());
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OrderResponse>();
        }

        public async Task<List<OrderResponse>> GetOrdersAsync(long userId)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/orders");
            httpRequest.Headers.Add("X-User-Id", userId.ToString());

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<List<OrderResponse>>()) ?? new();
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(long userId, long orderId)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/orders/{orderId}");
            httpRequest.Headers.Add("X-User-Id", userId.ToString());

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OrderResponse>();
        }

        public async Task<OrderResponse?> MarkOrderPaidAsync(long userId, long orderId)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/orders/{orderId}/pay");

            request.Headers.Add("X-User-Id", userId.ToString());

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OrderResponse>();
        }
     

       public async Task CancelOrderAsync(long userId, long orderId)
       {
        var req = new HttpRequestMessage(HttpMethod.Patch, $"/api/orders/{orderId}/cancel");
        req.Headers.Add("X-User-Id", userId.ToString());
        req.Content = JsonContent.Create(new { }); // ✅ body vacío

        var resp = await _httpClient.SendAsync(req);
        resp.EnsureSuccessStatusCode();
       }


    }
}
