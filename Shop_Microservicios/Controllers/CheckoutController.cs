using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop_Microservicios.ApiClients;
using Shop_Microservicios.Models.Api.Notification;
using Shop_Microservicios.Models.Api.Orders;
using Shop_Microservicios.Models.Api.Payments;
using Shop_Microservicios.Models.ViewModels.Checkout;
using Shop_Microservicios.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop_Microservicios.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly CartApiClient _cartClient;
        private readonly OrdersApiClient _ordersClient;
        private readonly PaymentsApiClient _paymentsClient;
        private readonly NotificationApiClient _notificationApi;

        public CheckoutController(
            CartApiClient cartClient,
            OrdersApiClient ordersClient,
            PaymentsApiClient paymentsClient,
            NotificationApiClient notificationApi)
        {
            _cartClient = cartClient;
            _ordersClient = ordersClient;
            _paymentsClient = paymentsClient;
            _notificationApi = notificationApi;
        }

        private long GetUserId()
        {
            var userIdCookie = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userIdCookie))
                throw new Exception("Usuario no autenticado (cookie userId vacía)");

            return long.Parse(userIdCookie);
        }

        private string? GetUserEmail()
        {
            // Prioridad: cookie persistida por Login/Address
            if (Request.Cookies.TryGetValue("email", out var email) && !string.IsNullOrWhiteSpace(email))
                return email.Trim();

            // Fallback: TempData (si venía de Address)
            return TempData["Email"]?.ToString();
        }

        // START: crea orden y manda a Address
        public async Task<IActionResult> Start()
        {
            var userId = GetUserId();

            var cartItems = await _cartClient.GetCartAsync(userId);
            if (cartItems == null || !cartItems.Any())
                return RedirectToAction("Index", "Cart");

            var request = new CreateOrderRequest
            {
                Items = cartItems.Select(i => new OrderItemRequest
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var order = await _ordersClient.CreateOrderAsync(userId, request);

            await _cartClient.ClearCartAsync(userId);

            return RedirectToAction(nameof(Address), new { orderId = order!.Id });
        }

        // ✅ GET Address
        [HttpGet]
        public IActionResult Address(long orderId)
        {
            return View(new CheckoutAddressViewModel { OrderId = orderId });
        }

        // ✅ POST Address
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Address(CheckoutAddressViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            TempData["OrderId"] = vm.OrderId.ToString();
            TempData["FullName"] = vm.FullName;
            TempData["Email"] = vm.Email;
            TempData["Phone"] = vm.Phone;
            TempData["AddressLine1"] = vm.AddressLine1;
            TempData["City"] = vm.City;
            TempData["PostalCode"] = vm.PostalCode;

            // ✅ Persistir email para Pay()
            if (!string.IsNullOrWhiteSpace(vm.Email))
            {
                Response.Cookies.Append(
                    "email",
                    vm.Email.Trim(),
                    new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    }
                );
            }

            return RedirectToAction(nameof(Payment), new { orderId = vm.OrderId });
        }

        // ✅ GET Payment
        [HttpGet]
        public async Task<IActionResult> Payment(long orderId)
        {
            var userId = GetUserId();
            var order = await _ordersClient.GetOrderByIdAsync(userId, orderId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(CheckoutPaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Payment", model);

            var userId = GetUserId();

            var order = await _ordersClient.GetOrderByIdAsync(userId, model.OrderId);
            if (order == null)
                return NotFound();

            var paymentRequest = new CreatePaymentRequest
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                Method = "CARD",
                CardNumber = model.CardNumber
            };

            var payment = await _paymentsClient.CreatePaymentAsync(userId, paymentRequest);

            TempData["PaymentStatus"] = payment.Status ?? "";
            TempData["PaymentAmount"] = payment.Amount.ToString("0.00");
            TempData["PaymentMethod"] = payment.Method ?? "";

            // ✅ Determinar si se considera pagado (MVP robusto)
            var status = (payment.Status ?? "").Trim().ToUpperInvariant();
            bool paid = status != "FAILED" && status != "CANCELLED" && status != "DECLINED";

            if (paid)
            {
                var email = GetUserEmail();

                // ✅ 1) IN_APP siempre (best-effort)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationApi.SendEventAsync(new NotificationEventRequest
                        {
                            UserId = (int)userId, // 👈 FIX
                            Type = "PAYMENT_SUCCESS",
                            Title = "Pago aprobado ✅",
                            Message = $"Tu orden #{order.Id} fue pagada correctamente.",
                            Channel = "IN_APP",
                            RequestId = $"payok-{order.Id}-{Guid.NewGuid():N}",
                            Source = "MVC"
                        });
                        ;
                    }
                    catch { }
                });

                // ✅ 2) EMAIL solo si hay correo
                if (!string.IsNullOrWhiteSpace(email))
                {
                    string receiptHtml = BuildReceiptHtml(
                        orderId: order.Id,
                        total: order.TotalAmount,
                        paymentMethod: payment.Method ?? "CARD",
                        customerEmail: email
                    );

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _notificationApi.SendEventAsync(new NotificationEventRequest
                            {
                                UserId = (int)userId,
                                Type = "RECEIPT_EMAIL",
                                Title = "Recibo de tu compra",
                                Message = receiptHtml,
                                Channel = "EMAIL",
                                Email = email,
                                RequestId = $"receipt-{order.Id}-{Guid.NewGuid():N}",
                                Source = "MVC"
                            });
                        }
                        catch { }
                    });
                }
            }

            return RedirectToAction("Review", "Checkout", new { orderId = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Review(long orderId)
        {
            var userId = GetUserId();

            var order = await _ordersClient.GetOrderByIdAsync(userId, orderId);
            if (order == null)
                return NotFound();

            ViewBag.PaymentStatus = TempData["PaymentStatus"]?.ToString();
            ViewBag.PaymentAmount = TempData["PaymentAmount"]?.ToString();
            ViewBag.PaymentMethod = TempData["PaymentMethod"]?.ToString();

            return View(order);
        }

        private static string BuildReceiptHtml(long orderId, decimal total, string paymentMethod, string customerEmail)
        {
            return $@"
<div style='font-family:Arial,sans-serif;max-width:640px;margin:auto;border:1px solid #eee;border-radius:10px;overflow:hidden;'>
  <div style='background:#0d6efd;color:white;padding:16px;'>
    <h2 style='margin:0;'>Shop - Recibo de compra</h2>
  </div>

  <div style='padding:16px;'>
    <p style='margin-top:0;'>Gracias por tu compra.</p>

    <table style='width:100%;border-collapse:collapse;'>
      <tr><td style='padding:8px 0;'><b>Orden</b></td><td style='padding:8px 0;text-align:right;'>#{orderId}</td></tr>
      <tr><td style='padding:8px 0;'><b>Total</b></td><td style='padding:8px 0;text-align:right;'><b>{total:C}</b></td></tr>
      <tr><td style='padding:8px 0;'><b>Método</b></td><td style='padding:8px 0;text-align:right;'>{paymentMethod}</td></tr>
      <tr><td style='padding:8px 0;'><b>Email</b></td><td style='padding:8px 0;text-align:right;'>{customerEmail}</td></tr>
      <tr><td style='padding:8px 0;'><b>Fecha</b></td><td style='padding:8px 0;text-align:right;'>{DateTime.Now:dd/MM/yyyy HH:mm}</td></tr>
    </table>

    <hr style='border:none;border-top:1px solid #eee;margin:16px 0;' />

    <p style='color:#666;margin:0;font-size:12px;'>
      Este correo confirma tu compra. Si no reconoces esta transacción, contacta soporte.
    </p>
  </div>
</div>";
        }
    }
}
