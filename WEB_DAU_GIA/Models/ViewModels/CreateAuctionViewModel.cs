using System.ComponentModel.DataAnnotations;

namespace WEB_DAU_GIA.Models.ViewModels
{
    public class CreateAuctionViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [StringLength(5000, ErrorMessage = "Mô tả không được vượt quá 5000 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public string AuctionCategory { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá khởi điểm")]
        [Range(10, 1000000, ErrorMessage = "Giá khởi điểm phải từ $10 đến $1,000,000")]
        public decimal StartingBid { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Vui lòng tải lên hình ảnh chính")]
        public IFormFile? MainImage { get; set; }

        public List<AdditionalImageItem> AdditionalImages { get; set; } = new();
    }
    public class AdditionalImageItem
    {
        public IFormFile? Image { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên hình ảnh")]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ItemDescription { get; set; }
    }
}
