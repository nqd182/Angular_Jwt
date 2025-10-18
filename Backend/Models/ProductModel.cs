    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jwt.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Tên sản phẩm không được để trống")]
        [MaxLength(50, ErrorMessage ="Tối đa 50 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Đánh giá không được để trống")]
        [MaxLength(250, ErrorMessage = "Tối đa 250 ký tự")]
        public string Description { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số dương")]
        [MaxLength(255, ErrorMessage = "Đường dẫn ảnh tối đa 255 ký tự")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("Loai")]
        [Required(ErrorMessage = "Mã loại là bắt buộc")]
        public int MaLoai { get; set; }
        public LoaiModel? Loai { get; set; }
    }
}
