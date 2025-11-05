using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WEB_DAU_GIA.Models.Entities;


    [Table("Bids")]
    public class Bid
    {
        [Key]
        public int BidId { get; set; }

        [Required]
        public int AuctionId { get; set; }

        [Required]
        public int BidderId { get; set; }

        [Required]
        public decimal BidAmount { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; } = null!;

        [ForeignKey("BidderId")]
        public virtual User Bidder { get; set; } = null!;
    }

