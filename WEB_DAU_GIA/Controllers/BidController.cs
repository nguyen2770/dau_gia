using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WEB_DAU_GIA.Data;
using WEB_DAU_GIA.Models.Entities;
using WEB_DAU_GIA.Models.ViewModels;

namespace WEB_DAU_GIA.Controllers
{
    public class BidController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public BidController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;

        }

        // POST: /Bid/PlaceBid
        [HttpPost]
        public async Task<IActionResult> PlaceBid([FromBody] PlaceBidRequest request)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });

                // Validate auction
                var auction = await _context.Auctions
                    .Include(a => a.Bids)
                    .FirstOrDefaultAsync(a => a.AuctionId == request.AuctionId);

                if (auction == null)
                    return Json(new { success = false, message = "Không tìm thấy đấu giá" });

                if (auction.Status != "active")
                    return Json(new { success = false, message = "Đấu giá đã kết thúc" });

                // Check if user is seller
                if (auction.SellerId == userId)
                    return Json(new { success = false, message = "Bạn không thể đặt giá cho đấu giá của mình" });

                // Calculate minimum bid
                var currentBid = auction.Bids.Any()
                    ? auction.Bids.Max(b => b.BidAmount)
                    : auction.StartingBid;
                var minimumBid = currentBid + 10;

                if (request.BidAmount < minimumBid)
                    return Json(new { success = false, message = $"Giá thầu phải ít nhất ${minimumBid}" });

                // Check if user has paid starting bid fee
                var hasStartingBidPayment = await _context.Payments
                    .AnyAsync(p => p.UserId == userId
                        && p.AuctionId == request.AuctionId
                        && p.Type == "StartingBid"
                        && p.PaymentStatus == "completed");

                if (!hasStartingBidPayment)
                {
                    // Redirect to payment
                    return Json(new
                    {
                        success = false,
                        redirectToPayment = true,
                        auctionId = request.AuctionId,
                        type = "StartingBid",
                        message = "Bạn cần thanh toán phí trước để đặt giá thầu"
                    });
                }

                // Create bid
                var bid = new Bid
                {
                    AuctionId = request.AuctionId,
                    BidderId = userId.Value,
                    BidAmount = request.BidAmount,
                    Status = "active",
                    //CreatedAt = DateTime.UtcNow
                };

                _context.Bids.Add(bid);
                await _context.SaveChangesAsync();

                // Create notification for seller
                var notification = new Notification
                {
                    UserId = auction.SellerId,
                    Title = "Giá thầu mới",
                    Message = $"Có giá thầu mới ${request.BidAmount} cho đấu giá '{auction.Title}'",
                    Link = $"/auction/{auction.AuctionId}",
                    IsRead = false,
                    //CreatedAt = DateTime.UtcNow
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đặt giá thầu thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing bid");
                return Json(new { success = false, message = "Đã có lỗi xảy ra" });
            }
        }
        // GET: /Bid/MyBids
        public async Task<IActionResult> MyBids()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get all auctions where user has placed bids
            var userBids = await _context.Bids
                .Include(b => b.Auction)
                .Where(b => b.BidderId == userId)
                .GroupBy(b => b.AuctionId)
                .Select(g => new
                {
                    AuctionId = g.Key,
                    Auction = g.First().Auction,
                    Bids = g.OrderByDescending(b => b.CreatedAt).ToList()
                })
                .ToListAsync();

            var viewModels = new List<MyBidViewModel>();

            foreach (var item in userBids)
            {
                // Get highest bid for this auction
                var highestBid = await _context.Bids
                    .Where(b => b.AuctionId == item.AuctionId)
                    .OrderByDescending(b => b.BidAmount)
                    .FirstOrDefaultAsync();

                var isLeading = highestBid?.BidderId == userId;

                viewModels.Add(new MyBidViewModel
                {
                    Auction = new AuctionBidInfo
                    {
                        AuctionId = item.Auction.AuctionId,
                        Title = item.Auction.Title,
                        Description = item.Auction.Description,
                        AuctionImage = item.Auction.AuctionImage,
                        AuctionCategory = item.Auction.AuctionCategory,
                        StartingBid = item.Auction.StartingBid,
                        Status = item.Auction.Status
                    },
                    UserBids = item.Bids.Select(b => new UserBidInfo
                    {
                        BidId = b.BidId,
                        BidAmount = b.BidAmount,
                        Status = b.Status,
                        CreatedAt = b.CreatedAt
                    }).ToList(),
                    IsLeading = isLeading
                });
            }

            return View(viewModels.OrderByDescending(v => v.UserBids.Max(b => b.CreatedAt)).ToList());
        }
    }

    public class PlaceBidRequest
    {
        public int AuctionId { get; set; }
        public int BidderId { get; set; }
        public decimal BidAmount { get; set; }
        public string Status { get; set; } = "active";
    }
}
