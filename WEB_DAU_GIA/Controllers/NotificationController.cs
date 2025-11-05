using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_DAU_GIA.Data;
using WEB_DAU_GIA.Models.ViewModels;

namespace WEB_DAU_GIA.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Notification/CheckNotifications
        [HttpGet]
        //public async Task<IActionResult> CheckNotifications()
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (userId == null)
        //        return Json(new { success = false, message = "Not logged in" });

        //    // Count unread notifications
        //    var unreadCount = await _context.Notifications
        //        .Where(n => n.UserId == userId && !n.IsRead)
        //        .CountAsync();

        //    return Json(new { success = true, count = unreadCount });
        //}

        //public IActionResult Index()
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (userId == null)
        //        return RedirectToAction("Login", "Account");

        //    ViewData["Title"] = "Notifications";
        //    return View();
        //}

        // GET: /Notification
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            var viewModel = new NotificationViewModel
            {
                NewNotifications = notifications.Where(n => !n.IsRead).ToList(),
                OlderNotifications = notifications.Where(n => n.IsRead).ToList(),
                UnreadCount = notifications.Count(n => !n.IsRead)
            };

            return View(viewModel);
        }

        // POST: /Notification/MarkAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequest request)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });

                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == request.NotificationId && n.UserId == userId);

                if (notification == null)
                    return Json(new { success = false, message = "Không tìm thấy thông báo" });

                notification.IsRead = true;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        // GET: /Notification/CheckNotifications (AJAX)
        [HttpGet]
        public async Task<IActionResult> CheckNotifications()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false });

            var unreadCount = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

            return Json(new { success = true, count = unreadCount });
        }

        // GET: /Notification/CheckNewNotifications (AJAX)
        [HttpGet]
        public async Task<IActionResult> CheckNewNotifications()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { hasNew = false });

            var hasNew = await _context.Notifications
                .AnyAsync(n => n.UserId == userId && !n.IsRead && n.CreatedAt > DateTime.UtcNow.AddMinutes(-1));

            return Json(new { hasNew });
        }
    }
    public class MarkAsReadRequest
    {
        public int NotificationId { get; set; }
    }
}
