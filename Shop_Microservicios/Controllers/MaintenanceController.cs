using Microsoft.AspNetCore.Mvc;

namespace Shop_Microservicios.Controllers
{
    public class MaintenanceController : Controller
    {
        [HttpGet("/Maintenance")]
        public IActionResult Index()
        {
            ViewBag.MaintenanceMessage = HttpContext.Items["MaintenanceMessage"]?.ToString()
                ?? "Este módulo está temporalmente en mantenimiento.";

            ViewBag.MaintenanceService = HttpContext.Items["MaintenanceService"]?.ToString()
                ?? "Módulo";

            return View();
        }
    }
}
