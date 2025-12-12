using Microsoft.AspNetCore.Mvc;
using Shop.Web.Services;
using Shop_Microservicios.Models;
using Shop_Microservicios.Services;

namespace Shop_Microservicios.Controllers
{
    public class CartController : Controller
    {
        private readonly CartApiClient _cartApi;
        private readonly ProductsApiClient _productsApi;

        public CartController(CartApiClient cartApi, ProductsApiClient productsApi)
        {
            _cartApi = cartApi;
            _productsApi = productsApi;
        }

        private long? GetUserIdFromCookie()
        {
            if (Request.Cookies.TryGetValue("userId", out var value) &&
                long.TryParse(value, out var userId))
            {
                return userId;
            }
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserIdFromCookie();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _cartApi.GetCartAsync(userId.Value);
            var allProducts = await _productsApi.GetAllAsync();

            var viewModel = cartItems.Select(ci =>
            {
                var product = allProducts.FirstOrDefault(p => p.Id == ci.ProductId);

                return new CartViewItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    Title = product?.Title ?? $"Producto {ci.ProductId}",
                    Thumbnail = product?.Thumbnail
                };
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(long productId, int quantity = 1)
        {
            var userId = GetUserIdFromCookie();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            await _cartApi.AddItemAsync(userId.Value, productId, quantity);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(long productId)
        {
            var userId = GetUserIdFromCookie();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            await _cartApi.RemoveItemAsync(userId.Value, productId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var userId = GetUserIdFromCookie();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            await _cartApi.ClearCartAsync(userId.Value);

            return RedirectToAction("Index");
        }
    }
}
