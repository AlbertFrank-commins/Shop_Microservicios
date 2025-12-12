using Shop_Microservicios.Models.Api.Shipping;
using System.Net.Http.Json;

namespace Shop_Microservicios.ApiClients;

public class ShippingApiClient
{
    private readonly HttpClient _http;

    public ShippingApiClient(HttpClient http)
    {
        _http = http;
    }

    private void SetUserHeader(long userId)
    {
        _http.DefaultRequestHeaders.Remove("X-User-Id");
        _http.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());
    }

    public async Task<ShipmentResponse?> CreateShipmentAsync(long userId, CreateShipmentRequest request)
    {
        SetUserHeader(userId);
        var resp = await _http.PostAsJsonAsync("/api/shipping", request);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<ShipmentResponse>();
    }

    public async Task<ShipmentResponse?> GetByOrderIdAsync(long userId, long orderId)
    {
        SetUserHeader(userId);
        return await _http.GetFromJsonAsync<ShipmentResponse>($"/api/shipping/order/{orderId}");
    }

    public async Task<ShipmentResponse?> GetByIdAsync(long userId, long id)
    {
        SetUserHeader(userId);
        return await _http.GetFromJsonAsync<ShipmentResponse>($"/api/shipping/{id}");
    }

    public async Task<ShipmentResponse?> UpdateStatusAsync(long userId, long id, string status)
    {
        SetUserHeader(userId);

        var body = new { status };
        var req = new HttpRequestMessage(HttpMethod.Patch, $"/api/shipping/{id}/status")
        {
            Content = JsonContent.Create(body)
        };

        var resp = await _http.SendAsync(req);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<ShipmentResponse>();
    }
}
