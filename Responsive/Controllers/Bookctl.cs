using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Responsive.Data;
using Responsive.Models;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Responsive.Controllers
{
    public class Bookctl : Controller
    {
        private readonly BookDBContext _db;
        private readonly UserManager<Account> _userManager;

        /* For image uploader*/
        private readonly IWebHostEnvironment _hostEnvironment;
        public Bookctl(BookDBContext db, IWebHostEnvironment hostEnvironment , UserManager<Account> userManager)
        {
            _db = db;
            this._hostEnvironment = hostEnvironment;
            _userManager = userManager; //
        }


        public IActionResult Search(string querry , string catquerry)
        {
            List<BookCategory> allcategories = _db.BookCategories.ToList();
            ViewData["Categories"] = allcategories;
            if (String.IsNullOrWhiteSpace(querry))
            {

                return View(_db.Books.ToList());
            }
            else
            {
                return View(_db.Books.Where(nam => nam.BookName.Contains(querry)).ToList()); 
            }
            
        }
        

        public IActionResult Index(int page = 1 ,int typeId = 0 , int catId = 0)
        {
            int itemsPerPage = 20; // Number of items to display per page
            int totalItems = _db.Books.Count(); // Replace Books with your actual DbSet name
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            // Ensure page is within valid range
            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }

            int skipAmount = (page - 1) * itemsPerPage;
            var booksOnCurrentPage = _db.Books
                                        .OrderBy(b => b.BookId) // Change this ordering as needed
                                        .Where(i => i.TypeId == typeId || i.CategoryId == catId)
                                        .Skip(skipAmount)
                                        .Take(itemsPerPage)
                                        .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.ItemsPerPage = itemsPerPage;
            List<BookCategory> relatedCategories = _db.BookCategories
                                                    .Where(category => category.Books.Any(book => book.TypeId == typeId))
                                                    .ToList();
            ViewData["Categories"] = relatedCategories;
            ViewData["typeId"] = typeId;
            return View(booksOnCurrentPage);
        }


        [HttpPost]
        public IActionResult Filter(List<int> filterId, int page = 1  )
        {
            int itemsPerPage = 20; // Number of items to display per page after filtering

            


            // Retrieve all categories to pass them to the view
            List<BookCategory> allcategories = _db.BookCategories.ToList();
            ViewData["Categories"] = allcategories;

            IQueryable<Book> filteredBooksQuery = _db.Books;

            if (filterId != null && filterId.Any())
            {
                filteredBooksQuery = filteredBooksQuery.Where(book => filterId.Contains((int)book.CategoryId));
            }
            int totalItems = filteredBooksQuery.Count(); // Total items after filtering

            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            // Ensure the page is within the valid range
            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }

            int skipAmount = (page - 1) * itemsPerPage;

            var filteredBooks = filteredBooksQuery
                                    .OrderBy(b => b.BookId) // Change this ordering as needed
                                    .Skip(skipAmount)
                                    .Take(itemsPerPage)
                                    .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.ItemsPerPage = itemsPerPage;

            return PartialView("_FilterBook", filteredBooks);
        }


        public IActionResult List()
        {
            List<Book> allbooks = _db.Books
                            .Include(bc => bc.BookCategory)
                            .Include(bt => bt.BookType) // Include BookType
                            .ToList();
            return View(allbooks);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if(id != null)
            {
                Book book = _db.Books.Include(a => a.BookCategory).FirstOrDefault(c => c.BookId == id);

           /*     var allReviewsForBook = _db.Reviews.Where(r => r.BookId == id).ToList();*/

                //check user comment
                var user = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var usercomment = _db.Reviews.FirstOrDefault(i => i.BookId == id && i.UserId == user);
                if(usercomment != null)
                {
                    ViewData["UserReview"] = 1;
                }
                else
                {
                    ViewData["UserReview"] = 0;
                }
                //all reviewer
                try
                {
                    var allreviewer = _db.Reviews.Include(r => r.Account).Where(r => r.BookId == id).ToList();
                    //split logged user
                    var CurrentUserReview = allreviewer.FirstOrDefault(r => r.UserId == user);
                    allreviewer.Remove(CurrentUserReview);

                    //List name reviewer
                    var OtherReviewWithUsername = new List<(Review Review, string Username)>();
                    foreach (var review in allreviewer)
                    {
                        var User = await _userManager.FindByIdAsync(review.UserId);
                        if (User != null)
                        {
                            OtherReviewWithUsername.Add((review, User.CustomerName));
                        }
                    }

                    //prepare data for show logged in user comment first
                    ViewData["CurrentUserReview"] = CurrentUserReview;
                    ViewData["AllReviews"] = OtherReviewWithUsername;
                }
                catch
                {
                    ViewData["AllReviews"] = null;
                }
                return View(book);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Review(Review review)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    var usercomment = _db.Reviews.FirstOrDefault(i => i.BookId == review.BookId && i.UserId == user.Id);

                    if (user != null && usercomment == null )
                    {
                        var reviewItem = new Review
                        {
                            Comment = review.Comment,
                            Rating = review.Rating,
                            ReviewDate = DateTime.Now,
                            BookId = review.BookId,
                            UserId = user.Id,
                        };
                        _db.Add(reviewItem);
                        _db.SaveChanges();
                        return RedirectToAction("Detail", new { id = review.BookId });
                    }
                    else
                    {
                        ViewData["UserReview"] = 1;
                    }
                }

                return NotFound();
            }
            catch
            {
                return NotFound();
            }
        }
        private bool BookExist(int id)
        {
            return (_db.Books?.Any(b => b.BookId == id)).GetValueOrDefault();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CategoryList()
        {
            List<BookCategory> allbookcategory = _db.BookCategories.ToList();
            return View(allbookcategory);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateCategory(BookCategory catitem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Add(catitem);
                    _db.SaveChanges();
                    return RedirectToAction("CategoryList");
                }
                else
                {
                    // ModelState is not valid, retrieve validation errors for each field
                    foreach (var modelStateKey in ModelState.Keys)
                    {
                        var errors = ModelState[modelStateKey].Errors;
                        foreach (var error in errors)
                        {
                            // Log or handle validation error messages here
                            var errorMessage = error.ErrorMessage;
                            var exception = error.Exception; // If any exception occurred during validation

                            // You can log or process these error messages as needed
                            // For debugging purposes, you can use Console.WriteLine or a logger to print these errors
                            Console.WriteLine($"Field: {modelStateKey}, Error: {errorMessage}");
                        }
                    }

                    // Return the view with errors
                    return View(catitem);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Log.Error(ex, "Error occurred while creating category");

                // Return an error view or handle the exception as needed
                return View("Error");
            }
        }



        [Authorize(Roles = "Admin")]
        public IActionResult EditCategory(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var cusdb = _db.BookCategories.Find(Id);
            if (cusdb == null)
            {
                return NotFound();
            }
            return View("EditCategory" , cusdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCategory(BookCategory bookCategorydata)
        {
           _db.BookCategories.Update(bookCategorydata);
            await _db.SaveChangesAsync();
            TempData["ResultOk"] = "Data Updated!";
            return RedirectToAction("CategoryList");
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _db.BookCategories.FirstOrDefault(ci => ci.CategoryId == id);
            if (category != null)
            {
                _db.BookCategories.Remove(category);
                _db.SaveChanges();
                return RedirectToAction("CategoryList"); 
            }

            return RedirectToAction("CategoryList");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            List<BookCategory> bookCategories = _db.BookCategories.ToList();
            ViewData["BookCategoryForm"] = bookCategories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Book book)
        {
            var TypeCheck = _db.BookTypes.FirstOrDefault(b => b.TypeId == book.TypeId);
            if (TypeCheck == null)
            {
                ModelState.AddModelError("TypeId", "Please select Book Type");
            }
            var Categorycheck = _db.BookCategories.FirstOrDefault(b => b.CategoryId == book.CategoryId);
            if (Categorycheck == null) 
            {
                ModelState.AddModelError("CategoryId", "Please select Category");
            }

            if (book.IsDiscount == false && book.DiscountPer.HasValue)
            {
                ModelState.AddModelError("DiscountPer", "Discount percentage should not be set when IsDiscount is false.");
            }
            else if(book.IsDiscount == true && !book.DiscountPer.HasValue)
            {
                ModelState.AddModelError("DiscountPer", "Please type value or uncheck isDiscount");
            }
            if (ModelState.IsValid)
            {
                var existingBookWithRank = _db.Books.FirstOrDefault(b => b.BookRank == book.BookRank);

                if (existingBookWithRank != null && book.BookRank != null)
                {
                    ModelState.AddModelError("BookRank", "This rank is already assigned to another book.");
                    return View(book);
                }
                if (book.BookImg != null)
                {
                    /* Save to file img*/
                    string imgpath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(book.BookImg.FileName);
                    string extension = Path.GetExtension(book.BookImg.FileName);
                    book.BookImgName = fileName + extension;
                    string path = Path.Combine(imgpath, "img", book.BookImgName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await book.BookImg.CopyToAsync(fileStream);
                    }
                }
                _db.Add(book);
                await _db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(book);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? Id)
        {
            List<BookCategory> bookCategories = _db.BookCategories.ToList();
            ViewData["BookCategoryForm"] = bookCategories;
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var cusdb = _db.Books.Find(Id);
            if (cusdb == null)
            {
                return NotFound();
            }
            return View(cusdb);
        }

        //edit ของใหม่
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Book book, int id)
        {
            if (book.IsDiscount == false && book.DiscountPer.HasValue)
            {
                ModelState.AddModelError("DiscountPer", "Discount percentage should not be set when IsDiscount is false.");
            }
            else if (book.IsDiscount == true && !book.DiscountPer.HasValue)
            {
                ModelState.AddModelError("DiscountPer", "Please type value or uncheck isDiscount");
            }
            if (ModelState.IsValid)
            {
                var existingBookWithRank = _db.Books.FirstOrDefault(b => b.BookRank == book.BookRank && b.BookId != book.BookId);


                if (existingBookWithRank != null && book.BookRank != null)
                {
                    ModelState.AddModelError("BookRank", "This rank is already assigned to another book.");
                    // Handle the error, return the view with errors, or take appropriate action
                    return View(book);
                }
                if (book.BookImg != null)
                {
                    if (!string.IsNullOrEmpty(book.BookImgName))
                    {
                        string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, "img", book.BookImgName);

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    // Save new image
                    string imgPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(book.BookImg.FileName);
                    string extension = Path.GetExtension(book.BookImg.FileName);
                    book.BookImgName = fileName + extension;
                    string path = Path.Combine(imgPath, "img", book.BookImgName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await book.BookImg.CopyToAsync(fileStream);
                    }
                }

                _db.Books.Update(book);
                await _db.SaveChangesAsync();
                TempData["ResultOk"] = "Data Updated!";
                return RedirectToAction("List");
            }


            return View(book);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.Books == null)
            {
                return NotFound();
            }
            var bookdb = await _db.Books.FirstOrDefaultAsync(b => b.BookId == id);
            if (bookdb == null)
            {
                return NotFound();
            }
            return View(bookdb);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _db.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound(); // Or handle as needed if the book doesn't exist
            }

            if (!string.IsNullOrEmpty(book.BookImgName))
            {
                // Delete the associated image file
                var imgpath = Path.Combine(_hostEnvironment.WebRootPath, "img", book.BookImgName);
                if (System.IO.File.Exists(imgpath))
                {
                    System.IO.File.Delete(imgpath);
                }
            }

            // Find and remove all related Reviews for the book
            var reviewsToDelete = _db.Reviews.Where(review => review.BookId == id).ToList();
            _db.Reviews.RemoveRange(reviewsToDelete);

            // Now find and remove all related OrderCartItems for the book
            var orderCartItemsToDelete = _db.OrderCartItems.Where(item => item.BookId == id).ToList();
            _db.OrderCartItems.RemoveRange(orderCartItemsToDelete);

            // Now find and remove all related CartItems for the book
            var cartItemsToDelete = _db.CartItems.Where(item => item.BookId == id).ToList();
            _db.CartItems.RemoveRange(cartItemsToDelete);

            // Finally, remove the book
            _db.Books.Remove(book);

            await _db.SaveChangesAsync();
            // ที่ต้องลบหมดก็เพราะว่าข้อมูลมันเชื่อมถึงกันเลยต้องลบออกนะ 
            return RedirectToAction("List");
        }




    }
}
