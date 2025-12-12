using Microsoft.AspNetCore.Mvc;
using Shop_Microservicios.ApiClients;
using Shop_Microservicios.Models.Api.Shipping;

namespace Shop_Microservicios.Controllers;

public class ShippingController : Controller
{
    private readonly ShippingApiClient _shippingClient;

    public ShippingController(ShippingApiClient shippingClient)
    {
        _shippingClient = shippingClient;
    }

    private long GetUserId()
    {
        var userIdCookie = Request.Cookies["userId"];
        if (string.IsNullOrEmpty(userIdCookie))
            throw new Exception("Usuario no autenticado (cookie userId vacía)");

        return long.Parse(userIdCookie);
    }

    // Form para crear shipping (opcional, lo usaremos desde Orders/Details)
    [HttpGet]
    public IActionResult Create(long orderId)
    {
        return View(new CreateShipmentRequest { OrderId = orderId });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateShipmentRequest request)
    {
        var userId = GetUserId();

        if (request.OrderId <= 0)
            return BadRequest("OrderId inválido");

        var created = await _shippingClient.CreateShipmentAsync(userId, request);

        TempData["Success"] = $"Envío creado. Tracking: {created?.TrackingNumber}";
        return RedirectToAction("Details", "Orders", new { id = request.OrderId });
    }

    // Ver shipping por orderId
    public async Task<IActionResult> ByOrder(long orderId)
    {
        var userId = GetUserId();
        var shipping = await _shippingClient.GetByOrderIdAsync(userId, orderId);

        if (shipping == null)
            return NotFound();

        return View("Details", shipping);
    }

    public IActionResult Details(ShipmentResponse model) => View(model);
}
