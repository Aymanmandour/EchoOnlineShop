using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EchoOnlineShop.Models
{
    public class AppType
    {

        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
