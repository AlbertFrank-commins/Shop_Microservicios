using Microsoft.AspNetCore.Mvc;
using Shop_Microservicios.ApiClients;
using Shop_Microservicios.Models.Api.Orders;
using Shop_Microservicios.Models.Api.Payments;
using Shop_Microservicios.Models.ViewModels.Checkout;
using Shop_Microservicios.Services;
using System.Linq;

namespace Shop_Microservicios.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly CartApiClient _cartClient;
        private readonly OrdersApiClient _ordersClient;
        private readonly PaymentsApiClient _paymentsClient;

        public CheckoutController(
            CartApiClient cartClient,
            OrdersApiClient ordersClient,
            PaymentsApiClient paymentsClient)
        {
            _cartClient = cartClient;
            _ordersClient = ordersClient;
            _paymentsClient = paymentsClient;
        }

        private long GetUserId()
        {
            var userIdCookie = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userIdCookie))
                throw new System.Exception("Usuario no autenticado (cookie userId vacía)");

            return long.Parse(userIdCookie);
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

        // ✅ POST Address (NO borra, NO vuelve vacío)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Address(CheckoutAddressViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Guarda como STRING para evitar errores raros de TempData
            TempData["OrderId"] = vm.OrderId.ToString();
            TempData["FullName"] = vm.FullName;
            TempData["Email"] = vm.Email;
            TempData["Phone"] = vm.Phone;
            TempData["AddressLine1"] = vm.AddressLine1;
            TempData["City"] = vm.City;
            TempData["PostalCode"] = vm.PostalCode;

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
            // 1️⃣ Validar formulario
            if (!ModelState.IsValid)
            {
                return View("Payment", model);
            }

            var userId = GetUserId();

            // 2️⃣ Obtener la orden
            var order = await _ordersClient.GetOrderByIdAsync(userId, model.OrderId);
            if (order == null)
                return NotFound();

            // 3️⃣ Crear el request de pago
            var paymentRequest = new CreatePaymentRequest
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                Method = "CARD",
                CardNumber = model.CardNumber
            };

            // 4️⃣ Llamar al microservicio de pagos
            var payment = await _paymentsClient.CreatePaymentAsync(userId, paymentRequest);

            // 5️⃣ Guardar info para la vista Review (opcional)
            TempData["PaymentStatus"] = payment.Status ?? "";
            TempData["PaymentAmount"] = payment.Amount.ToString("0.00");
            TempData["PaymentMethod"] = payment.Method ?? "";

            return RedirectToAction("Review", "Checkout", new { orderId = order.Id });

        }

        [HttpGet]
        public async Task<IActionResult> Review(long orderId)
        {
            var userId = GetUserId();

            // Trae la orden para mostrar resumen
            var order = await _ordersClient.GetOrderByIdAsync(userId, orderId);
            if (order == null)
                return NotFound();

            // Si estás usando TempData para mostrar resultados (PaymentStatus, etc.)
            ViewBag.PaymentStatus = TempData["PaymentStatus"]?.ToString();
            ViewBag.PaymentAmount = TempData["PaymentAmount"]?.ToString();
            ViewBag.PaymentMethod = TempData["PaymentMethod"]?.ToString();


            return View(order); // Renderiza Views/Checkout/Review.cshtml
        }

        // (Pay/Review lo ajustamos después)
    }
}
