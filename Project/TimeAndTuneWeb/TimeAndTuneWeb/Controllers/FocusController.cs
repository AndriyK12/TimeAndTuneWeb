using Microsoft.AspNetCore.Mvc;

namespace TimeAndTuneWeb.Controllers
{
    public class FocusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
