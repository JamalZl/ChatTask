using ChatTask.DAL;
using ChatTask.Models;
using ChatTask.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChatTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly  ChatDbContext _context;

        public HomeController(UserManager<AppUser> userManager,ChatDbContext context, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        public  IActionResult CreateUser()
        {
            AppUser user1 = new AppUser { FullName = "Camal Zeynalli", UserName = "JamalZl" };
            AppUser user2 = new AppUser { FullName = "Hasan Nuruzada", UserName = "HasanNuru" };
            AppUser user3 = new AppUser { FullName = "Ahad Taghiyev", UserName = "Ahad085" };
            AppUser user4 = new AppUser { FullName = "Nurlan Mammadli", UserName = "Nurlanmle" };

            var resul1 = _userManager.CreateAsync(user1, "@User123").Result;
            var resul2 = _userManager.CreateAsync(user2, "@User123").Result;
            var resul3 = _userManager.CreateAsync(user3, "@User123").Result;
            var resul4 = _userManager.CreateAsync(user4, "@User123").Result;
            return Content("Created");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByNameAsync(loginVM.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "Password or UserName is incorrect");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, true, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Password or UserName is incorrect");
                return View();
            }
            return RedirectToAction("chat");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login");
        }
        public IActionResult Index()
        {
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

        public IActionResult Chat()
        {
            List<AppUser> model = _userManager.Users.ToList();
            return View(model);
        }
    }
}
