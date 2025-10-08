using System.ComponentModel.DataAnnotations;

namespace Jwt.Models
{
    public class NguoiDung
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(250)]
        public string Password { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }

    }
}
