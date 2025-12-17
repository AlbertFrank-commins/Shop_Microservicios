using Microsoft.AspNetCore.Mvc;
using Shop_Microservicios.ApiClients;

namespace Shop_Microservicios.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly NotificationApiClient _notifications;

        public NotificationsController(NotificationApiClient notifications)
        {
            _notifications = notifications;
        }

        // GET: /Notifications
        public async Task<IActionResult> Inbox()
        {
            if (!Request.Cookies.TryGetValue("userId", out var cookie) ||
                !long.TryParse(cookie, out var userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var notifications = await _notifications.GetInboxAsync(userId, limit: 30);
            return View(notifications);
        }

        // POST: /Notifications/Read/5
        [HttpPost]
        public async Task<IActionResult> Read(long id)
        {
            await _notifications.MarkReadAsync(id);
            return RedirectToAction(nameof(Inbox));
        }
    }
}
