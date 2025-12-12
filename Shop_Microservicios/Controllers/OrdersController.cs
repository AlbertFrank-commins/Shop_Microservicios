using Microsoft.AspNetCore.Mvc;
using Shop.Web.Services;
using Shop_Microservicios.ApiClients;
using Shop_Microservicios.Models.Api.Orders;
using Shop_Microservicios.Services;
using Shop_Microservicios.Models.Api.Payments;
using System.Linq;
using System.Threading.Tasks;
using Shop_Microservicios.Models.ViewModels.Orders;

namespace Shop_Microservicios.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrdersApiClient _ordersClient;
        private readonly CartApiClient _cartClient;
        private readonly ProductsApiClient _productsClient;
        private readonly PaymentsApiClient _paymentsClient;// 👈 nuevo
        public OrdersController(OrdersApiClient ordersClient, CartApiClient cartClient, ProductsApiClient productsClient, PaymentsApiClient paymentsClient)
        {
            _ordersClient = ordersClient;
            _cartClient = cartClient;
            _productsClient = productsClient;
            _paymentsClient = paymentsClient;
        }

        private long GetUserId()
        {
            var userIdCookie = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userIdCookie))
            {
                throw new System.Exception("Usuario no autenticado (cookie userId vacía)");
            }

            return long.Parse(userIdCookie);
        }

        // 1) Crear una orden a partir del carrito
        public async Task<IActionResult> CreateFromCart()
        {
            var userId = GetUserId();

            var cartItems = await _cartClient.GetCartAsync(userId);

            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

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

            return RedirectToAction("Details", new { id = order!.Id });
        }

        // 2) Listar órdenes del usuario
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var orders = await _ordersClient.GetOrdersAsync(userId);
            return View(orders);
        }

        // 3) Detalle de una orden
        public async Task<IActionResult> Details(long id)
        {
            var userId = GetUserId();
            var order = await _ordersClient.GetOrderByIdAsync(userId, id);

            if (order == null)
                return NotFound();

            var vm = new OrderDetailsViewModel
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount
            };

            foreach (var item in order.Items)
            {
                // 👇 Aquí llamamos a tu ProductsApiClient
                var product = await _productsClient.GetByIdAsync(item.ProductId);

                vm.Items.Add(new OrderDetailsItemViewModel
                {
                    ProductId = item.ProductId,
                    Title = product?.Title ?? $"Producto #{item.ProductId}",
                    Thumbnail = product?.Thumbnail ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.LineTotal
                });
            }
            ViewBag.PaymentStatus = TempData["PaymentStatus"];
            ViewBag.PaymentAmount = TempData["PaymentAmount"];
            ViewBag.PaymentMethod = TempData["PaymentMethod"];

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Pay(long id, string method, string cardNumber)
        {
            var userId = GetUserId();

            // 1. Traer la orden
            var order = await _ordersClient.GetOrderByIdAsync(userId, id);
            if (order == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(method))
                method = "CARD";

            if (string.IsNullOrWhiteSpace(cardNumber))
                cardNumber = "4111111111111112"; // demo

            // 2. Crear request de pago
            var request = new CreatePaymentRequest
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                Method = method,
                CardNumber = cardNumber
            };

            // 3. Llamar al microservicio de pagos
            var payment = await _paymentsClient.CreatePaymentAsync(userId, request);

            if (payment != null)
            {
                TempData["PaymentStatus"] = payment.Status;
                TempData["PaymentAmount"] = payment.Amount.ToString("C");
                TempData["PaymentMethod"] = payment.Method;

                // ⭐ Si el pago fue APROBADO, marcamos la orden como PAID
                if (payment.Status.Equals("APPROVED", System.StringComparison.OrdinalIgnoreCase))
                {
                    await _ordersClient.MarkOrderPaidAsync(userId, id);
                }
            }

            return RedirectToAction("Details", new { id });
        }



    }
}
