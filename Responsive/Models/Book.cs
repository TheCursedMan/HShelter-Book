using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Responsive.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string? BookImgName { get; set; }
        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile? BookImg { get; set; }
        [Required]
        public string BookName { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int BookPrice { get; set; }
        [Required]
        public string AuthorName { get; set; }
        [Required]
        public string Publisher { get; set; }
        public string? Plot { get; set; }
        [Range(1,10 , ErrorMessage ="อันดับหนังสือต้องอยู่แค่ 1-10 เท่านั้น") ]
        public int? BookRank { get; set; }
        public bool? IsDiscount { get; set; }
        [Range(0, 100 , ErrorMessage= "ช่วยใส่เป็น 0-100% ด้วยครับ")]
        public float? DiscountPer { get; set; }


        //linked part
        [ForeignKey("BookCategory")]
        public int? CategoryId { get; set; }
        public BookCategory? BookCategory { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        //added part

        [ForeignKey("BookType")]
        public int? TypeId { get; set; }
        public BookType? BookType { get; set; }
    }
}
