using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Responsive.Data;
using Responsive.Models;
using System.Security.Claims;

namespace Responsive.Controllers
{
    public class Addressctl : Controller
    {
        private readonly BookDBContext _db;
        public Addressctl(BookDBContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            string? currentuser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;    
            if (!string.IsNullOrWhiteSpace(currentuser))
            {
                List<UserAddress> alladdresses = _db.Addressesdb.Where(u => u.UserId.Equals(currentuser)).ToList();
                return View(alladdresses);

            }
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserAddress addressitem)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelStateValue in ModelState.Values)
                {
                    foreach (var error in modelStateValue.Errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        // Log or debug the error messages to identify the issue.
                        ViewData["AddressError"] = errorMessage;
                    }
                }

                return View(addressitem); // Return the view with the invalid model.
            }
            string? currentuser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var newaddressitem = new UserAddress
            {
                CustomerName = addressitem.CustomerName,
                Address = addressitem.Address,
                PostalCode = addressitem.PostalCode,
                PhoneNumber = addressitem.PhoneNumber,
                UserId = currentuser,
            };
            // If the model is valid, proceed to save to the database.
            await _db.Addressesdb.AddAsync(newaddressitem);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var cusdb = _db.Addressesdb.Find(Id);
            if (cusdb == null)
            {
                return NotFound();
            }
            return View(cusdb);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserAddress addressitem)
        {
            if(ModelState.IsValid)
            {
                string? currentuser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var newaddressitem = new UserAddress
                {
                    AddressId = addressitem.AddressId,
                    CustomerName = addressitem.CustomerName,
                    Address = addressitem.Address,
                    PostalCode = addressitem.PostalCode,
                    PhoneNumber = addressitem.PhoneNumber,
                    UserId = currentuser,
                };
                _db.Addressesdb.Update(newaddressitem);
                await _db.SaveChangesAsync();
                TempData["AddressResult"] = "Address Updated!!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(addressitem);

            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.Books == null)
            {
                return NotFound();
            }
            var bookdb = await _db.Addressesdb.FirstOrDefaultAsync(b => b.AddressId == id);
            if (bookdb == null)
            {
                return NotFound();
            }
            return View(bookdb);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_db.Addressesdb == null)
            {
                return Problem("Entity Dbcontext.Addressdb is null ");
            }
            var addressitem = await _db.Addressesdb.FindAsync(id);
            if (addressitem != null)
            {
                _db.Addressesdb.Remove(addressitem);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
