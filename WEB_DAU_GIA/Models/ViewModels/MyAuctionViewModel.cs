namespace WEB_DAU_GIA.Models.ViewModels
{
    public class MyAuctionViewModel
    {
        public int AuctionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AuctionCategory { get; set; } = string.Empty;
        public decimal StartingBid { get; set; }
        public string AuctionImage { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string IsLive { get; set; } = string.Empty;
        public string DisplayStatus { get; set; } = string.Empty;
        public int BidCount { get; set; }
        public decimal? CurrentBid { get; set; }
    }
}
