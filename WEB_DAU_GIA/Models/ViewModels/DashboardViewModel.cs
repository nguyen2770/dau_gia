namespace WEB_DAU_GIA.Models.ViewModels
{
    public class DashboardViewModel
    {
        // KPI Data
        public int TotalAuctions { get; set; }
        public int ActiveBids { get; set; }
        public decimal TotalAuctionIncome { get; set; }
        public int CompletedAuctions { get; set; }

        // Auction Stats
        public int ActiveAuctions { get; set; }
        public int ClosedAuctions { get; set; }
        public int LiveAuctions { get; set; }
        public int RemovedFromLive { get; set; }

        // Charts Data
        public List<CategoryData> AuctionCategories { get; set; } = new();
        public List<CategoryData> AuctionTrends { get; set; } = new();

        // Tables Data
        public List<AuctionOverviewItem> AuctionOverview { get; set; } = new();
        public List<PaidAuctionItem> PaidAuctions { get; set; } = new();
        public List<ActivityItem> Activities { get; set; } = new();
    }

    public class CategoryData
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AuctionOverviewItem
    {
        public AuctionData Auction { get; set; } = new();
        public int BidCount { get; set; }
    }

    public class AuctionData
    {
        public int AuctionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? WinnerId { get; set; }
    }

    public class PaidAuctionItem
    {
        public AuctionData Auction { get; set; } = new();
        public BidData WinningBid { get; set; } = new();
    }

    public class BidData
    {
        public decimal BidAmount { get; set; }
        public string BidderName { get; set; } = string.Empty;
    }

    public class ActivityItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
