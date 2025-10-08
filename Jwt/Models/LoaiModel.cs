using System.ComponentModel.DataAnnotations;

namespace Jwt.Models
{
    public class LoaiModel
    {
        [Key]    
        public int MaLoai { get; set; }
        [Required]
        [MaxLength(50)]
        public string TenLoai { get; set; }
    }
}
