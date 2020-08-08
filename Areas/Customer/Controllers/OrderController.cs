using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext db;
        private int Pagesize = 2;
        [BindProperty]
        public OrderDetailsVM orderDetailsVM { get; set; }
        public OrderController(ApplicationDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = await db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == UserId);
            orderDetailsVM = new OrderDetailsVM()
            {
                OrderHeader = await db.OrderHeader.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.UserId == UserId && x.Id == id),
                OrderDetails = await db.OrderDetails.Where(z => z.OrderId == id).ToListAsync(),
            };
            return View(orderDetailsVM);
        }
        [Authorize]
        public async Task<IActionResult> OrderHistory(int productPage = 1)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = await db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == UserId);

            OrderListVM orderListVM = new OrderListVM()
            {
                Orders = new List<OrderDetailsVM>(),
            };

            var OrderHeaders = await db.OrderHeader.Include(x => x.ApplicationUser).Where(x => x.UserId == UserId).ToListAsync();
            foreach (var obj in OrderHeaders)
            {
                orderDetailsVM = new OrderDetailsVM()
                {
                    OrderHeader = obj,
                    OrderDetails = await db.OrderDetails.Where(z => z.OrderId == obj.Id).ToListAsync(),
                };
                orderListVM.Orders.Add(orderDetailsVM);
            }
            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id)
                                                    .Skip((productPage - 1) * Pagesize)
                                                    .Take(Pagesize).ToList();
            orderListVM.PagingInfo = new PagingInfo
              {
                CurrentPage = productPage,
                ItemsPerPage = Pagesize,
                TotalItem = count,
                urlParam = "/Customer/Order/OrderHistory?productPage=:"

            };

            return View(orderListVM);
        }
        [Authorize]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            OrderDetailsVM orderDetailsVM = new OrderDetailsVM()
            {
                OrderHeader = await db.OrderHeader.FirstOrDefaultAsync(x => x.Id == id),
                OrderDetails = await db.OrderDetails.Where(z => z.OrderId == id).ToListAsync(),
            };
            orderDetailsVM.OrderHeader.ApplicationUser = await db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == orderDetailsVM.OrderHeader.UserId);
            return PartialView("_IndividualOrderDetails", orderDetailsVM);
        }
        [Authorize]
        public async Task<IActionResult> GetOrderStatus(int id)
        {
            OrderDetailsVM orderDetailsVM = new OrderDetailsVM()
            {
                OrderHeader = await db.OrderHeader.FirstOrDefaultAsync(x => x.Id == id),
            };
            orderDetailsVM.OrderHeader.ApplicationUser = await db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == orderDetailsVM.OrderHeader.UserId);
            return PartialView("_OrderStatus", orderDetailsVM);
        }
    }
}