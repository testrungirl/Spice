using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                    menuItemFromDb.Image = @"\images" + MenuItemVM.MenuItem.Id + extension;
                }
                else
                {
                    //no file was uploaded, so use default
                    var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".jpg");
                    menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".jpg";
                }
                await db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(MenuItemVM);
        }
    }
}