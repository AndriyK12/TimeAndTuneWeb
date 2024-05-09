namespace TimeAndTuneWeb.Controllers
{
    using System.Diagnostics;
    using System.Security.Claims;
    using EFCore.Service;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;
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
            Log.Information("Loading HomePage tasks table");
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

            var tasks = _taskProvider.GetAllTasksByUserID(userId);

            return View(tasks);
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
