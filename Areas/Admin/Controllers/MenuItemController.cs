using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly IWebHostEnvironment hostingEnvironment; 
        public MenuItemController(ApplicationDbContext Db, IWebHostEnvironment HostingEnvironment)
        {
            db = Db;
            hostingEnvironment = HostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await db.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).ToListAsync());
        }

        public IActionResult Create()
        {

            return View();
        }
    }
}