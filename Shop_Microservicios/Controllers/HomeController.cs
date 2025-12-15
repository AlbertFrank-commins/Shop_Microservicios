using Microsoft.AspNetCore.Mvc;
using Shop.Web.Models.Api;
using Shop.Web.Services;
using Shop_Microservicios.Models;
using Shop_Microservicios.Models.ViewModel;
using Shop_Microservicios.Controllers;     // ProductsApiClient
using System.Diagnostics;

namespace Shop_Microservicios.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductsApiClient _productsApi;

        public HomeController(ILogger<HomeController> logger, ProductsApiClient productsApi)
        {
            _logger = logger;
            _productsApi = productsApi;
        }

        // 🔹 Ahora el Index trae productos de la API
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


        public IActionResult Privacy()
        {
            return View();
        }

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

            var vm = new ProductDetailsViewModel
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                Brand = product.Brand,
                Thumbnail = product.Thumbnail,
                Rating = product.Rating
            };

            return View(vm);
        }
    }
}
