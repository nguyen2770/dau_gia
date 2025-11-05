namespace WEB_DAU_GIA.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public int AuctionId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public AuctionPaymentInfo Auction { get; set; } = new();
    }

    public class AuctionPaymentInfo
    {
        public int AuctionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AuctionImage { get; set; } = string.Empty;
        public string AuctionCategory { get; set; } = string.Empty;
        public decimal StartingBid { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
