using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EchoOnlineShop.Models
{
    public class Product
    {
        public Product()
        {
            TempQty = 1;
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }
        public string Image { get; set; }

        [Display(Name= "Category Type")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } // To enable lazy loading use the keyword virtual 

        [Display(Name = "Application Type")]
        public int AppTypeID { get; set; }

        [ForeignKey("AppTypeID")]
        public virtual AppType AppType { get; set; } // To enable lazy loading use the keyword virtual 

        [NotMapped]
        [Range(1,10000)]
        public int TempQty { get; set; }

    }
}
