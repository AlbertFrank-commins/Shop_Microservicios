using System.Net;
using Shop_Microservicios.Exceptions;

namespace Shop_Microservicios.Middleware
{
    public class ServiceUnavailableMiddleware
    {
        private readonly RequestDelegate _next;

        public ServiceUnavailableMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ServiceUnavailableException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

                context.Items["MaintenanceMessage"] = ex.Message;
                context.Items["MaintenanceService"] = ex.ServiceName ?? "Módulo";

                // Render vista Razor
                context.Request.Path = "/Maintenance";
                await _next(context);
            }
        }
    }
}
