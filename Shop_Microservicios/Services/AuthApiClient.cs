using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shop.Web.Models.Api;

namespace Shop.Web.Services;

public class AuthApiClient
{
    private readonly HttpClient _http;

    public AuthApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _http.PostAsJsonAsync("login", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }

    public async Task<AuthResponse> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        var request = new RegisterRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        };

        var response = await _http.PostAsJsonAsync("register", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }
}
