using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EFCore;
using TimeAndTuneWeb.ViewModels;
using EFCore.Service;
using Microsoft.EntityFrameworkCore;

namespace TimeAndTuneWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserProvider _userProvider;

        public AccountController(IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (_userProvider.isUserAlreadyExist(model.Email))
            {
                var user = _userProvider.getUserByEmail(model.Email);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    await Authenticate(user);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Incorrect login and (or) password");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!_userProvider.isUserAlreadyExist(model.Email))
            {
                var user = _userProvider.getUserByEmail(model.Email);
                if (user.Email == null)
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    _userProvider.addNewUser(model.Username, model.Email, hashedPassword);

                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "A user with this email address already exists");
            }
            return View(model);
        }

        private async System.Threading.Tasks.Task Authenticate(User user)
        {
            var claim = new List<Claim>()
            {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Userid.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);


            var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                /* IsPersistent = true*/ // Налаштуйте це за потребою
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
    
}
