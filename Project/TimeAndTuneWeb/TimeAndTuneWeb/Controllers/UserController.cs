using Microsoft.AspNetCore.Mvc;
using EFCore;
using EFCore.Service;

namespace TimeAndTuneWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserProvider _userProvider;

        public UserController(IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(string email)
        {
            var user = _userProvider.getUserByEmail("johndoe@example.com");
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
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

