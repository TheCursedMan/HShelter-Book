using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Responsive.Models
{
    public class OrderCartItem
    {
        [Key]
        public int OrderCartItemId { get; set; }
        public int Quantity { get; set; }

        //linked other part

        [ForeignKey("Book")]
        public int? BookId { get; set; }
        public Book? Book { get; set; }
        [ForeignKey("Order")]
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
