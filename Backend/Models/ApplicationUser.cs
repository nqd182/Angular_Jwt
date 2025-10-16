using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Jwt.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string HoTen { get; set; }

    }
}
