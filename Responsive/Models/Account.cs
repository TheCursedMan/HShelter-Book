using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace Responsive.Models
{
    public class Account : IdentityUser
    {
        [Required(ErrorMessage = "กรุณาป้อนชื่อ")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "กรุณาป้อนนามสกุล")]
        public string CustomerSurname { get; set; }
        [Required(ErrorMessage = "กรุณาป้อนวันเกิด")]
        [DataType(DataType.Date)]
        public string BirthDate { get; set; }
        [Required(ErrorMessage = "กรุณาระบุเพศ")]
        public string Gender { get; set; }

        public string? CustomerImgName { get; set; }
        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile? CustomerImg { get; set; }


        //linked part
        public ICollection<Order>? Orders { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }    
        public ICollection<UserAddress>? UserAddresses { get; set; }
    }
}
