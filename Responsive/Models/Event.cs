using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Responsive.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        [Required]
        public string EventName { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public string Eventdate { get; set; }
        [Required]
        public string EventDescription { get; set; }
        [NotMapped]
        public IFormFile? EventImg { get; set; }
        public string? EventImgName { get; set; }
    }
}
