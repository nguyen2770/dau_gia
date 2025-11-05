using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_DAU_GIA.Models.Entities;

[Table("Roles")]
public class Role
{
    [Key]
    public int RoleId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;  // ❌ SỬA: Không nullable

    [StringLength(200)]
    public string? Description { get; set; }  // ❌ THÊM: Mô tả role

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public virtual ICollection<User> Users { get; set; } = new List<User>();  // ❌ THÊM
}