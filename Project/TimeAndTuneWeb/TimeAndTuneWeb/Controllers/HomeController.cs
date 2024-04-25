namespace TimeAndTuneWeb.Controllers
{
    using System.Diagnostics;
    using EFCore.Service;
    using Microsoft.AspNetCore.Mvc;
    using TimeAndTuneWeb.Models;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITaskProvider _taskProvider;

        public HomeController(ILogger<HomeController> logger, ITaskProvider taskProvider)
        {
            _logger = logger;
            _taskProvider = taskProvider;
        }

        public IActionResult Index()
        {
            var task = _taskProvider.getTaskById(1);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
            
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
