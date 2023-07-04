using System.Collections.Generic;
using EchoOnlineShop.Models;

namespace EchoOnlineShop.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            ProductList = new List<Product>();
        }
        public ApplicationUser ApplicationUser { get; set; }
        public List<Product> ProductList { get; set; }
    }
}
