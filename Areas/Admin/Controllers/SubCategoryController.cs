using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

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

        //Get Inex
        public async Task<IActionResult> Index()
        {
            //Get Index
            return View(await db.SubCategory.Include(s=>s.Category).ToListAsync()); ;
        }

        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await db.SubCategory.OrderBy(p => p.Name)
                                                        .Select(p => p.Name)
                                                        .Distinct()
                                                        .ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategory subCategory)
        {
            if (ModelState.IsValid)
            {
                db.SubCategory.Add(subCategory);
                await db.SaveChangesAsync();
                return View(nameof(Index));

            }
            return View(subCategory);
        }
    }
}