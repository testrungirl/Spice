 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

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
            IndexVM.MenuItem = await db.MenuItem.Include(m => m.Category).Include(x=>x.SubCategory).ToListAsync();

            IndexVM.Category = await db.Category.ToListAsync();
            IndexVM.Coupon = await db.Coupon.Where(x=>x.isActive==true).ToListAsync();
            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItem = await db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.Id == id);
            ShoppingCart CartObj = new ShoppingCart()
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id,
            };
            return View(CartObj);
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
