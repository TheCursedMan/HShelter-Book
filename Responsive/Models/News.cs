using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Responsive.Models
{
    public class News
    {
        [Key] 
        public int NewsId { get; set; }
        [Required]
        public string NewsName { get; set; }
        [Required]
        public string NewsDescription { get; set;}
        [Required]
        [DataType(DataType.DateTime)]
        public string NewsDate { get; set; }
        [NotMapped]
        public IFormFile? NewsImg { get; set; }
        public string? NewsImgName { get; set; }

    }
}
