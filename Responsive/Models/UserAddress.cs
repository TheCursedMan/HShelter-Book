using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Responsive.Models
{
    public class UserAddress
    {
        [Key]
        public int AddressId { get; set; }
        [Required(ErrorMessage = "โปรดใส่ชื่อ")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "โปรดใส่ที่อยู่")]
        public string Address { get; set; }
        [Required(ErrorMessage = "โปรดใส่เลขไปรษณีย์")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "โปรดป้อนรหัสไปรษณีย์ให้ถูกต้อง")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "โปรดเบอร์โทรศัพท์")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "โปรดใส่เบอร์โทรศัพท์ให้ถูกต้อง")]
        public string PhoneNumber { get; set; }

        //link user
        [ForeignKey("Account")]
        public string? UserId { get; set; }
        public Account? Account { get; set; }
    }
}
