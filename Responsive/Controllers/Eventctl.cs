using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Responsive.Data;
using Responsive.Models;

namespace Responsive.Controllers
{
    public class Eventctl : Controller
    {
        private readonly BookDBContext _db;

        /* For image uploader*/
        private readonly IWebHostEnvironment _hostEnvironment;
        public Eventctl(BookDBContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            this._hostEnvironment = hostEnvironment;
        }

        /*For image uploader*/
     /*   public IActionResult Index()
        {
            IEnumerable<Event> allEvents = _db.Events;
            return View(allEvents);
        }*/

        [Authorize(Roles = "Admin")]
        public IActionResult List()
        {
            IEnumerable<Event> allevents = _db.Events;
            return View(allevents); 
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != null)
            {
                Event Event = _db.Events.FirstOrDefault(i => i.EventId == id);

                if (Event != null)
                {
                    return View(Event);
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
        public async Task<IActionResult> Create(Event events)
        {
            if (ModelState.IsValid)
            {
                if (events.EventImg != null)
                {
                    /* Save to file img*/
                    string imgpath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(events.EventImg.FileName);
                    string extension = Path.GetExtension(events.EventImg.FileName);
                    events.EventImgName = fileName + extension;
                    string path = Path.Combine(imgpath, "img", events.EventImgName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await events.EventImg.CopyToAsync(fileStream);
                    }
                }
                _db.Events.Add(events);
                await _db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(events);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var cusdb = _db.Events.Find(Id);
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
        public async Task<IActionResult> Edit(Event events, int id)
        {
            if (ModelState.IsValid)
            {
                events.EventId = id;

                if (events.EventImg != null)
                {
                    if (!string.IsNullOrEmpty(events.EventImgName))
                    {
                        string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, "img", events.EventImgName);

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    // Save new image
                    string imgPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(events.EventImg.FileName);
                    string extension = Path.GetExtension(events.EventImg.FileName);
                    events.EventImgName = fileName + extension;
                    string path = Path.Combine(imgPath, "img", events.EventImgName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await events.EventImg.CopyToAsync(fileStream);
                    }
                }

                // Update all properties
                _db.Events.Update(events);
                await _db.SaveChangesAsync();
                TempData["ResultOk"] = "Data Updated!";
                return RedirectToAction("List");
            }
            else
            {
                // Exclude specific properties from being marked as modified
                _db.Entry(events).Property(x => x.EventImg).IsModified = false;
                _db.Entry(events).Property(x => x.EventImgName).IsModified = false;

                await _db.SaveChangesAsync();
                TempData["ResultOk"] = "Data Updated!";
                return RedirectToAction("List");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.Events == null)
            {
                return NotFound();
            }
            var newsdb = await _db.Events.FirstOrDefaultAsync(b => b.EventId == id);
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
            if (_db.Events == null)
            {
                return Problem("Entity Dbcontext.Newsdb is null ");
            }
            var news = await _db.Events.FindAsync(id);
            if (!string.IsNullOrEmpty(news.EventImgName))
            {
                //delete img
                var imgpath = Path.Combine(_hostEnvironment.WebRootPath, "img", news.EventImgName);
                if (System.IO.File.Exists(imgpath))
                {
                    System.IO.File.Delete(imgpath);
                }
            }
            if (news != null)
            {
                _db.Events.Remove(news);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("List");
        }
        private bool BookExist(int id)
        {
            return (_db.Events?.Any(b => b.EventId == id)).GetValueOrDefault();
        }
    }
}
