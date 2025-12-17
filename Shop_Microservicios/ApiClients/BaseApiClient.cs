using System.Net.Http.Json;
using Shop_Microservicios.Exceptions;

namespace Shop_Microservicios.ApiClients
{
    public abstract class BaseApiClient
    {
        protected readonly HttpClient Http;
        private readonly string _serviceName;

        protected BaseApiClient(HttpClient http, string serviceName)
        {
            Http = http;
            _serviceName = serviceName;
        }

        protected async Task<T> GetAsync<T>(string url, CancellationToken ct = default)
        {
            try
            {
                var resp = await Http.GetAsync(url, ct);

                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync(ct);
                    throw new ServiceUnavailableException(
                        $"{_serviceName} no está disponible ahora mismo.",
                        _serviceName,
                        new Exception($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase} | {body}")
                    );
                }

                return (await resp.Content.ReadFromJsonAsync<T>(cancellationToken: ct))!;
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceUnavailableException($"{_serviceName} está en mantenimiento temporal.", _serviceName, ex);
            }
        }

        protected async Task<T> SendAsync<T>(HttpRequestMessage req, CancellationToken ct = default)
        {
            try
            {
                var resp = await Http.SendAsync(req, ct);

                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync(ct);
                    throw new ServiceUnavailableException(
                        $"{_serviceName} no está disponible ahora mismo.",
                        _serviceName,
                        new Exception($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase} | {body}")
                    );
                }

                return (await resp.Content.ReadFromJsonAsync<T>(cancellationToken: ct))!;
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceUnavailableException($"{_serviceName} está en mantenimiento temporal.", _serviceName, ex);
            }
        }

        protected async Task SendAsync(HttpRequestMessage req, CancellationToken ct = default)
        {
            try
            {
                var resp = await Http.SendAsync(req, ct);

                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync(ct);
                    throw new ServiceUnavailableException(
                        $"{_serviceName} no está disponible ahora mismo.",
                        _serviceName,
                        new Exception($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase} | {body}")
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceUnavailableException($"{_serviceName} está en mantenimiento temporal.", _serviceName, ex);
            }
        }

        protected void SetUserHeader(long userId)
        {
            Http.DefaultRequestHeaders.Remove("X-User-Id");
            Http.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());
        }
    }
}
