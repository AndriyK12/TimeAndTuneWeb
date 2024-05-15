namespace TimeAndTuneWeb.Controllers
{
    using EFCore.Service;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class FocusController : Controller
    {
        public IActionResult Focus()
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = -1;
            if (userIdClaim != null)
            {
                string userIdValue = userIdClaim.Value;

                try
                {
                    userId = int.Parse(userIdValue);
                }
                catch (FormatException ex)
                {
                }
            }
            DatabaseUserProvider userService = new DatabaseUserProvider();
            var userEmailClaim = HttpContext.User.FindFirst(ClaimTypes.Email);
            var userEmail = userEmailClaim.Value;
            var coinsAmount = userService.getCoinsAmount(userService.getUserByEmail(userEmail));
            ViewBag.Email = userEmail;
            ViewBag.CoinsAmount = coinsAmount;

            return View();
        }
    }
}
