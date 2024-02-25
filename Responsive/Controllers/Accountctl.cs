using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Responsive.Data;
using Responsive.Models;
namespace Responsive.Controllers
{
    public class Accountctl : Controller
    {
        private readonly BookDBContext _db;
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        public Accountctl(BookDBContext db, UserManager<Account> userManager, SignInManager<Account> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            var response = new Login();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                //When found user check password
                var passwordcheck = await _userManager.CheckPasswordAsync(user, model.Password);
                if (passwordcheck)
                {
                    //if password correct sign in 
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "UserDetail");
                    }

                }
                //Password incorrect
                TempData["Error"] = "Wrong password Try again";
                return View(model);
            }
            //User not found
            TempData["Error"] = "User Not Found";
            return View(model);
        }

        public IActionResult Register()
        {
            var response = new Register();
            return View(response);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register registervm)
        {
            if (!ModelState.IsValid) return View(registervm);
            var user = await _userManager.FindByEmailAsync(registervm.Email);
            //when email is already has in database
            if (user != null)
            {
                TempData["Error"] = "This Email is already in use";
                return View(registervm);
            }
            //Validate Password (Really important that did me stuck for a day that cause it isn't throw any error)
            var PasswordValidateResult = await _userManager.PasswordValidators.First().ValidateAsync(_userManager, null, registervm.Password);
            if (!PasswordValidateResult.Succeeded)
            {
                foreach (var error in PasswordValidateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registervm);
            }

            //when email can be use create obj for add to database
            var newUser = new Account()
            {
                //define value with input
                Email = registervm.Email,
                UserName = registervm.Email,
                CustomerName = registervm.CustomerName,
                CustomerSurname = registervm.CustomerSurname,
                BirthDate = registervm.BirthDate,
                Gender = registervm.Gender
            };
            //add to database with obj + password
            var newResponse = await _userManager.CreateAsync(newUser, registervm.Password);
            if (newResponse.Succeeded)
            {
                //Add User role 
                try
                {
                    await _userManager.AddToRoleAsync(newUser, UserRole.User);
                }
                catch (Exception ex)
                {

                }
                return RedirectToAction("Login");
            }
            else
            {
                TempData["Error"] = "Error creating the user";
                return View(registervm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> logout()
        {
            await _signInManager.SignOutAsync();

            // Update LastLogoutTime in the database
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                await _userManager.UpdateAsync(currentUser);
            }

            return RedirectToAction("Index", "Home");
        }

    }
}