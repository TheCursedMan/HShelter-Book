using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Responsive.Data;
using Responsive.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Responsive.Controllers
{
    public class Paymentctl : Controller
    {
        private readonly BookDBContext _db;
        public Paymentctl(BookDBContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            string? currentuser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(currentuser))
            {
                List<Payment> allpayment = _db.Payments.Where(u => u.UserId.Equals(currentuser)).ToList();
                return View(allpayment);

            }
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment paymentitem)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelStateValue in ModelState.Values)
                {
                    foreach (var error in modelStateValue.Errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        // Log or debug the error messages to identify the issue.
                        ViewData["PaymentError"] = errorMessage;
                    }
                }

                return View(paymentitem); // Return the view with the invalid model.
            }
            string? currentuser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var newpaymentitem = new Payment
            {
                PaymentType = paymentitem.PaymentType,
                PaymentNumber = paymentitem.PaymentNumber,
                HolderName = paymentitem.HolderName,
                PhoneNumber = paymentitem.PhoneNumber,  
                UserId = currentuser
            };
            // If the model is valid, proceed to save to the database.
            await _db.Payments.AddAsync(newpaymentitem);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var cusdb = _db.Payments.Find(Id);
            if (cusdb == null)
            {
                return NotFound();
            }
            return View(cusdb);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Payment paymentitem)
        {
            if (ModelState.IsValid)
            {
                string? currentuser = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var newpaymentitem = new Payment
                {
                    PaymentId = paymentitem.PaymentId,
                    PaymentType = paymentitem.PaymentType,
                    PaymentNumber = paymentitem.PaymentNumber,
                    HolderName = paymentitem.HolderName,
                    PhoneNumber = paymentitem.PhoneNumber,
                    UserId = currentuser
                };
                _db.Payments.Update(newpaymentitem);
                await _db.SaveChangesAsync();
                TempData["PaymentResult"] = "Payment Updated!!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(paymentitem);

            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.Books == null)
            {
                return NotFound();
            }
            var paymentdb = await _db.Payments.FirstOrDefaultAsync(b => b.PaymentId == id);
            if (paymentdb == null)
            {
                return NotFound();
            }
            return View(paymentdb);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_db.Payments == null)
            {
                return Problem("Entity Dbcontext.Payments is null ");
            }
            var paymentitem = await _db.Payments.FindAsync(id);
            if (paymentitem != null)
            {
                _db.Payments.Remove(paymentitem);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
