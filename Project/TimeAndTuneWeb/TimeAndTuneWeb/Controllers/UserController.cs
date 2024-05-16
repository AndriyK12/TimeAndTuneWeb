// <copyright file="UserController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TimeAndTuneWeb.Controllers
{
    using EFCore.Service;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserProvider _userProvider;

        public UserController(IUserProvider userProvider)
        {
            this._userProvider = userProvider;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Details(string email)
        {
            var user = this._userProvider.getUserByEmail("johndoe@example.com");
            if (user == null)
            {
                return this.NotFound();
            }

            return this.View(user);
        }

        //// GET: User/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: User/Create
        //[HttpPost]
        //public ActionResult Create(User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _userProvider.addNewUser(user.Username, user.Email, user.Password);
        //        return RedirectToAction("Index");
        //    }
        //    return View(user);
        //}

        //// GET: User/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    var user = _userProvider.getUserById(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        //// POST: User/Edit/5
        //[HttpPost]
        //public ActionResult Edit(User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _userProvider.updateUser(user);
        //        return RedirectToAction("Index");
        //    }
        //    return View(user);
        //}

        //// GET: User/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    var user = _userProvider.getUserById(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        //// POST: User/Delete/5
        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    _userProvider.deleteUser(id);
        //    return RedirectToAction("Index");
        //}
    }
}

