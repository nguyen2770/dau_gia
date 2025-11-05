using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_DAU_GIA.Models.Entities;

[Table("Users")]
public class User
{
    [Key]
    public int UserId { get; set; }

    public int RoleId { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]  // ? S?A: Mobile ph?i là string, không ph?i int
    public string Mobile { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]  // ? S?A: T?ng ?? dài cho password hash
    public string Password { get; set; } = string.Empty;

    [StringLength(255)]
    public string Token { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]  // ? S?A: Thêm Required và gi?i h?n ?? dài
    public string Address { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // ? S?A: Dùng UtcNow thay vì Now

    public DateTime? UpdatedAt { get; set; }  // ? S?A: Nullable DateTime

    // Navigation Properties
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;  // ? THÊM: Navigation property

    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public virtual ICollection<Auction> Auctions { get; set; } = new List<Auction>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();  // ? THÊM
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();  // ? THÊM
}