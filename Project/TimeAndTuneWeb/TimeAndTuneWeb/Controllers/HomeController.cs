namespace TimeAndTuneWeb.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using TimeAndTuneWeb.Models;

    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> logger = logger;

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
