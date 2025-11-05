namespace WEB_DAU_GIA.Models.ViewModels
{
    public class MyBidViewModel
    {
        public AuctionBidInfo Auction { get; set; } = new();
        public List<UserBidInfo> UserBids { get; set; } = new();
        public bool IsLeading { get; set; }
    }

    public class AuctionBidInfo
    {
        public int AuctionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AuctionImage { get; set; } = string.Empty;
        public string AuctionCategory { get; set; } = string.Empty;
        public decimal StartingBid { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UserBidInfo
    {
        public int BidId { get; set; }
        public decimal BidAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
