using System.Net.Http;
using System.Net.Http.Json;
using Shop_Microservicios.Models.Api.Notification;

namespace Shop_Microservicios.ApiClients
{
    public class NotificationApiClient : BaseApiClient
    {
        private readonly string _apiKey;

        public NotificationApiClient(HttpClient http, string apiKey)
            : base(http, "Notificaciones")
        {
            _apiKey = apiKey;

            // Si tu servicio usa apiKey por header
            if (!string.IsNullOrWhiteSpace(_apiKey))
            {
                Http.DefaultRequestHeaders.Remove("X-Api-Key");
                Http.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
            }
        }

        public Task<List<NotificationDto>> GetInboxAsync(long userId, int limit = 30, CancellationToken ct = default)
        {
            var url = $"/api/notifications/inbox/{userId}?limit={limit}";
            return GetAsync<List<NotificationDto>>(url, ct);
        }

        public Task MarkReadAsync(long notificationId, CancellationToken ct = default)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, $"/api/notifications/{notificationId}/read");
            return SendAsync(req, ct);
        }

        // 👇 ESTE ES EL QUE TU Checkout YA ESTÁ LLAMANDO
        public Task SendEventAsync(NotificationEventRequest request, CancellationToken ct = default)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "/api/notifications/events")
            {
                Content = JsonContent.Create(request)
            };

            return SendAsync(req, ct);
        }
    }
}
