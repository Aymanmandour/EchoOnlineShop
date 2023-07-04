using Braintree;
using EchoOnlineShop.Data;
using EchoOnlineShop.Models;
using EchoOnlineShop.Utilities;
using EchoOnlineShop.Utilities.BrainTree;
using EchoOnlineShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EchoOnlineShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBrainTreeGate _brain;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContext context, IBrainTreeGate brain)
        {
            _context = context;
            _brain = brain;
        }

        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //shoppingCartsList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).ToList();

                //or 
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            // get distinct products
            List<int> prodInCart = shoppingCartsList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodListTemp = _context.Product.Where(p => prodInCart.Contains(p.Id));
            IList<Product> prodList = new List<Product>();
            foreach (var cartObj in shoppingCartsList)
            {
                Product productTemp = prodListTemp.FirstOrDefault(p => p.Id == cartObj.ProductId);
                productTemp.TempQty = cartObj.QTY;
                prodList.Add(productTemp);
            }
            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> prodlist)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (var prod in prodlist)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, QTY = prod.TempQty });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            var gateway = _brain.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;

            var claimsIdentiy = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentiy.FindFirst(ClaimTypes.NameIdentifier);
            //or 
            //var userId = User.FindFirstValue(ClaimTypes.Name);

            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //shoppingCartsList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).ToList();

                //or 
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            // get distinct products
            List<int> prodInCart = shoppingCartsList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _context.Product.Where(p => prodInCart.Contains(p.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _context.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                //,ProductList = prodList

            };
            foreach (var cartObj in shoppingCartsList)
            {
                Product productTemp = _context.Product.FirstOrDefault(p => p.Id == cartObj.ProductId);
                productTemp.TempQty = cartObj.QTY;
                ProductUserVM.ProductList.Add(productTemp);
            }
            //return View(prodList);
            return View(ProductUserVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(IFormCollection collection,ProductUserVM productUserVM)
        {
            var claimsIdentiy = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentiy.FindFirst(ClaimTypes.NameIdentifier);

            // Create header
            OrderHeader orderHeader = new OrderHeader()
            {
                CreatedByUserId = claim.Value,
                FinalOrderTotal = productUserVM.ProductList.Sum(p=>p.Price*p.TempQty),
                City = productUserVM.ApplicationUser.City,
                StreetAddress = productUserVM.ApplicationUser.StreetAddress,
                State = productUserVM.ApplicationUser.State,
                PostalCode = productUserVM.ApplicationUser.PostalCode,
                FullName = productUserVM.ApplicationUser.FullName,
                Email = productUserVM.ApplicationUser.Email,
                PhoneNumber ="123", // productUserVM.ApplicationUser.PhoneNumber,
                OrderDate=DateTime.Now,
                OrderStatus=WC.StatusPending,
            };

            _context.OrderHeader.Add(orderHeader);
            _context.SaveChanges();


            foreach (var prod in ProductUserVM.ProductList)
            {
                OrderDetails orderDetails = new OrderDetails()
                {
                    OrderHeaderId = orderHeader.Id,
                    PricePerQty = prod.Price,
                    Qty=prod.TempQty,
                    ProductId=prod.Id

                };
                _context.OrderDetails.Add(orderDetails);
                _context.SaveChanges();

            }
            TempData[WC.Success] = "Order submitted successfully :" + orderHeader.Id.ToString(); // Not in use, for now
            ////braintree
            string nonceFromTheClient = collection["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                PaymentMethodNonce = nonceFromTheClient,
                //DeviceData = deviceDataFromTheClient,
                OrderId = orderHeader.Id.ToString(),
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var gateway = _brain.GetGateway();
            Result<Transaction> result = gateway.Transaction.Sale(request);

            //braintree
            if (result.Target.ProcessorResponseText == "Approved")
            {
                orderHeader.TransactionId = result.Target.Id;
                orderHeader.OrderStatus = WC.StatusApproved;
            }
            else
            {
                orderHeader.OrderStatus = WC.StatusCancelled;
            }
            _context.SaveChanges();

            HttpContext.Session.Clear();

            return RedirectToAction(nameof(Status));
        }

        public IActionResult Status()
        {
            return View();
        }
        public IActionResult Remove(int Id)
        {

            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //shoppingCartsList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).ToList();

                //or 
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            shoppingCartsList.Remove(shoppingCartsList.FirstOrDefault(p => p.ProductId == Id));
            HttpContext.Session.Set(WC.SessionCart, shoppingCartsList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> prodlist)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (var prod in prodlist)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = prod.Id, QTY = prod.TempQty });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            //HttpContext.Session.Set(WC.SessionCart, "");
            return RedirectToAction("Index","Home");
        }
    }
}
