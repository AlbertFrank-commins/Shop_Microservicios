namespace Shop.Web.Models.Api;

public class AuthResponse
{
    public string Token { get; set; }
    public long UserId { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
