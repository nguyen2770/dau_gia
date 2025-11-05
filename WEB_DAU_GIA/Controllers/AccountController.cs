using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WEB_DAU_GIA.Data;
using WEB_DAU_GIA.Models.Entities;
using WEB_DAU_GIA.Models.ViewModels;

namespace WEB_DAU_GIA.Controllers
{
    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Username);

                if (user == null)
                {
                    ViewBag.Message = "Tên đăng nhập hoặc mật khẩu không đúng.";
                    ViewBag.MessageOk = false;
                    return View(model);
                }

                var hashedPassword = HashPassword(model.Password);
                if (user.Password != hashedPassword)
                {
                    ViewBag.Message = "Tên đăng nhập hoặc mật khẩu không đúng.";
                    ViewBag.MessageOk = false;
                    return View(model);
                }

                // Set session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetString("Email", user.Email);

                _logger.LogInformation($"User {user.Username} logged in successfully at {DateTime.UtcNow}");

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewBag.Message = "Đã xảy ra lỗi. Vui lòng thử lại.";
                ViewBag.MessageOk = false;
                return View(model);
            }
        }

        // GET: /Account/Signup
        [HttpGet]
        public IActionResult Signup()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        // POST: /Account/Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Validate username
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ViewBag.Message = "Tên đăng nhập đã tồn tại.";
                    ViewBag.MessageOk = false;
                    return View(model);
                }

                // Validate email
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ViewBag.Message = "Email đã được sử dụng.";
                    ViewBag.MessageOk = false;
                    return View(model);
                }

                // Create new user
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Username = model.Username,
                    Gender = model.Gender,
                    Mobile = model.Mobile,
                    Address = model.Address,
                    Password = HashPassword(model.Password),
                    RoleId = 2,
                    //CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Set session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                HttpContext.Session.SetString("Email", user.Email);

                _logger.LogInformation($"New user {user.Username} registered successfully at {DateTime.UtcNow}");

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during signup");
                ViewBag.Message = $"Đã xảy ra lỗi. Vui lòng thử lại. {ex.Message}";
                ViewBag.MessageOk = false;
                return View(model);
            }
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            _logger.LogInformation($"User {userId} logged out at {DateTime.UtcNow}");

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Helper method: Hash password với SHA256
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
