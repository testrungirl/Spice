using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using Stripe;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db;
        private OrderDetailsVM orderDetailsVM;

        [BindProperty]
        public OrderDetailsCart DetailsCart { get; set; }
        public CartController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            DetailsCart = new OrderDetailsCart()
            {
                OrderHeaders = new Models.OrderHeader()
            };
            DetailsCart.OrderHeaders.OrderTotal = 0;
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cart = db.ShoppingCart.Where(x => x.ApplicationUserId == UserId);
            if (cart != null)
            {
                DetailsCart.ShoppingCarts = cart.ToList();
            }
            foreach (var list in DetailsCart.ShoppingCarts)
            {
                list.MenuItem = await db.MenuItem.FirstOrDefaultAsync(x => x.Id == list.ItemId);
                DetailsCart.OrderHeaders.OrderTotal += (list.MenuItem.Price * list.Count);
                list.MenuItem.Description = SD.ConvertToRawHtml(list.MenuItem.Description);
                if (list.MenuItem.Description.Length > 100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            DetailsCart.OrderHeaders.OrderTotalOriginal = DetailsCart.OrderHeaders.OrderTotal;
            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailsCart.OrderHeaders.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await db.Coupon.FirstOrDefaultAsync(c => c.Name.ToLower() == DetailsCart.OrderHeaders.CouponCode.ToLower());
                DetailsCart.OrderHeaders.OrderTotal = SD.DiscountedPrice(couponFromDb, DetailsCart.OrderHeaders.OrderTotalOriginal);
            }
            return View(DetailsCart);
        }

        public async Task<IActionResult> Summary()
        {
            DetailsCart = new OrderDetailsCart()
            {
                OrderHeaders = new Models.OrderHeader()
            };
            DetailsCart.OrderHeaders.OrderTotal = 0;
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = await db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == UserId);
            var cart = db.ShoppingCart.Where(x => x.ApplicationUserId == UserId);
            if (cart != null)
            {
                DetailsCart.ShoppingCarts = cart.ToList();
            }
            foreach (var list in DetailsCart.ShoppingCarts)
            {
                list.MenuItem = await db.MenuItem.FirstOrDefaultAsync(x => x.Id == list.ItemId);
                DetailsCart.OrderHeaders.OrderTotal += (list.MenuItem.Price * list.Count);

            }
            DetailsCart.OrderHeaders.PickupName = user.Name;
            DetailsCart.OrderHeaders.PhoneNumber = user.PhoneNumber;
            DetailsCart.OrderHeaders.PickupTime = DateTime.Now;
            DetailsCart.OrderHeaders.OrderTotalOriginal = DetailsCart.OrderHeaders.OrderTotal;
            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailsCart.OrderHeaders.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await db.Coupon.FirstOrDefaultAsync(c => c.Name.ToLower() == DetailsCart.OrderHeaders.CouponCode.ToLower());
                DetailsCart.OrderHeaders.OrderTotal = SD.DiscountedPrice(couponFromDb, DetailsCart.OrderHeaders.OrderTotalOriginal);
            }
            return View(DetailsCart);
        }
        public IActionResult AddCoupon()
        {
            if (DetailsCart.OrderHeaders.CouponCode == null)
            {
                DetailsCart.OrderHeaders.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, DetailsCart.OrderHeaders.CouponCode);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await db.ShoppingCart.FirstOrDefaultAsync(x => x.Id == cartId);
            cart.Count += 1;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await db.ShoppingCart.FirstOrDefaultAsync(x => x.Id == cartId);
            if (cart.Count == 1)
            {
                db.ShoppingCart.Remove(cart);
                await db.SaveChangesAsync();

                var count = db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            }
            else
            {
                cart.Count -= 1;
                await db.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await db.ShoppingCart.FirstOrDefaultAsync(x => x.Id == cartId);
            db.ShoppingCart.Remove(cart);
            await db.SaveChangesAsync();

            var count = db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string StripeToken)
        {
            try
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                DetailsCart.ShoppingCarts = await db.ShoppingCart.Where(x => x.ApplicationUserId == UserId).ToListAsync();

                DetailsCart.OrderHeaders.PaymentStatus = SD.PaymentStatusPending;
                DetailsCart.OrderHeaders.OrderDate = DateTime.Now;
                DetailsCart.OrderHeaders.UserId = UserId;
                DetailsCart.OrderHeaders.Status = SD.PaymentStatusPending;
                DetailsCart.OrderHeaders.PickupTime = Convert.ToDateTime(DetailsCart.OrderHeaders.PickUpDate.ToShortDateString() + " " + DetailsCart.OrderHeaders.PickupTime.ToShortTimeString());
                DetailsCart.OrderHeaders.PhoneNumber = DetailsCart.OrderHeaders.PhoneNumber;
                DetailsCart.OrderHeaders.Comments = DetailsCart.OrderHeaders.Comments;

                List<OrderDetails> orderDetailsList = new List<OrderDetails>();
                db.OrderHeader.Add(DetailsCart.OrderHeaders);
                await db.SaveChangesAsync();

                foreach (var list in DetailsCart.ShoppingCarts)
                {
                    list.MenuItem = await db.MenuItem.FirstOrDefaultAsync(x => x.Id == list.ItemId);
                    OrderDetails orderDetails = new OrderDetails
                    {
                        MenuItemId = list.ItemId,
                        OrderId = DetailsCart.OrderHeaders.Id,
                        Description = list.MenuItem.Description,
                        Name = list.MenuItem.Name,
                        Price = list.MenuItem.Price,
                        Count = list.Count,
                    };
                    DetailsCart.OrderHeaders.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                    db.OrderDetails.Add(orderDetails);

                }

                if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
                {
                    DetailsCart.OrderHeaders.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                    var couponFromDb = await db.Coupon.FirstOrDefaultAsync(c => c.Name.ToLower() == DetailsCart.OrderHeaders.CouponCode.ToLower());
                    DetailsCart.OrderHeaders.OrderTotal = SD.DiscountedPrice(couponFromDb, DetailsCart.OrderHeaders.OrderTotalOriginal);
                }
                else
                {
                    DetailsCart.OrderHeaders.OrderTotal = DetailsCart.OrderHeaders.OrderTotalOriginal;
                }
                DetailsCart.OrderHeaders.CouponCodeDiscount = DetailsCart.OrderHeaders.OrderTotalOriginal - DetailsCart.OrderHeaders.OrderTotal;
                await db.SaveChangesAsync();
                db.ShoppingCart.RemoveRange(DetailsCart.ShoppingCarts);
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
                await db.SaveChangesAsync();

                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(DetailsCart.OrderHeaders.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order Id : " + DetailsCart.OrderHeaders.Id,
                    Source = StripeToken,
                    //Customer = DetailsCart.OrderHeaders.PickupName,
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);

                if (charge.BalanceTransactionId == null)
                {
                    DetailsCart.OrderHeaders.Status = SD.PaymentStatusRejected;
                }
                else
                {
                    DetailsCart.OrderHeaders.TransactionId = charge.BalanceTransactionId;
                }
                if (charge.Status.ToLower() == "succeeded")
                {
                    DetailsCart.OrderHeaders.PaymentStatus = SD.PaymentStatusApproved;
                    DetailsCart.OrderHeaders.Status = SD.StatusSubmitted;
                }
                else
                {
                    DetailsCart.OrderHeaders.PaymentStatus = SD.PaymentStatusRejected;
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
                //return RedirectToAction("Confirm", "Order", new { id = DetailsCart.OrderHeaders.Id });

            }
            catch (StripeException e)
            {
                var error = "";
                switch (e.StripeError.Type)
                {
                    case "card_error":
                        error = "Code: " + e.StripeError.Code + " Message: " + e.StripeError.Message;
                        break;
                    case "api_connection_error":
                        error = "api_connection_error";
                        break;
                    case "api_error":
                        error = "api_error";
                        break;
                    case "authentication_error":
                        error = "authentication_error";
                        break;
                    case "invalid_request_error":
                        error = "invalid_request_error";
                        break;
                    case "rate_limit_error":
                        error = "rate_limit_error";
                        break;
                    case "validation_error":
                        error = "validation_error";
                        break;
                    default:
                        error = "Unknown Error Type";
                        break;

                }
                return StatusCode(406, error);
            }
        }

        

    }
}