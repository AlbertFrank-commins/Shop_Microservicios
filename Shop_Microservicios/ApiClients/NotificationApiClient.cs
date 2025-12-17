using System.Net.Http.Json;
using System.Text.Json;
using Shop_Microservicios.Models.Api.Notification;

namespace Shop_Microservicios.ApiClients
{
    public class NotificationApiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public NotificationApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<NotificationDto>> GetInboxAsync(long userId, int limit = 30)
        {
            var url = $"/api/notifications/inbox/{userId}?limit={limit}";
            var resp = await _http.GetAsync(url);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Notification inbox failed: {(int)resp.StatusCode}\nURL: {url}\nBody: {body}");

            // 1) Intento: lista directa
            try
            {
                var list = JsonSerializer.Deserialize<List<NotificationDto>>(body, _json);
                if (list != null) return list;
            }
            catch { }

            // 2) Intento: paginado Spring => { content: [...] }
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("content", out var content))
                {
                    var list = JsonSerializer.Deserialize<List<NotificationDto>>(content.GetRawText(), _json);
                    return list ?? new List<NotificationDto>();
                }
            }
            catch { }

            throw new Exception($"Inbox JSON shape not supported.\nURL: {url}\nBody: {body}");
        }

        public async Task MarkReadAsync(long id)
        {
            var url = $"/api/notifications/{id}/read";
            var resp = await _http.PostAsync(url, content: null);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new Exception($"MarkRead failed: {(int)resp.StatusCode}\nURL: {url}\nBody: {body}");
        }
        public async Task SendEventAsync(NotificationEventRequest request)
        {
            var resp = await _http.PostAsJsonAsync(
                "/api/notifications/events",
                request
            );

            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new Exception(
                    $"SendEvent failed: {(int)resp.StatusCode}\nBody: {body}"
                );
        }

    }
}
