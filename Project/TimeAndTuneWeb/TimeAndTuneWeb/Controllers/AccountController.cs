using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EFCore;
using TimeAndTuneWeb.ViewModels;
using EFCore.Service;
using Microsoft.EntityFrameworkCore;
using SendingEmails;
using Bogus;

namespace TimeAndTuneWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserProvider _userProvider;
        private readonly IEmailSender _emailSender;


        public AccountController(IUserProvider userProvider, IEmailSender emailSender)
        {
            _userProvider = userProvider;
            this._emailSender = emailSender;
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
                    var routeValues = new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "Index" },
                        { "period", "month" }
                    };

                    return RedirectToAction("Index", "Home", routeValues);
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

                    user = _userProvider.getUserByEmail(model.Email);
                    await Authenticate(user);

                    var routeValues = new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "Index" },
                        { "period", "month" }
                    };

                    var receiver = user.Email;
                    var subject = "Welcome!";
                    var message = $"Hello, {user.Username}!\nWe are glad to inform you that your registration was successful! Welcome to TimeAndTune!";

                    await _emailSender.SendEmailAsync(receiver, subject, message);

                    return RedirectToAction("Index", "Home", routeValues);
                }

                ModelState.AddModelError("", "A user with this email address already exists");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = _userProvider.getUserByEmail(model.Email);
            if (user.Email == null)
            {
                ModelState.AddModelError("", "There's no such user in system.");
            }
            else
            {
                DatabaseUserProvider userService = new DatabaseUserProvider();
                var faker = new Faker();
                string password = faker.Internet.Password(10);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                try
                {
                    userService.changePassword(user.Userid, hashedPassword);
                    var receiver = user.Email;
                    var subject = "Password changed";
                    var message = $"Hello, {user.Username}!\nYour password was changed to {password}, please don't share it with anyone!";

                    await _emailSender.SendEmailAsync(receiver, subject, message);
                }
                catch
                {
                    ModelState.AddModelError("", "Error changing password.");
                }

            }

            return RedirectToAction("Login", "Account");
        }

        private async System.Threading.Tasks.Task Authenticate(User user)
        {
            var claim = new List<Claim>()
            {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Userid.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);


            var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                /* IsPersistent = true*/ // Налаштуйте це за потребою
            };

            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
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
