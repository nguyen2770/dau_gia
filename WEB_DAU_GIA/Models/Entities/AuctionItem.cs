using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WEB_DAU_GIA.Models.Entities;


    [Table("AuctionItems")]
    public class AuctionItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        public int AuctionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ItemDescription { get; set; }

        [Required]
        [MaxLength(500)]
        public string ItemImage { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ItemCategory { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; } = null!;
    }

