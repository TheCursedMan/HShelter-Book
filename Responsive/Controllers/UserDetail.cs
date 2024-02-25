using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Responsive.Data;
using Responsive.Models;

namespace Responsive.Controllers
{
    public class UserDetail : Controller
    {
        private readonly BookDBContext _db;
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        public UserDetail(BookDBContext db, UserManager<Account> userManager, SignInManager<Account> signInManager, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            this._hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = _userManager.GetUserAsync(User).Result;
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var users = await _userManager.Users.Where(x => x.Id.Equals(Id)).FirstOrDefaultAsync();
            return View(users);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Account account )
        {
            var user = await _userManager.FindByIdAsync(account.Id);

            if (user == null)
            {
                // Handle when user is not found
                return NotFound();
            }




            //Picture
            if (account.CustomerImg != null)
            {
                /* Save to file img*/
                string imgpath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(account.CustomerImg.FileName);
                string extension = Path.GetExtension(account.CustomerImg.FileName);
                account.CustomerImgName = fileName + extension;
                string path = Path.Combine(imgpath, "img", account.CustomerImgName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await account.CustomerImg.CopyToAsync(fileStream);
                }
            }

            // Modify user data
            user.CustomerName = account.CustomerName;
            user.Email = account.Email;
            user.PhoneNumber = account.PhoneNumber;
            user.Gender = account.Gender;   
            user.BirthDate = account.BirthDate; 
            user.CustomerImgName = account.CustomerImgName;

            // Update user using UserManager
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // User data updated successfully
                return RedirectToAction("Index");
            }
            else
            {
                // Handle errors in updating user data
                return View(account);
            }
        }


    }
}

