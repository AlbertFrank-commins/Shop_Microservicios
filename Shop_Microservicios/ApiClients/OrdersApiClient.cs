using System.Net.Http;
using System.Net.Http.Json;
using Shop_Microservicios.Models.Api.Orders;

namespace Shop_Microservicios.ApiClients
{
    public class OrdersApiClient : BaseApiClient
    {
        public OrdersApiClient(HttpClient httpClient)
            : base(httpClient, "Órdenes")
        {
        }

        public Task<OrderResponse?> CreateOrderAsync(long userId, CreateOrderRequest request)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "/api/orders");
            req.Headers.Add("X-User-Id", userId.ToString());
            req.Content = JsonContent.Create(request);

            return SendAsync<OrderResponse?>(req);
        }

        public Task<List<OrderResponse>> GetOrdersAsync(long userId)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/orders");
            req.Headers.Add("X-User-Id", userId.ToString());

            return SendAsync<List<OrderResponse>>(req);
        }

        public Task<OrderResponse?> GetOrderByIdAsync(long userId, long orderId)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"/api/orders/{orderId}");
            req.Headers.Add("X-User-Id", userId.ToString());

            return SendAsync<OrderResponse?>(req);
        }

        public Task<OrderResponse?> MarkOrderPaidAsync(long userId, long orderId)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, $"/api/orders/{orderId}/pay");
            req.Headers.Add("X-User-Id", userId.ToString());

            return SendAsync<OrderResponse?>(req);
        }

        public Task CancelOrderAsync(long userId, long orderId)
        {
            var req = new HttpRequestMessage(HttpMethod.Patch, $"/api/orders/{orderId}/cancel");
            req.Headers.Add("X-User-Id", userId.ToString());
            req.Content = JsonContent.Create(new { }); // body vacío

            return SendAsync(req);
        }
    }
}
