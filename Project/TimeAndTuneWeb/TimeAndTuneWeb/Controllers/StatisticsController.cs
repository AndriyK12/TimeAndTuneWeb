namespace TimeAndTuneWeb.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class StatisticsController : Controller
    {
        public IActionResult Statistics()
        {
            return View();
        }
    }
}
