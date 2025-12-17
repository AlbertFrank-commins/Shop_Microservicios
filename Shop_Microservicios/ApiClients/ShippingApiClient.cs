using System.Net.Http;
using System.Net.Http.Json;
using Shop_Microservicios.Models.Api.Shipping;

namespace Shop_Microservicios.ApiClients
{
    public class ShippingApiClient : BaseApiClient
    {
        public ShippingApiClient(HttpClient http)
            : base(http, "Envíos")
        {
        }

        public Task<ShipmentResponse?> CreateShipmentAsync(long userId, CreateShipmentRequest request)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "/api/shipping");
            req.Headers.Add("X-User-Id", userId.ToString());
            req.Content = JsonContent.Create(request);

            return SendAsync<ShipmentResponse?>(req);
        }

        public Task<ShipmentResponse?> GetByOrderIdAsync(long userId, long orderId)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"/api/shipping/order/{orderId}");
            req.Headers.Add("X-User-Id", userId.ToString());

            return SendAsync<ShipmentResponse?>(req);
        }

        public Task<ShipmentResponse?> GetByIdAsync(long userId, long id)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"/api/shipping/{id}");
            req.Headers.Add("X-User-Id", userId.ToString());

            return SendAsync<ShipmentResponse?>(req);
        }

        public Task<ShipmentResponse?> UpdateStatusAsync(long userId, long id, string status)
        {
            var body = new { status };

            var req = new HttpRequestMessage(HttpMethod.Patch, $"/api/shipping/{id}/status");
            req.Headers.Add("X-User-Id", userId.ToString());
            req.Content = JsonContent.Create(body);

            return SendAsync<ShipmentResponse?>(req);
        }
    }
}
