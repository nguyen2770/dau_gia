using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WEB_DAU_GIA.Data;
using WEB_DAU_GIA.Models.ViewModels;

namespace WEB_DAU_GIA.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Get statistics
            var totalAuctions = await _context.Auctions.CountAsync(a => a.SellerId == userId);
            var wonAuctions = await _context.Auctions.CountAsync(a => a.WinnerId == userId);
            var totalBids = await _context.Bids.CountAsync(b => b.BidderId == userId);
            var totalSpent = await _context.Payments
                .Where(p => p.UserId == userId && p.PaymentStatus == "completed")
                .SumAsync(p => p.Amount);

            var viewModel = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Mobile = user.Mobile.ToString(),
                Address = user.Address,
                Email = user.Email,
                Username = user.Username,
                TotalAuctions = totalAuctions,
                WonAuctions = wonAuctions,
                TotalBids = totalBids,
                TotalSpent = totalSpent,
                MemberSince = user.CreatedAt
            };

            return View(viewModel);
        }

        // POST: /Profile/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                // Reload statistics
                var totalAuctions = await _context.Auctions.CountAsync(a => a.SellerId == userId);
                var wonAuctions = await _context.Auctions.CountAsync(a => a.WinnerId == userId);
                var totalBids = await _context.Bids.CountAsync(b => b.BidderId == userId);
                var totalSpent = await _context.Payments
                    .Where(p => p.UserId == userId && p.PaymentStatus == "completed")
                    .SumAsync(p => p.Amount);

                model.TotalAuctions = totalAuctions;
                model.WonAuctions = wonAuctions;
                model.TotalBids = totalBids;
                model.TotalSpent = totalSpent;

                return View("Index", model);
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Update user info
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Gender = model.Gender;
            user.Mobile = model.Mobile;
            user.Address = model.Address;
            user.Email = model.Email;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Update session
            HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Index");
        }

        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin.";
                return RedirectToAction("Index");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Verify current password
            var hashedCurrentPassword = HashPassword(model.CurrentPassword);
            if (user.Password != hashedCurrentPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToAction("Index");
            }

            // Update password
            user.Password = HashPassword(model.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
