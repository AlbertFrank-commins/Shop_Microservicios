using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Shop.Web.Services;
using Shop_Microservicios.Models;

namespace Shop.Web.Controllers;

public class AccountController : Controller
{
    private readonly AuthApiClient _authApi;

    public AccountController(AuthApiClient authApi)
    {
        _authApi = authApi;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try
        {
            var auth = await _authApi.LoginAsync(model.Email, model.Password);

            Response.Cookies.Append("jwt", auth.Token);
            Response.Cookies.Append("userId", auth.UserId.ToString());

            // 👇 Aquí decides a dónde ir después del login
            return RedirectToAction("Index", "Home");
            // o: return RedirectToAction("Index", "Products");
        }
        catch
        {
            ViewBag.Error = "Credenciales inválidas";
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string firstName, string lastName, string email, string password)
    {
        try
        {
            var auth = await _authApi.RegisterAsync(firstName, lastName, email, password);

            Response.Cookies.Append("jwt", auth.Token);
            Response.Cookies.Append("userId", auth.UserId.ToString());

            return RedirectToAction("Index", "Home");
        }
        catch
        {
            ViewBag.Error = "Error al registrar (quizá email ya existe).";
            return View();
        }
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        Response.Cookies.Delete("userId");

        return RedirectToAction("Index", "Home");
    }
}
