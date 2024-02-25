using System.ComponentModel.DataAnnotations;

namespace Responsive.Models
{
    public class BookType
    {
        [Key]
        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public ICollection<Book> Books { get; set;}
    }
}
