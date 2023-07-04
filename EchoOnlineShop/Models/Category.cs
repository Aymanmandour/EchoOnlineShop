using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EchoOnlineShop.Models
{
    public class Category
    {

        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1,int.MaxValue,ErrorMessage ="Display order must be greater than 0")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
