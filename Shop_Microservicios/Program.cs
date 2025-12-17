using Shop.Web.Models.Api;
using Shop.Web.Services;
using Shop_Microservicios;
using Shop_Microservicios.ApiClients;
using Shop_Microservicios.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<AuthApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:8081/api/auth/");
});

// 🔹 cart-service (PUERTO 8082, RUTA /api/cart)
builder.Services.AddHttpClient<CartApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:8082/api/cart/");
    //            └──────────── base = /api/cart/
});

builder.Services.AddHttpClient<PaymentsApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:8084"); // puerto de tu payment-service
});
// 🔹 API de productos (DummyJSON)
builder.Services.AddHttpClient<ProductsApiClient>(client =>
{
    client.BaseAddress = new Uri("https://dummyjson.com/");
});

builder.Services.AddHttpClient<OrdersApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:8083"); // puerto de orders-service
});

builder.Services.AddHttpClient<ShippingApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:8085");
});

builder.Services.AddHttpClient<Shop_Microservicios.ApiClients.RecommendationApiClient>(http =>
{
    http.BaseAddress = new Uri(builder.Configuration["Services:Recommendation"]!);
});

var notificationBaseUrl = builder.Configuration["Services:Notification"];

builder.Services.AddHttpClient<NotificationApiClient>(c =>
{
    c.BaseAddress = new Uri(notificationBaseUrl!);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
