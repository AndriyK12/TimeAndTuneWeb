namespace TimeAndTuneWeb.Controllers
{
    using System.Security.Claims;
    using System.Text.RegularExpressions;
    using Bogus;
    using EFCore;
    using EFCore.Service;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using SendingEmails;
    using Serilog;
    using TimeAndTuneWeb.ViewModels;

    public class AccountController : Controller
    {
        private readonly IUserProvider _userProvider;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<HomeController> _logger;


        public AccountController(IUserProvider userProvider, IEmailSender emailSender, ILogger<HomeController> logger)
        {
            this._userProvider = userProvider;
            this._emailSender = emailSender;
            this._logger = logger;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public static bool IsEmail(string input)
        {
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                var routeValues = new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "Index" },
                        { "period", "month" },
                    };

                return this.RedirectToAction("Index", "Home", routeValues);
            }

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Password))
            {
                return this.View(model);
            }

            if (!IsEmail(model.Email))
            {
                this.ModelState.AddModelError(string.Empty, "Please enter valid email.");
                Log.Warning("Were entered a value that is nat an email");
                return this.View(model);
            }

            if (this._userProvider.isUserAlreadyExist(model.Email))
            {
                var user = this._userProvider.getUserByEmail(model.Email);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    await this.Authenticate(user);
                    var routeValues = new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "Index" },
                        { "period", "month" },
                    };
                    Log.Information("Logging in");
                    return this.RedirectToAction("Index", "Home", routeValues);
                }

                this.ModelState.AddModelError(string.Empty, "Incorrect login and (or) password");
                Log.Warning("Incorrect login and (or) password");
                return this.View(model);
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "There's no such user in system.");
                Log.Warning("There's no such user in system.");
                return this.View(model);
            }

            return this.View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                var routeValues = new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "Index" },
                        { "period", "month" },
                    };

                return this.RedirectToAction("Index", "Home", routeValues);
            }

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Username) && string.IsNullOrWhiteSpace(model.Password) && string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                return this.View(model);
            }

            if (!IsEmail(model.Email))
            {
                this.ModelState.AddModelError(string.Empty, "Please enter valid email.");
                Log.Warning("Were entered a value that is nat an email");
                return this.View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                this.ModelState.AddModelError(string.Empty, "Passwords do not match!");
                return this.View(model);
            }

            if (!this._userProvider.isUserAlreadyExist(model.Email))
            {
                var user = this._userProvider.getUserByEmail(model.Email);
                if (user.Email == null)
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    this._userProvider.addNewUser(model.Username, model.Email, hashedPassword);

                    user = this._userProvider.getUserByEmail(model.Email);
                    await this.Authenticate(user);

                    var routeValues = new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "Index" },
                        { "period", "month" },
                    };

                    var receiver = user.Email;
                    var subject = "Welcome!";
                    var message = $"Hello, {user.Username}!\nWe are glad to inform you that your registration was successful! Welcome to TimeAndTune!";

                    Log.Information("Sending greetings");
                    await this._emailSender.SendEmailAsync(receiver, subject, message);
                    Log.Information("Greetings were sent");

                    Log.Information("Registered successfully");
                    return this.RedirectToAction("Index", "Home", routeValues);
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "A user with this email address already exists");
                    Log.Warning("A user with this email address already exists");
                    return this.View(model);
                }
            }
            else
            {
                this.ModelState.AddModelError(string.Empty, "A user with this email address already exists");
                Log.Warning("A user with this email address already exists");
                return this.View(model);
            }

            return this.View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                var routeValues = new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "Index" },
                        { "period", "month" },
                    };

                return this.RedirectToAction("Index", "Home", routeValues);
            }

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!IsEmail(model.Email))
            {
                this.ModelState.AddModelError(string.Empty, "Please enter valid email.");
                Log.Warning("Were entered a value that is nat an email");
                return this.View(model);
            }

            var user = this._userProvider.getUserByEmail(model.Email);
            if (user.Email == null)
            {
                this.ModelState.AddModelError(string.Empty, "There's no such user in system.");
                Log.Warning("There's no such user in system.");
                return this.View(model);
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

                    await this._emailSender.SendEmailAsync(receiver, subject, message);
                }
                catch
                {
                    this.ModelState.AddModelError(string.Empty, "Error changing password.");
                    Log.Warning("Error changing password.");
                }
            }

            return this.RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult NewPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                var routeValues = new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "Index" },
                        { "period", "month" },
                    };

                return this.RedirectToAction("Index", "Home", routeValues);
            }

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewPassword(NewPasswordViewModel model)
        {
            var user = this._userProvider.getUserByEmail(model.Email);

            if (user.Email == null)
            {
                this.ModelState.AddModelError(string.Empty, "There's no such user in system.");
                return this.View(model);
            }
            else
            {
                DatabaseUserProvider userService = new DatabaseUserProvider();
                var password = model.OldPassword;
                var newPassword = model.Password;
                var confirmNewPassword = model.ConfirmPassword;

                if (newPassword != confirmNewPassword)
                {
                    this.ModelState.AddModelError(string.Empty, "New and confirmation passwords should be equal");
                    return this.View(model);
                }

                if (password == newPassword)
                {
                    this.ModelState.AddModelError(string.Empty, "Old and new passwords can not be identical!");
                    return this.View(model);
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                try
                {
                    userService.changePassword(user.Userid, hashedPassword);
                }
                catch
                {
                    this.ModelState.AddModelError(string.Empty, "Error changing password.");
                }
            }

            return this.RedirectToAction("Login", "Account");
        }

        private async System.Threading.Tasks.Task Authenticate(User user)
        {
            var claim = new List<Claim>()
            {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Userid.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
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
            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToAction("Login", "Account");
        }
    }
}
