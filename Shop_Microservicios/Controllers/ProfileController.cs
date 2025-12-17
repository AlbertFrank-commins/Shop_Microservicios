using Microsoft.AspNetCore.Mvc;

namespace Shop_Microservicios.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            // Leemos de cookies (como tu auth actual)
            var userId = Request.Cookies.TryGetValue("userId", out var id) ? id : "";
            var email = Request.Cookies.TryGetValue("email", out var em) ? em : "";

            ViewBag.UserId = userId;
            ViewBag.Email = email;

            return View();
        }
    }
}
