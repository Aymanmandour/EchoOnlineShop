using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EchoOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EchoOnlineShop.Data;
using EchoOnlineShop.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EchoOnlineShop.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EchoOnlineShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM();
            homeVM.Products = _context.Product.Include(c => c.Category).Include(a => a.AppType);
            homeVM.Categories = _context.Category;

            // or
            //HomeVM homeVM1 = new HomeVM()
            //{
            //    Products = _context.Product.Include(c => c.Category),
            //    Categories = _context.Category
            //};
            //return View(homeVM1);
            return View(homeVM);
        }
        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();

            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _context.Product.Include(c => c.Category).Include(a => a.AppType).FirstOrDefault(p => p.Id == id), // you must retrun only one product
                ExistInCart = false

            };

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            foreach (var item in shoppingCartsList)
            {
                if (item.ProductId == id)
                {
                    detailsVM.ExistInCart = true;
                }

            }

            return View(detailsVM);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id, DetailsVM detailsVM)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            shoppingCartsList.Add(new ShoppingCart() { ProductId = id, QTY = detailsVM.Product.TempQty });
            HttpContext.Session.Set(WC.SessionCart, shoppingCartsList);

            // to avoid using magic string like "Index" use nameof
            return RedirectToAction(nameof(Index)); // or return RedirectToAction("Index");
        }

        public IActionResult RemovFromCart(int id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            var itemToRemove = shoppingCartsList.SingleOrDefault(r => r.ProductId == id);
            if (itemToRemove != null)
            {
                shoppingCartsList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartsList);

            // to avoid using magic string like "Index" use nameof
            return RedirectToAction(nameof(Index)); // or return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

      
    }
}
