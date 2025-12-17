using System.Net.Http;
using System.Net.Http.Json;
using Shop_Microservicios.Models.Api.Payments;

namespace Shop_Microservicios.ApiClients
{
    public class PaymentsApiClient : BaseApiClient
    {
        public PaymentsApiClient(HttpClient httpClient)
            : base(httpClient, "Pagos")
        {
        }

        public Task<PaymentResponse?> CreatePaymentAsync(long userId, CreatePaymentRequest request)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "/api/payments");
            req.Headers.Add("X-User-Id", userId.ToString());
            req.Content = JsonContent.Create(request);

            return SendAsync<PaymentResponse?>(req);
        }
    }
}
