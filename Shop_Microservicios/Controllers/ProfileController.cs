using Microsoft.AspNetCore.Mvc;

namespace Shop_Microservicios.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            if (!Request.Cookies.TryGetValue("userId", out var uid) || string.IsNullOrWhiteSpace(uid) ||
                !Request.Cookies.TryGetValue("email", out var email) || string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserId = uid;
            ViewBag.Email = email;
            return View();
        }
    }
}
