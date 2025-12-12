using Shop.Web.Models.Api;
using Shop_Microservicios.Models.Api;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Shop.Web.Services;

public class ProductsApiClient
{
    private readonly HttpClient _http;

    public ProductsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        // DummyJSON por defecto devuelve 30 productos.
        // Aquí pedimos hasta 100.
        var response =
            await _http.GetFromJsonAsync<ProductsResponse>("products?limit=100&skip=0");

        return response?.Products ?? new List<ProductDto>();
    }

    public async Task<ProductDto> GetByIdAsync(long id)
    {
        return await _http.GetFromJsonAsync<ProductDto>($"products/{id}");
    }
}
