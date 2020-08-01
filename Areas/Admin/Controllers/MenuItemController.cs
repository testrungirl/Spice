using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly IWebHostEnvironment hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext Db, IWebHostEnvironment HostingEnvironment)
        {
            db = Db;
            hostingEnvironment = HostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = db.Category,
                MenuItem = new Models.MenuItem(),
            };
        }
        public async Task<IActionResult> Index()
        {
            return View(await db.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).ToListAsync());
        }

        //Get Create
        public IActionResult Create()
        {
            
            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            var id = Request.Form["SubCategoryId"];
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (ModelState.IsValid)
            {
                db.MenuItem.Add(MenuItemVM.MenuItem);
                await db.SaveChangesAsync();

                //Work on the image saving section
                string webRootPath = hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var menuItemFromDb = await db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

                if (files.Count > 0)
                {
                    //files has been uploaded
                    var uploads = Path.Combine(webRootPath, "images");
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }
                    menuItemFromDb.Image = @"/images/" + MenuItemVM.MenuItem.Id + extension;
                }
                else
                {
                    //no file was uploaded, so use default
                    var uploads = Path.Combine(webRootPath, @"images/" + SD.DefaultFoodImage);
                    System.IO.File.Copy(uploads, webRootPath + @"/images/" + MenuItemVM.MenuItem.Id + ".jpg");
                    menuItemFromDb.Image = @"/images/" + MenuItemVM.MenuItem.Id + ".jpg";
                }
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(MenuItemVM);
        }

        //Get Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var menuItem= await db.MenuItem.Include(m=>m.Category).Include(m=>m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            if (menuItem== null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.SubCategory = await db.SubCategory.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (ModelState.IsValid)
            {
                //Work on the image saving section
                string webRootPath = hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var menuItemFromDb = await db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

                if (files.Count > 0)
                {
                    //New Image has been uploaded
                    var uploads = Path.Combine(webRootPath, "images");
                    var extension_new = Path.GetExtension(files[0].FileName);

                    //Delete the original file
                    var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    //we will upload the new file
                    using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }
                    menuItemFromDb.Image = @"/images/" + MenuItemVM.MenuItem.Id + extension_new;
                }
                menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
                menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
                menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
                menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
                menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
                menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;
                db.MenuItem.Update(menuItemFromDb);
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            MenuItemVM.SubCategory = await db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var menuItem= await db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (menuItem== null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.SubCategory = await db.SubCategory.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
             if (id == null)
            {
                return NotFound();
            }
            var menuItem= await db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            if (menuItem== null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.SubCategory = await db.SubCategory.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        //Post - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //STRING_SPLIT(string, separator)
            var menuItem = await db.MenuItem.SingleOrDefaultAsync(m => m.Id == id);
            if(menuItem == null)
            {
                return View(nameof(Index));
            }

            string webRootPath = hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);

                db.Remove(menuItem);
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(nameof(Index));
        }
    }
}