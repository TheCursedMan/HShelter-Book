using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Responsive.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        //linked other part
        [ForeignKey("Account")]
        public string? UserId { get; set; }
        public Account? Account { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }

        //added variable
        public int Quantity { get; set; }
        public float TotalPrice { get; set; }

        [ForeignKey("Book")]
        public int? BookId { get; set; }
        public Book? Book { get; set; }

        //link to orderitem
        public ICollection<OrderCartItem>? OrderCartItems { get; set; }
    }
}
    