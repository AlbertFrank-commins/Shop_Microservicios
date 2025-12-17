using System.Net.Http.Json;
using Shop_Microservicios.Models.Api.Notification;

namespace Shop_Microservicios.ApiClients
{
    public class NotificationApiClient
    {
        private readonly HttpClient _http;

        public NotificationApiClient(HttpClient http)
        {
            _http = http;
        }

        // GET inbox: /api/notifications/inbox/{userId}?limit=30
        public async Task<List<NotificationDto>> GetInboxAsync(long userId, int limit = 30)
        {
            var url = $"/api/notifications/inbox/{userId}?limit={limit}";
            var data = await _http.GetFromJsonAsync<List<NotificationDto>>(url);
            return data ?? new List<NotificationDto>();
        }

        // POST mark read: /api/notifications/{id}/read
        public async Task MarkReadAsync(long notificationId)
        {
            var url = $"/api/notifications/{notificationId}/read";
            var resp = await _http.PostAsync(url, content: null);
            resp.EnsureSuccessStatusCode();
        }

        // POST event: /api/notifications/events
        public async Task<(bool created, long notificationId)> SendEventAsync(NotificationEventRequest req)
        {
            var resp = await _http.PostAsJsonAsync("/api/notifications/events", req);
            resp.EnsureSuccessStatusCode();

            var body = await resp.Content.ReadFromJsonAsync<NotificationEventResponse>();
            if (body == null) throw new Exception("Notification service returned empty body.");

            return (body.Created, body.NotificationId);
        }

        private class NotificationEventResponse
        {
            public bool Created { get; set; }
            public long NotificationId { get; set; }
        }
    }
}
