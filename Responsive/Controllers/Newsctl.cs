using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Responsive.Data;
using Responsive.Models;

namespace Responsive.Controllers
{
    public class Newsctl : Controller
    {
        private readonly BookDBContext _db;

        /* For image uploader*/
        private readonly IWebHostEnvironment _hostEnvironment;
        public Newsctl(BookDBContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            this._hostEnvironment = hostEnvironment;
        }

        /*For image uploader*/
        public IActionResult Index()
        {
            List<Book> NewsBooks = _db.Books
                                    .Where(nb => nb.BookCategory.CategoryName == "New Book")
                                    .Take(3)
                                    .ToList();
            ViewData["New Book"] = NewsBooks;
            IEnumerable<News> allNews = _db.Newsdb;
            IEnumerable<Event> allEvents = _db.Events;
            var newsmodel = new NewsEventCaller
            {
                AllNews = allNews,
                Events = allEvents,
            };
            return View(newsmodel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult List()
        {
            IEnumerable<News> allnews = _db.Newsdb;
            return View(allnews);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != null)
            {
                News news = _db.Newsdb.FirstOrDefault(i => i.NewsId == id);

                if (news != null)
                {
                    return View(news);
                }
                else
                {
                    return NoContent();
                }
            }
            else
            {
                return NotFound();
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(News news)
        {
            if (ModelState.IsValid)
            {
                if (news.NewsImg != null)
                {
                    /* Save to file img*/
                    string imgpath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(news.NewsImg.FileName);
                    string extension = Path.GetExtension(news.NewsImg.FileName);
                    news.NewsImgName = fileName + extension;
                    string path = Path.Combine(imgpath, "img", news.NewsImgName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await news.NewsImg.CopyToAsync(fileStream);
                    }
                }
                _db.Add(news);
                await _db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(news);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var cusdb = _db.Newsdb.Find(Id);
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
        public async Task<IActionResult> Edit(News news, int id)
        {
            if (ModelState.IsValid)
            {
                news.NewsId = id;

                if (news.NewsImg != null)
                {
                    if (!string.IsNullOrEmpty(news.NewsImgName))
                    {
                        string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, "img", news.NewsImgName);

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    // Save new image
                    string imgPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(news.NewsImg.FileName);
                    string extension = Path.GetExtension(news.NewsImg.FileName);
                    news.NewsImgName = fileName + extension;
                    string path = Path.Combine(imgPath, "img", news.NewsImgName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await news.NewsImg.CopyToAsync(fileStream);
                    }
                }

                // Update all properties
                _db.Newsdb.Update(news);
                await _db.SaveChangesAsync();
                TempData["ResultOk"] = "Data Updated!";
                return RedirectToAction("List");
            }
            else
            {
                // Exclude specific properties from being marked as modified
                _db.Entry(news).Property(x => x.NewsImg).IsModified = false;
                _db.Entry(news).Property(x => x.NewsImgName).IsModified = false;

                await _db.SaveChangesAsync();
                TempData["ResultOk"] = "Data Updated!";
                return RedirectToAction("List");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.Newsdb == null)
            {
                return NotFound();
            }
            var newsdb = await _db.Newsdb.FirstOrDefaultAsync(b => b.NewsId == id);
            if (newsdb == null)
            {
                return NotFound();
            }
            return View(newsdb);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_db.Newsdb == null)
            {
                return Problem("Entity Dbcontext.Newsdb is null ");
            }
            var news = await _db.Newsdb.FindAsync(id);
            if (!string.IsNullOrEmpty(news.NewsImgName))
            {
                //delete img
                var imgpath = Path.Combine(_hostEnvironment.WebRootPath, "img", news.NewsImgName);
                if (System.IO.File.Exists(imgpath))
                {
                    System.IO.File.Delete(imgpath);
                }
            }
            if (news != null)
            {
                _db.Newsdb.Remove(news);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("List");
        }
        private bool BookExist(int id)
        {
            return (_db.Newsdb?.Any(b => b.NewsId == id)).GetValueOrDefault();
        }

    }
}
