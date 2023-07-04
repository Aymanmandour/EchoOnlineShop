using EchoOnlineShop.Data;
using EchoOnlineShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EchoOnlineShop.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _context.Category;
            return View(objCategoryList);
        }

        // GET - Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                if (obj != null)
                {
                    _context.Category.Add(obj);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            // If the model is not valid, return the view with object with the errors values
            return View(obj);
        }
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                NotFound();
            }
            var obj = _context.Category.Find(id);
            if (obj == null)
            {
                NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                if (obj != null)
                {
                    _context.Category.Update(obj);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            // If the model is not valid, return the view with object with the errors values
            return View(obj);
        }

        // GET - Delete
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                NotFound();
            }
            var obj = _context.Category.Find(id);
            if (obj == null)
            {
                NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id) // we need to change the name because of the  GET - Delete 
        {
            if (id == 0)
            {
                NotFound();
            }
            var obj = _context.Category.Find(id);
            if (obj == null)
            {
                NotFound();
            }
            _context.Category.Remove(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
