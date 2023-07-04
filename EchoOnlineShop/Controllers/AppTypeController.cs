 using EchoOnlineShop.Data;
using EchoOnlineShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EchoOnlineShop.Controllers
{
    [Authorize(Roles=WC.AdminRole)]
    public class AppTypeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public AppTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<AppType> objCategoryList = _context.AppType;
            return View(objCategoryList);
        }

        // GET - Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppType obj)
        {
            if (ModelState.IsValid)
            {
                if (obj != null)
                {
                    _context.AppType.Add(obj);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(obj);
        }

        // GET - Edit
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                NotFound();
            }
            var obj = _context.AppType.Find(id);
            if (obj == null)
            {
                NotFound();

            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AppType obj)
        {
            if (ModelState.IsValid)
            {
                if (obj != null)
                {
                    _context.AppType.Update(obj);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(obj);
        }

        // GET - Delete
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                NotFound();
            }
            var obj = _context.AppType.Find(id);
            if (obj == null)
            {
                NotFound();

            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            if (id == 0)
            {
                NotFound();
            }
            var obj = _context.AppType.Find(id);
            if (obj == null)
            {
                NotFound();

            }
            _context.AppType.Remove(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
