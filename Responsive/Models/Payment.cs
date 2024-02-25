using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Responsive.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [DisplayName("ธนาคาร")]
        [Required(ErrorMessage = "Please enter the payment type.")]
        public string PaymentType { get; set; }

        [DisplayName("ชื่อ-นามสกุล")]
        [Required(ErrorMessage = "Please enter the holder's name.")]
        public string HolderName { get; set; }

        [DisplayName("เลขธนาคาร/บัตรเครดิต")]
        [Required(ErrorMessage = "Please enter the last 4 digits of the payment number.")]
        public string PaymentNumber { get; set; }

        [DisplayName("เบอร์โทรศัพท์")]
        [Required(ErrorMessage = "Please enter the phone number.")]
        public string PhoneNumber { get; set; }

        //link user
        [ForeignKey("Account")]
        public string? UserId { get; set; }
        public Account? Account { get; set; }
    }
}
