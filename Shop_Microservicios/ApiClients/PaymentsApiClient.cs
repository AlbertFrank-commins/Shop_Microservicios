using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shop_Microservicios.Models.Api.Payments;

namespace Shop_Microservicios.ApiClients
{
    public class PaymentsApiClient
    {
        private readonly HttpClient _httpClient;

        public PaymentsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PaymentResponse?> CreatePaymentAsync(long userId, CreatePaymentRequest request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/payments");
            httpRequest.Headers.Add("X-User-Id", userId.ToString());
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaymentResponse>();
        }
    }
}
