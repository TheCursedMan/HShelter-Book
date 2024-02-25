using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Responsive.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public string? Comment { get; set; }
        [Range(0, 5)]
        public float Rating { get; set; }
        public DateTime ReviewDate { get; set; }

        //linked other class
        [ForeignKey("Book")]
        public int? BookId { get; set; }
        public Book? Book { get; set; }
        [ForeignKey("Account")]
        public string? UserId { get; set; }
        public Account? Account { get; set; }
    }
}
