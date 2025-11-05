using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_DAU_GIA.Models.Entities;

[Table("Auctions")]
public class Auction
{
    [Key]
    public int AuctionId { get; set; }

    [Required]
    public int SellerId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string AuctionCategory { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]  // ? THÊM: Ch? ??nh precision
    public decimal StartingBid { get; set; }

    [StringLength(500)]
    public string AuctionImage { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string IsLive { get; set; } = "no";

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "pending";

    public int? WinnerId { get; set; }

    [Column(TypeName = "decimal(18,2)")]  // ? THÊM: Ch? ??nh precision
    public decimal? WinningBid { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Not mapped - calculated field
    [NotMapped]
    public decimal CurrentBid { get; set; }

    // Navigation Properties
    [ForeignKey("SellerId")]
    public virtual User Seller { get; set; } = null!;  // ? ?Ã CÓ virtual

    [ForeignKey("WinnerId")]  // ? THÊM
    public virtual User? Winner { get; set; }

    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();  // ? THÊM virtual
    public virtual ICollection<AuctionItem> AuctionItems { get; set; } = new List<AuctionItem>();  // ? THÊM virtual
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();  // ? THÊM
}