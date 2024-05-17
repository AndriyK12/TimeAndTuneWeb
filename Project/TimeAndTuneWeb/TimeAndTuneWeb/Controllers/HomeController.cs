namespace TimeAndTuneWeb.Controllers
{
    using System.Diagnostics;
    using System.Security.Claims;
    using EFCore.Service;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;
    using TimeAndTuneWeb.Models;
    using TimeAndTuneWeb.ViewModels;
    using Newtonsoft.Json;

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITaskProvider _taskProvider;

        public HomeController(ILogger<HomeController> logger, ITaskProvider taskProvider)
        {
            this._logger = logger;
            this._taskProvider = taskProvider;
        }

        [Authorize]
        public IActionResult Index(string period = "month")
        {
            Log.Information("Loading HomePage tasks table");
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

            var tasks = this.GetTasksByPeriod(period, userId);
            DatabaseUserProvider userService = new DatabaseUserProvider();
            var userEmailClaim = this.HttpContext.User.FindFirst(ClaimTypes.Email);
            var userEmail = userEmailClaim.Value;
            var coinsAmount = userService.getCoinsAmount(userService.getUserByEmail(userEmail));
            this.ViewBag.Email = userEmail;
            this.ViewBag.CoinsAmount = coinsAmount;

            return this.View(tasks);
        }

        [HttpGet]
        public IActionResult AddNewTask()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTask(AddTaskViewModel model)
        {
            if (model.Name != null && model.Date != null && model.Priority != null)
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                int userId = -1;
                if (userIdClaim != null)
                {
                    string userIdValue = userIdClaim.Value;

                    try
                    {
                        userId = int.Parse(userIdValue);
                        _taskProvider.addNewTask(model.Name, model.Description, model.Date, model.Priority, userId);
                    }
                    catch (FormatException ex)
                    {
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult UpdateTask(UpdateTaskViewModel model, int temp = 1)
        {
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            var currentTask = taskService.getTaskById(model.Id);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(UpdateTaskViewModel model)
        {
            if (model.Id != null && model.Name != null && model.Date != null && model.Priority != null)
            {
                _taskProvider.updateTaskById(model.Id, model.Name, model.Description, model.Date.ToString(), model.Priority);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult UpdateExecutionTask()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateExecutionTask(UpdateTaskViewModel model)
        {
            int currentId = model.Id;
            if (Request.Cookies.TryGetValue($"TasksInfo{currentId}", out string cookieValue))
            {
                Tuple<DateTime, bool> currentTaskInfoValue = JsonConvert.DeserializeObject<Tuple<DateTime, bool>>(cookieValue);
                if (!currentTaskInfoValue.Item2)
                {
                    TimeSpan executionTime = DateTime.Now - currentTaskInfoValue.Item1;
                    EFCore.Task currentTask = _taskProvider.getTaskById(currentId);
                    if (currentTask.Executiontime != null)
                    {
                        _taskProvider.updateTaskExecutiontimeById(currentId, ((TimeSpan)currentTask.Executiontime + executionTime), false);
                    }
                    else
                    {
                        _taskProvider.updateTaskExecutiontimeById(currentId, executionTime, false);
                    }

                    Response.Cookies.Delete($"TasksInfo{currentId}");
                }
            }
            else
            {
                string tasksInfoJson = JsonConvert.SerializeObject(new Tuple<DateTime, bool>(DateTime.Now, false));
                Response.Cookies.Append($"TasksInfo{currentId}", tasksInfoJson);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult FinishTask()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FinishTask(UpdateTaskViewModel model)
        {
            int currentId = model.Id;
            EFCore.Task currentTask = _taskProvider.getTaskById(currentId);
            if (Request.Cookies.TryGetValue($"TasksInfo{currentId}", out string cookieValue))
            {
                Tuple<DateTime, bool> currentTaskInfoValue = JsonConvert.DeserializeObject<Tuple<DateTime, bool>>(cookieValue);
                if (!currentTaskInfoValue.Item2)
                {
                    TimeSpan executionTime = DateTime.Now - currentTaskInfoValue.Item1;
                    if (currentTask.Executiontime != null)
                    {
                        _taskProvider.updateTaskExecutiontimeById(currentId, ((TimeSpan)currentTask.Executiontime + executionTime), true);
                    }
                    else
                    {
                        _taskProvider.updateTaskExecutiontimeById(currentId, executionTime, true);
                    }

                    Response.Cookies.Delete($"TasksInfo{currentId}");
                }
            }
            else
            {
                if (currentTask.Executiontime != null)
                {
                    _taskProvider.updateTaskExecutiontimeById(currentId, (TimeSpan)currentTask.Executiontime, true);
                }
            }

            DatabaseUserProvider userService = new DatabaseUserProvider();

            int hoursSpent = 0;
            if (currentTask.Executiontime != null)
            {
                hoursSpent = ((TimeSpan)currentTask.Executiontime).Hours;
            }

            DatabaseUserProvider userProvider = new DatabaseUserProvider();
            userProvider.addCoinsForAUserById((int)currentTask.Useridref, (10 + hoursSpent));

            return RedirectToAction("Index", "Home");
        }
        
        [Authorize]
        private IEnumerable<EFCore.Task> GetTasksByPeriod(string period, int userId)
        {
            List<EFCore.Task> tasks = new List<EFCore.Task>();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();

            DateTime startDate, endDate;
            switch (period.ToLower())
            {
                case "week":
                    tasks = taskService.getAllTasksByWeekUsingUserId(userId);
                    break;
                case "month":
                    tasks = taskService.getAllTasksByMonthUsingUserId(userId);
                    break;
                case "day":
                    tasks = taskService.getAllTasksByDayUsingUserId(userId);
                    break;
                default:
                    tasks = null;
                    break;
            }

            return tasks;
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return this.View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            Log.Information("Logging out from our website");
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.RedirectToAction("Login", "Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}