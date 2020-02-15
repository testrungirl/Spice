using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext db;

        public SubCategoryController(ApplicationDbContext Db)
        {
            db = Db;
        }
        public async Task<IActionResult> Index()
        {
            //Get Index
            return View(await db.SubCategory.Include(s=>s.Category).ToListAsync()); ;
        }
    }
}