using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Responsive.Data;
using Responsive.Models;
using System.Diagnostics;

namespace Responsive.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookDBContext _db;
        public HomeController(BookDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            //top 5 showing
            List<Book> books = _db.Books.Where(r=> r.BookRank != null && r.BookRank <= 5).OrderBy(r => r.BookRank).ToList();
            ViewData["BookRank"] = books;
            //sales showing
            List<Book> salesbook = _db.Books.Where(s=>s.IsDiscount == true && s.DiscountPer > 0 ).ToList();
            ViewData["SalesBook"] = salesbook;

            List<Book> HealingBook = _db.Books.Where(h=>h.TypeId == 3).ToList();
            ViewData["HealingBook"] = HealingBook;
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}