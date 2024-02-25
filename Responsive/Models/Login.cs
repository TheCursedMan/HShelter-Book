using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Responsive.Models
{
    public class Login
    {
        [Display(Name ="Email Address")]
        [Required(ErrorMessage="Email address is required")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]   
        public string Password { get; set; }
    }
}
