using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Responsive.Models
{
    public class Register
    { 
        [EmailAddress]
        [Required(ErrorMessage = "กรุณาป้อนอีเมล์การใช้งาน")]
        public string Email { get; set; }
        [Required(ErrorMessage = "กรุณาป้อนรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [NotMapped]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        [Required(ErrorMessage = "กรุณายืนยันรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        /*------*/
        [Required(ErrorMessage = "กรุณาป้อนชื่อ")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "กรุณาป้อนนามสกุล")]
        public string CustomerSurname { get; set; }
        [Required(ErrorMessage = "กรุณาป้อนวันเกิด")]
        [DataType(DataType.Date)]
        public string BirthDate { get; set; }
        [Required(ErrorMessage = "กรุณาระบุเพศ")]
        public string Gender { get; set; }
        
    }
}
