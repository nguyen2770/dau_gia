using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_DAU_GIA.Data;
using WEB_DAU_GIA.Models.Entities;
using WEB_DAU_GIA.Models.ViewModels;

namespace WEB_DAU_GIA.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public PaymentController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: /Payment/MyPayments
        public async Task<IActionResult> MyPayments()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var payments = await _context.Payments
                .Include(p => p.Auction)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();

            var viewModels = payments.Select(p => new PaymentViewModel
            {
                PaymentId = p.PaymentId,
                UserId = p.UserId,
                AuctionId = p.AuctionId,
                Amount = p.Amount,
                Type = p.Type,
                PaymentMethod = p.PaymentMethod,
                PaymentStatus = p.PaymentStatus,
                UpdatedAt = p.UpdatedAt,
                Auction = new AuctionPaymentInfo
                {
                    AuctionId = p.Auction.AuctionId,
                    Title = p.Auction.Title,
                    Description = p.Auction.Description,
                    AuctionImage = p.Auction.AuctionImage,
                    AuctionCategory = p.Auction.AuctionCategory,
                    StartingBid = p.Auction.StartingBid,
                    Status = p.Auction.Status
                }
            }).ToList();

            return View(viewModels);
        }

        // GET: /Payment/DownloadReceipt
        public async Task<IActionResult> DownloadReceipt(int paymentId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var payment = await _context.Payments
                .Include(p => p.Auction)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId && p.UserId == userId);

            if (payment == null)
            {
                return NotFound();
            }

            // Generate PDF receipt (simplified version)
            var receiptContent = GenerateReceiptHtml(payment);
            var bytes = System.Text.Encoding.UTF8.GetBytes(receiptContent);

            return File(bytes, "text/html", $"Receipt_{paymentId}.html");
        }

        private string GenerateReceiptHtml(Payment payment)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Hóa đơn #{payment.PaymentId}</title>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .info {{ margin: 20px 0; }}
        .total {{ font-size: 24px; font-weight: bold; color: green; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>BidOut - Hóa đơn thanh toán</h1>
        <p>Mã giao dịch: #{payment.PaymentId}</p>
    </div>
    <div class='info'>
        <p><strong>Khách hàng:</strong> {payment.User.FirstName} {payment.User.LastName}</p>
        <p><strong>Email:</strong> {payment.User.Email}</p>
        <p><strong>Đấu giá:</strong> {payment.Auction.Title}</p>
        <p><strong>Loại:</strong> {(payment.Type == "StartingBid" ? "Phí khởi điểm" : "Mua đấu giá")}</p>
        <p><strong>Phương thức:</strong> {payment.PaymentMethod}</p>
        <p><strong>Ngày:</strong> {payment.UpdatedAt:dd/MM/yyyy HH:mm}</p>
    </div>
    <div class='total'>
        <p>Tổng tiền: ${payment.Amount}</p>
    </div>
    <p>Cảm ơn bạn đã sử dụng dịch vụ!</p>
</body>
</html>";
        }


        // GET: /Payment/Checkout?auctionId=1&type=StartingBid
        [HttpGet]
        public async Task<IActionResult> Checkout(int auctionId, string type)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var auction = await _context.Auctions.FindAsync(auctionId);
            if (auction == null)
                return NotFound();

            decimal amount = 0;
            if (type == "StartingBid")
                amount = auction.StartingBid * 0.1m;
            else if (type == "AuctionPurchase")
                amount = auction.WinningBid ?? 0;

            ViewBag.Auction = auction;
            ViewBag.Amount = amount;
            ViewBag.Type = type;
            ViewBag.UserId = userId;
            ViewBag.StripePublishableKey = _config["Stripe:PublishableKey"];
            return View();
        }

        // POST: /Payment/CreatePaymentIntent
        [HttpPost]
        public IActionResult CreatePaymentIntent([FromBody] PaymentIntentRequest req)
        {
            var amount = (long)(req.Amount * 100); // cents
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };
            var service = new PaymentIntentService();
            var intent = service.Create(options);

            return Json(new { clientSecret = intent.ClientSecret });
        }

        // POST: /Payment/Process
        [HttpPost]
        public async Task<IActionResult> Process([FromBody] PaymentProcessRequest data)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId != data.UserId)
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });

            var auction = await _context.Auctions.FindAsync(data.AuctionId);
            if (auction == null)
                return Json(new { success = false, message = "Đấu giá không tồn tại." });

            decimal expected = data.Type == "StartingBid"
                ? auction.StartingBid * 0.1m
                : auction.WinningBid ?? 0;
            if (data.Amount != expected)
                return Json(new { success = false, message = "Số tiền không hợp lệ." });

            // Lưu Payment
            var payment = new Payment
            {
                UserId = data.UserId,
                AuctionId = data.AuctionId,
                Amount = data.Amount,
                Type = data.Type,
                PaymentMethod = "card",
                PaymentStatus = "completed",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Payments.Add(payment);

            // (Có thể update quyền đặt giá hoặc trạng thái khác)
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Thanh toán thành công!" });
        }
    }

    public class PaymentIntentRequest
    {
        public decimal Amount { get; set; }
    }

    public class PaymentProcessRequest
    {
        public int UserId { get; set; }
        public int AuctionId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = "";
    }
}
