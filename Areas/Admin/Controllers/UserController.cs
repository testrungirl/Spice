using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext db;
        public UserController(ApplicationDbContext _db)
        {
            db = _db;
        }

        public async Task<IActionResult> Index()
        {
            var ClaimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await db.ApplicationUser.Where(x=>x.Id != claim.Value).ToListAsync());
        }

        public async Task<IActionResult> Lock(string Id)
        {
            if(Id == null)
            {
                return NotFound();
            }
            var applicationUser = await db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == Id);
            if(applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UnLock(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var applicationUser = await db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == Id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.LockoutEnd = DateTime.Now;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}