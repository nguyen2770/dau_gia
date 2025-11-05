using WEB_DAU_GIA.Models.Entities;

namespace WEB_DAU_GIA.Models.ViewModels
{
    public class AuctionDetailsViewModel
    {
        public Auction Auction { get; set; } = new();
        public User Seller { get; set; } = new();
        public User? Winner { get; set; }
        public List<AuctionItem> AuctionItems { get; set; } = new();
        public List<BidViewModel> Bids { get; set; } = new();
        public decimal CurrentBid { get; set; }
        public decimal MinimumBid { get; set; }
    }
}
