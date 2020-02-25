using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [TempData]
        public string StatusMessage { get; set; }
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
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {

                var doesSubCategoryExists = db.SubCategory.Include(s => s.Category)
                                                        .Where(s => s.Name == model.SubCategory.Name 
                                                        && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExists.Count() > 0)
                {
                    //Error
                    StatusMessage = "Error : Sub Category exists under " + doesSubCategoryExists.First().Category.Name + " name. Please use another name";
                }
                else
                {
                    db.SubCategory.Add(model.SubCategory);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await db.SubCategory.OrderBy(x => x.Name)
                                                        .Select(x => x.Name)
                                                        .ToListAsync(),
                StatusMessage = StatusMessage,

            };
            return View(modelVM);
        } 

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from SubCategory in db.SubCategory
                             where SubCategory.CategoryId == id
                             select SubCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }
    }
}