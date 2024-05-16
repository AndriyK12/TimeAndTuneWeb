// <copyright file="FocusController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TimeAndTuneWeb.Controllers
{
    using System.Security.Claims;
    using EFCore.Service;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class FocusController : Controller
    {
        public IActionResult Focus()
        {
            var userIdClaim = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = -1;
            if (userIdClaim != null)
            {
                string userIdValue = userIdClaim.Value;

                try
                {
                    userId = int.Parse(userIdValue);
                }
                catch (FormatException)
                {
                }
            }

            DatabaseUserProvider userService = new DatabaseUserProvider();
            var userEmailClaim = this.HttpContext.User.FindFirst(ClaimTypes.Email);
            var userEmail = userEmailClaim.Value;
            var coinsAmount = userService.getCoinsAmount(userService.getUserByEmail(userEmail));
            this.ViewBag.Email = userEmail;
            this.ViewBag.CoinsAmount = coinsAmount;

            return this.View();
        }
    }
}
