using System.ComponentModel.DataAnnotations;

namespace Responsive.Models
{
    public class BookCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        //linked other class
        public ICollection<Book>? Books { get; set; }
    }
}
