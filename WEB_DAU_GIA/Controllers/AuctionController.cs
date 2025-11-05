using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WEB_DAU_GIA.Data;
using WEB_DAU_GIA.Models.Entities;
using WEB_DAU_GIA.Models.ViewModels;

namespace WEB_DAU_GIA.Controllers
{
    public class AuctionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuctionController> _logger;
        // Add IWebHostEnvironment to constructor
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuctionController(ApplicationDbContext context,
                                ILogger<AuctionController> logger,
                                IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Auction
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy tất cả đấu giá đang active
            var auctions = await _context.Auctions
                .Include(a => a.Seller)
                .Include(a => a.Bids)
                .Where(a => a.Status == "active" || a.Status == "closed")
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            // Tính CurrentBid cho mỗi auction
            foreach (var auction in auctions)
            {
                if (auction.Bids.Any())
                {
                    auction.CurrentBid = auction.Bids.Max(b => b.BidAmount);
                }
                else
                {
                    auction.CurrentBid = auction.StartingBid;
                }
            }

            return View(auctions);
        }

        // AJAX GET: Lấy danh sách đấu giá (cho refresh)
        [HttpGet]
        public async Task<IActionResult> GetAuctions()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false, message = "Not logged in" });

            var auctions = await _context.Auctions
                .Include(a => a.Bids)
                .Where(a => a.Status == "active")
                .Select(a => new
                {
                    a.AuctionId,
                    a.Title,
                    a.Description,
                    a.AuctionImage,
                    a.AuctionCategory,
                    a.StartingBid,
                    a.Status,
                    CurrentBid = a.Bids.Any() ? a.Bids.Max(b => b.BidAmount) : a.StartingBid,
                    BidCount = a.Bids.Count
                })
                .ToListAsync();

            return Json(new { success = true, data = auctions });
        }
        // GET: /Auction/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var auction = await _context.Auctions
                .Include(a => a.Seller)
                .Include(a => a.Bids)
                    .ThenInclude(b => b.Bidder)
                .Include(a => a.AuctionItems)
                .FirstOrDefaultAsync(a => a.AuctionId == id);

            if (auction == null)
            {
                return NotFound();
            }

            // Get winner if exists
            User? winner = null;
            if (auction.WinnerId != null)
            {
                winner = await _context.Users.FindAsync(auction.WinnerId);
            }

            // Calculate current bid
            var currentBid = auction.Bids.Any()
                ? auction.Bids.Max(b => b.BidAmount)
                : auction.StartingBid;

            // Calculate minimum bid (current + $10)
            var minimumBid = currentBid + 10;

            var viewModel = new AuctionDetailsViewModel
            {
                Auction = auction,
                Seller = auction.Seller,
                Winner = winner,
                AuctionItems = auction.AuctionItems.ToList(),
                Bids = auction.Bids.Select(b => new BidViewModel
                {
                    BidId = b.BidId,
                    BidderId = b.BidderId,
                    BidderName = $"{b.Bidder.FirstName} {b.Bidder.LastName}",
                    BidAmount = b.BidAmount,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt
                }).ToList(),
                CurrentBid = currentBid,
                MinimumBid = minimumBid
            };

            return View(viewModel);
        }
        // GET: /Auction/MyAuctions
        public async Task<IActionResult> MyAuctions()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var auctions = await _context.Auctions
                .Include(a => a.Bids)
                .Where(a => a.SellerId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var viewModels = auctions.Select(a => {
                var now = DateTime.UtcNow;
                var auctionEndDate = a.EndTime;
                var isActive = a.Status == "active" && auctionEndDate > now;

                string displayStatus;
                if (a.Status == "pending")
                {
                    displayStatus = "pending";
                }
                else if (a.Status == "closed")
                {
                    displayStatus = "closed";
                }
                else if (isActive)
                {
                    displayStatus = "active";
                }
                else
                {
                    displayStatus = "closed";
                }

                return new MyAuctionViewModel
                {
                    AuctionId = a.AuctionId,
                    Title = a.Title,
                    Description = a.Description,
                    AuctionCategory = a.AuctionCategory,
                    StartingBid = a.StartingBid,
                    AuctionImage = a.AuctionImage,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status,
                    IsLive = a.IsLive,
                    DisplayStatus = displayStatus,
                    BidCount = a.Bids.Count,
                    CurrentBid = a.Bids.Any() ? a.Bids.Max(b => b.BidAmount) : (decimal?)null
                };
            }).ToList();

            return View(viewModels);
        }

        // POST: /Auction/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });

                var auction = await _context.Auctions.FindAsync(request.AuctionId);
                if (auction == null)
                    return Json(new { success = false, message = "Không tìm thấy đấu giá" });

                // Check ownership
                if (auction.SellerId != userId)
                    return Json(new { success = false, message = "Bạn không có quyền cập nhật đấu giá này" });

                // Update IsLive status
                auction.IsLive = request.IsLive;
                auction.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Auction {auction.AuctionId} IsLive updated to {request.IsLive} by user {userId}");

                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating auction status");
                return Json(new { success = false, message = "Đã có lỗi xảy ra" });
            }
        }

        public class UpdateStatusRequest
        {
            public int AuctionId { get; set; }
            public string IsLive { get; set; } = string.Empty;
        }

        // GET: /Auction/Create
        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new CreateAuctionViewModel());
        }

        // POST: /Auction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuctionViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Validate dates
                if (model.StartTime < DateTime.UtcNow)
                {
                    ModelState.AddModelError("StartTime", "Ngày bắt đầu phải sau thời điểm hiện tại");
                    return View(model);
                }

                if (model.EndTime <= model.StartTime)
                {
                    ModelState.AddModelError("EndTime", "Ngày kết thúc phải sau ngày bắt đầu");
                    return View(model);
                }

                // Upload main image
                string mainImageUrl = string.Empty;
                if (model.MainImage != null)
                {
                    mainImageUrl = await UploadImage(model.MainImage);
                }

                // Create auction
                var auction = new Auction
                {
                    SellerId = userId.Value,
                    Title = model.Title,
                    Description = model.Description,
                    AuctionCategory = model.AuctionCategory,
                    StartingBid = model.StartingBid,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    AuctionImage = mainImageUrl,
                    Status = "pending",
                    IsLive = "no",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Auctions.Add(auction);
                await _context.SaveChangesAsync();

                // Upload additional images
                foreach (var additionalImage in model.AdditionalImages)
                {
                    if (additionalImage.Image != null)
                    {
                        var imageUrl = await UploadImage(additionalImage.Image);

                        var auctionItem = new AuctionItem
                        {
                            AuctionId = auction.AuctionId,
                            ItemName = additionalImage.ItemName,
                            ItemDescription = additionalImage.ItemDescription,
                            ItemImage = imageUrl,
                            ItemCategory = model.AuctionCategory,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.AuctionItems.Add(auctionItem);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Auction {auction.AuctionId} created by user {userId}");

                TempData["SuccessMessage"] = "Tạo đấu giá thành công! Đấu giá của bạn đang chờ được phê duyệt.";
                return RedirectToAction("Details", new { id = auction.AuctionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating auction");
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra. Vui lòng thử lại.";
                return View(model);
            }
        }

        // Helper: Upload image
        private async Task<string> UploadImage(IFormFile file)
        {
            try
            {
                // Validate file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    throw new Exception("Chỉ chấp nhận file ảnh (JPG, PNG, GIF)");
                }

                if (file.Length > 5 * 1024 * 1024) // 5MB
                {
                    throw new Exception("Kích thước file không được vượt quá 5MB");
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "auctions");

                // Create directory if not exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return relative URL
                return $"/uploads/auctions/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                throw;
            }
        }

    }
}
