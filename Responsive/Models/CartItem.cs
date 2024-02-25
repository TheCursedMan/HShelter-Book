using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Responsive.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public int Quantity { get; set; }


        //linked other part

        [ForeignKey("Book")]
        public int? BookId { get; set; }
        public Book? Book { get; set; }
        [ForeignKey("Order")]
        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        //link user
        [ForeignKey("Account")]
        public string? UserId { get; set; }
        public Account? Account { get; set; }
    }
}
