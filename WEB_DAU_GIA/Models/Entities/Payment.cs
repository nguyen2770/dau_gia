using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_DAU_GIA.Models.Entities;


[Table("Payments")]
public class Payment
{
    [Key]
    public int PaymentId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int AuctionId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty; // StartingBid, AuctionPurchase

    [Required]
    [MaxLength(50)]
    public string PaymentMethod { get; set; } = "Card";

    [Required]
    [MaxLength(50)]
    public string PaymentStatus { get; set; } = "pending"; // pending, completed, failed

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("AuctionId")]
    public virtual Auction Auction { get; set; } = null!;
}
