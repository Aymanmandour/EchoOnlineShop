using EchoOnlineShop.Data;
using EchoOnlineShop.Models;
using EchoOnlineShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace EchoOnlineShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objProductList = _context.Product.Include(c=>c.Category).Include(a=>a.AppType);

            //foreach (var obj in objProductList)
            //{
            //    obj.Category = _context.Category.FirstOrDefault(u => u.ID == obj.CategoryId);
            //    obj.AppType = _context.AppType.FirstOrDefault(u => u.ID == obj.AppTypeID);
            //}
            return View(objProductList);
        }

        // GET - InsertUpdate
        // The id will be key to know if create or Update. If create the Id is null. If delete the Id has value
        public IActionResult InsertUpdate(int? id)
        {
            ProductVM productVm = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _context.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ID.ToString()
                }),
                 AppTypeSelectList = _context.AppType.Select(i => new SelectListItem
                 {
                     Text = i.Name,
                     Value = i.ID.ToString()
                 })
            };

            if (id == null)
            {
                // This means create
                return View(productVm); // empty object
            }
            else
            {
                productVm.Product = _context.Product.Find(id);
                if (productVm.Product == null)
                {
                    return NotFound();
                }
                return View(productVm);
            }
            //IEnumerable<SelectListItem> CategoryDD = _context.Category.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.ID.ToString()
            //});

            //ViewBag.CategoryDD = CategoryDD;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertUpdate(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string WebRootPath = _webHostEnvironment.WebRootPath;
                if (productVM.Product.Id == 0)
                {
                    // create
                    string upLoad = WebRootPath + WC.ImagePath; // Save picture to new location
                    string fileName =   Guid.NewGuid().ToString(); // create random name
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream= new FileStream(Path.Combine(upLoad, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.Image = fileName + extension;
                    _context.Product.Add(productVM.Product); 
                }
                else
                {
                    var objFromDb= _context.Product.AsNoTracking().FirstOrDefault(p=>p.Id==productVM.Product.Id);
                    if (files.Count>0) // If there is iamge, means we need to delete the old one and upload the new one
                    {
                        string upLoad = WebRootPath + WC.ImagePath; // Save picture to new location
                        string fileName = Guid.NewGuid().ToString(); // create random name
                        string extension = Path.GetExtension(files[0].FileName);
                        //Delete old image
                        var oldFile = Path.Combine(upLoad, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upLoad, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extension;
                        _context.Product.Add(productVM.Product);
                    }
                    else // if no image maeans no update for the iamge
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    // update
                    _context.Product.Update(productVM.Product);
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            // If the model is not valid, return the view with object with the errors values

            productVM.CategorySelectList = _context.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.ID.ToString()
            });

            productVM.AppTypeSelectList = _context.AppType.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.ID.ToString()
            });

            return View(productVM);
        }
        // GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id == 0)
            {
                NotFound();
            }
            Product product = _context.Product.Include(c=>c.Category).Include(a=>a.AppType).FirstOrDefault(p=>p.Id==id);

            if (product == null)
            {
                NotFound();
            }
            return View(product);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id) // we need to change the name because of the  GET - Delete 
        {
            var objFromDb = _context.Product.Find(id);
            if (objFromDb == null)
            {
                NotFound();
            }

            var files = HttpContext.Request.Form.Files;
            string WebRootPath = _webHostEnvironment.WebRootPath;
            string upLoad = WebRootPath + WC.ImagePath; // Save picture to new location

            //Delete old image
            var oldFile = Path.Combine(upLoad, objFromDb.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
     
            _context.Product.Remove(objFromDb);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
