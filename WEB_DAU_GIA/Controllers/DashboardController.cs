using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_DAU_GIA.Data;
using WEB_DAU_GIA.Models.ViewModels;

namespace WEB_DAU_GIA.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Check authentication
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = await GetDashboardData(userId.Value);
            return View(viewModel);
        }

        // AJAX endpoint
        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false, message = "Not logged in" });

            var data = await GetDashboardData(userId.Value);
            return Json(new { success = true, data });
        }

        private async Task<DashboardViewModel> GetDashboardData(int userId)
        {
            var now = DateTime.UtcNow;

            // Get all auctions for the user
            var userAuctions = await _context.Auctions
                .Include(a => a.Bids)
                .Where(a => a.SellerId == userId)
                .ToListAsync();

            // KPI Data
            var totalAuctions = userAuctions.Count;
            var activeBids = await _context.Bids
                .Where(b => b.Auction.SellerId == userId && b.Status == "active")
                .CountAsync();

            var completedAuctions = userAuctions.Count(a => a.Status == "closed");

            var totalIncome = await _context.Payments
                .Where(p => p.Auction.SellerId == userId && p.PaymentStatus == "completed")
                .SumAsync(p => p.Amount);

            // Auction Stats
            var activeAuctions = userAuctions.Count(a => a.Status == "active");
            var closedAuctions = userAuctions.Count(a => a.Status == "closed");
            var liveAuctions = userAuctions.Count(a => a.IsLive == "yes");
            var removedFromLive = userAuctions.Count(a => a.IsLive == "no" && a.Status == "active");

            // Auction Categories (for charts)
            var categories = userAuctions
                .GroupBy(a => a.AuctionCategory)
                .Select(g => new CategoryData
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Auction Overview
            var auctionOverview = userAuctions
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new AuctionOverviewItem
                {
                    Auction = new AuctionData
                    {
                        AuctionId = a.AuctionId,
                        Title = a.Title,
                        Status = a.Status,
                        WinnerId = a.WinnerId
                    },
                    BidCount = a.Bids.Count
                })
                .ToList();

            // Paid Auctions
            var paidAuctions = await _context.Payments
                .Include(p => p.Auction)
                .Include(p => p.Auction.Bids)
                .Where(p => p.Auction.SellerId == userId && p.Type == "AuctionPurchase")
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new PaidAuctionItem
                {
                    Auction = new AuctionData
                    {
                        AuctionId = p.Auction.AuctionId,
                        Title = p.Auction.Title,
                        Status = p.Auction.Status
                    },
                    WinningBid = new BidData
                    {
                        BidAmount = p.Auction.WinningBid ?? 0,
                        BidderName = p.User.FirstName + " " + p.User.LastName
                    }
                })
                .ToListAsync();

            // Recent Activities (Notifications)
            var activities = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .Select(n => new ActivityItem
                {
                    Id = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    Link = n.Link,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalAuctions = totalAuctions,
                ActiveBids = activeBids,
                TotalAuctionIncome = totalIncome,
                CompletedAuctions = completedAuctions,
                ActiveAuctions = activeAuctions,
                ClosedAuctions = closedAuctions,
                LiveAuctions = liveAuctions,
                RemovedFromLive = removedFromLive,
                AuctionCategories = categories,
                AuctionTrends = categories, // Same data for now
                AuctionOverview = auctionOverview,
                PaidAuctions = paidAuctions,
                Activities = activities
            };
        }
    }
}
