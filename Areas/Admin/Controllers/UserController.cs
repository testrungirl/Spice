using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
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

            return View(await db.ApplicatonUser.Where(x=>x.Id != claim.Value).ToListAsync());
        }
    }
}