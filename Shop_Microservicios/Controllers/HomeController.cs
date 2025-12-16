using Microsoft.AspNetCore.Mvc;
using Shop.Web.Models.Api;
using Shop.Web.Services; // ProductsApiClient (según tu proyecto)
using Shop_Microservicios.ApiClients; // RecommendationApiClient
using Shop_Microservicios.Models;
using Shop_Microservicios.Models.Api.Recommendation; // RecommendationEventRequest
using Shop_Microservicios.Models.ViewModel;
using System.Diagnostics;

namespace Shop_Microservicios.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductsApiClient _productsApi;
        private readonly RecommendationApiClient _recommendations;

        public HomeController(
            ILogger<HomeController> logger,
            ProductsApiClient productsApi,
            RecommendationApiClient recommendations
        )
        {
            _logger = logger;
            _productsApi = productsApi;
            _recommendations = recommendations;
        }

        // 🔹 Index con búsqueda (q) + productos
        public async Task<IActionResult> Index(string? q)
        {
            List<ProductDto> products = await _productsApi.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var query = q.Trim().ToLower();

                products = products
                    .Where(p =>
                        (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(query)) ||
                        (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(query)) ||
                        (!string.IsNullOrEmpty(p.Brand) && p.Brand.ToLower().Contains(query)) ||
                        (!string.IsNullOrEmpty(p.Category) && p.Category.ToLower().Contains(query))
                    )
                    .ToList();
            }

            ViewBag.Query = q;
            return View(products);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var product = await _productsApi.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            // 1) userId desde cookie (como dijiste que usas)
            long? userId = null;
            if (Request.Cookies.TryGetValue("userId", out var cookie) && long.TryParse(cookie, out var parsed))
                userId = parsed;

            // 2) Enviar evento VIEW (best-effort: no rompe la vista si falla recommendation)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _recommendations.SendEventAsync(new RecommendationEventRequest
                    {
                        UserId = userId,
                        EventType = "VIEW",
                        ProductId = product.Id,
                        Category = product.Category,
                        RequestId = Guid.NewGuid().ToString(),
                        Source = "MVC"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Recommendation service unavailable while sending VIEW event.");
                }
            });

            // 3) Pedir relacionados (IDs) y traer detalles reales desde Products Service
            var relatedProducts = new List<ProductDto>();

            try
            {
                var relatedRefs = await _recommendations.GetRelatedAsync(product.Id, product.Category, limit: 8);

                // Si tu ProductsApi no tiene endpoint batch, lo hacemos 1 a 1 (simple MVP)
                foreach (var r in relatedRefs)
                {
                    if (r.ProductId == product.Id) continue;

                    var p = await _productsApi.GetByIdAsync(r.ProductId);
                    if (p != null)
                        relatedProducts.Add(p);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Recommendation service unavailable while loading related products.");
                // fallback: relatedProducts se queda vacío y la vista no se rompe
            }

            // 4) Construir tu ViewModel como ya lo haces
            var vm = new ProductDetailsViewModel
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                Brand = product.Brand,
                Thumbnail = product.Thumbnail,
                Rating = product.Rating,

                // ✅ NUEVO: lista de productos relacionados para mostrar en la vista
                RelatedProducts = relatedProducts
            };

            return View(vm);
        }
    }
}
