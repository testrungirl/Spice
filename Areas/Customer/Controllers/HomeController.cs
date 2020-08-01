using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;

        public HomeController(ApplicationDbContext _db)
        {
            db = _db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel();
            IndexVM.MenuItem = await db.MenuItem.Include(m => m.Category).Include(x => x.SubCategory).ToListAsync();

            IndexVM.Category = await db.Category.ToListAsync();
            IndexVM.Coupon = await db.Coupon.Where(x => x.isActive == true).ToListAsync();
            var user = User.FindFirst(ClaimTypes.NameIdentifier);

            if(user != null)
            {
                var count = db.ShoppingCart.Where(x => x.ApplicationUserId == user.Value).Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            }
            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItem = await db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.Id == id);
            ShoppingCart CartObj = new ShoppingCart()
            {
                MenuItem = menuItem,
                ItemId = menuItem.Id,
            };
            return View(CartObj);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart obj)
        {
            obj.Id = 0;
            if (ModelState.IsValid)
            {
                //var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                //var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                obj.ApplicationUserId = userId;
                ShoppingCart cartFromDb = await db.ShoppingCart.Where(x => x.ApplicationUserId == obj.ApplicationUserId 
                                            && x.ItemId == obj.ItemId).FirstOrDefaultAsync();
                if (cartFromDb == null)
                {
                    await db.ShoppingCart.AddAsync(obj);
                }
                else
                {
                    cartFromDb.Count += obj.Count;
                }
                await db.SaveChangesAsync();
                var count = db.ShoppingCart.Where(c => c.ApplicationUserId == obj.ApplicationUserId).Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
                return RedirectToAction("Index");


            }
            else
            {
                var menuItem = await db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.Id == obj.ItemId);
                ShoppingCart CartObj = new ShoppingCart()
                {
                    MenuItem = menuItem,
                    ItemId = menuItem.Id,
                };
                return View(CartObj);
            }
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
