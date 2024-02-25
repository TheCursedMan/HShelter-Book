using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Responsive.Data;
using Responsive.Models;
using System.Net;
using System.Security.Claims;

namespace Responsive.Controllers
{
    public class Cartctl : Controller
    {
        private readonly BookDBContext _db;
        public Cartctl(BookDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            string? currentUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserAddress = _db.Addressesdb.FirstOrDefault(ad => ad.UserId == currentUserId);
            ViewData["userAddress"] = currentUserAddress;
            IEnumerable<CartItem> cartItems;

            if (TempData["CartItems"] != null && TempData["CartItems"] is List<CartItem> tempCartItems)
            {
                cartItems = tempCartItems;
            }
            else
            {
                cartItems = _db.CartItems.Include(ci => ci.Book).Where(ci => ci.UserId == currentUserId).ToList();
            }

            return View(cartItems);
        }




        [HttpPost]
        public IActionResult AddToCart(int Bookid)
        {

            string? currentUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var book = _db.Books.FirstOrDefault(b => b.BookId == Bookid);

            if (book != null)
            {
                var cartItem = new CartItem
                {
                    BookId = book.BookId,
                    Book = book,
                    Quantity = 1,
                    UserId = currentUserId
                };

                _db.CartItems.Add(cartItem);
                _db.SaveChanges();
                return Ok(new { success = true });

            }
            return BadRequest(new { error = "Failed to add book to cart" });
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int newQuantity)
        {
            var cartItem = _db.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;
                _db.SaveChanges();
                return RedirectToAction("Index"); // Return success response
            }

            return NotFound(); // Return error response
        }

        [HttpPost]
        public IActionResult Delete(int cartItemId)
        {
            var cartItem = _db.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
            if (cartItem != null)
            {
                _db.CartItems.Remove(cartItem);
                _db.SaveChanges();
                return RedirectToAction("Index"); // Return success response
            }

            return NotFound(); // Return error response
        }

        [HttpPost]
        public IActionResult ConfirmOrder(decimal totalprice)
        {

            string? currentUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var allItems = _db.CartItems.Where(ci => ci.UserId == currentUserId).ToList();

            if (allItems.Any())
            {
                var newOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    UserId = currentUserId,
                    TotalPrice = (float)totalprice + 50
                    // Other Order properties...
                };

                _db.Orders.Add(newOrder);
                _db.SaveChanges();

                foreach (var cartItem in allItems)
                {
                    var orderCartItem = new OrderCartItem
                    {
                        OrderId = newOrder.OrderId,
                        BookId = cartItem.BookId,
                        Quantity = cartItem.Quantity
                    };

                    _db.OrderCartItems.Add(orderCartItem);
                }

                _db.SaveChanges();

                // Clear cart items after confirming the order
                _db.CartItems.RemoveRange(allItems);
                _db.SaveChanges();

                TempData["CartMessageSuccess"] = "Order Created Successfully!!";
                // ... (rest of your code remains unchanged)
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["CartMessageFailed"] = "No items in the cart";
                return View();
            }
        }





    }
}