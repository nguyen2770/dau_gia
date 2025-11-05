namespace WEB_DAU_GIA.Models.ViewModels
{
    public class BidViewModel
    {
        public int BidId { get; set; }
        public int BidderId { get; set; }
        public string BidderName { get; set; } = string.Empty;
        public decimal BidAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
