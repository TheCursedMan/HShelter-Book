using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Responsive.Data;
using Responsive.Models;
using System.Security.Claims;

namespace Responsive.Controllers
{
    public class Orderctl : Controller
    {
        private readonly BookDBContext _db;
        public Orderctl(BookDBContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            string? currentUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId != null)
            {
                // Retrieve cart items for the logged-in user
                var Order = _db.Orders.Include(i => i.OrderCartItems).ThenInclude(b => b.Book).Where(ci => ci.UserId == currentUserId).ToList();

                foreach (var order in Order)
                {
                    decimal orderTotal = 0;
                    foreach (var cartItem in order.OrderCartItems)
                    {
                        if (cartItem.Book != null)
                        {
                            orderTotal += cartItem.Quantity * (cartItem.Book.BookPrice);
                        }
                    }
                    order.TotalAmount = orderTotal;
                }


                return View(Order);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult Detail(int id)
        {
            string? currentUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var totalpriceorder = _db.Orders.Where(o=> o.UserId == currentUserId && o.OrderId == id).FirstOrDefault();
            ViewData["sumpriceorder"] = totalpriceorder?.TotalPrice;
            if (id != null && currentUserId != null)
            {
                IEnumerable<OrderCartItem> orderCartItems = _db.OrderCartItems
                        .Include(i => i.Book)
                        .Where(c => c.OrderId == id && c.Order.UserId == currentUserId)
                        .ToList();

                return View(orderCartItems);
            }
            else
            {
                return NoContent();
            }

        }
    }
}