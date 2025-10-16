using System.ComponentModel.DataAnnotations;

namespace Jwt.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Hãy nhập tên người dùng")]
        public string HoTen { get; set; }
        [Required(ErrorMessage = "Hãy nhập tên tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Hãy nhập email"), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hãy nhập mật khẩu")]
        public string Password { get; set; }
    }
}
