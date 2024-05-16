namespace TimeAndTuneWeb.Controllers
{
    using System.Security.Claims;
    using EFCore.Service;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Serilog;

    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly IUserProvider _userProvider;

        public StatisticsController(IUserProvider userProvider)
        {
            this._userProvider = userProvider;
        }

        [Authorize]
        public IActionResult Statistics()
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

            // Week chart
            Log.Information("Loading StatisticsPage chart for a week");
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            int currentDayOfWeek = (int)currentDate.DayOfWeek;
            int daysToMonday = currentDayOfWeek - (int)DayOfWeek.Monday;
            DateOnly startOfWeek = currentDate.AddDays(-daysToMonday);
            DateOnly[] dates = new DateOnly[7];
            for (int i = 0; i < 7; i++)
            {
                dates[i] = startOfWeek.AddDays(i);
            }

            var tasks = new List<int>();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            foreach (DateOnly date in dates)
            {
                int amount = taskService.GetAmountOfCompletedTasksByDate(date, userId/*userService.getUserID(MainWindow.ActiveUser)*/);
                tasks.Add(amount);
            }

            var weekLabels = dates.Select(date => date.ToString("dd/MM")).ToArray();
            var weekData = tasks;

            this.ViewBag.WeekDates = weekLabels;
            this.ViewBag.WeekCompletedTasks = weekData;


            // Month chart
            Log.Information("Loading StatisticsPage chart for a month");
            DateOnly firstDayOfMonth = new DateOnly(currentDate.Year, currentDate.Month, 1);
            DateOnly lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            int daysInMonth = (int)lastDayOfMonth.Day - firstDayOfMonth.Day + 1;
            dates = new DateOnly[daysInMonth];
            for (int i = 0; i < daysInMonth; i++)
            {
                dates[i] = firstDayOfMonth.AddDays(i);
            }

            tasks = new List<int>();
            foreach (DateOnly date in dates)
            {
                int amount = taskService.GetAmountOfCompletedTasksByDate(date, userId/*userService.getUserID(MainWindow.ActiveUser)*/);
                tasks.Add(amount);
            }

            var monthLabels = dates.Select(date => date.ToString("dd/MM")).ToArray();
            var monthData = tasks;

            this.ViewBag.MonthDates = monthLabels;
            this.ViewBag.MonthCompletedTasks = monthData;

            // Year chart
            Log.Information("Loading StatisticsPage chart for a year");
            int currentYear = currentDate.Year;
            Dictionary<int, List<DateOnly>> dateDictionary = new Dictionary<int, List<DateOnly>>();
            for (int month = 1; month <= 12; month++)
            {
                firstDayOfMonth = new DateOnly(currentYear, month, 1);
                lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                List<DateOnly> datesOfMonth = new List<DateOnly>();
                for (DateOnly date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
                {
                    datesOfMonth.Add(date);
                }

                dateDictionary.Add(month, datesOfMonth);
            }

            tasks = new List<int>();
            foreach (var kvp in dateDictionary)
            {
                int amount_of_tasks_by_month = 0;
                foreach (var date in kvp.Value)
                {
                    amount_of_tasks_by_month += taskService.GetAmountOfCompletedTasksByDate(date, userId/*userService.getUserID(MainWindow.ActiveUser)*/);
                }

                tasks.Add(amount_of_tasks_by_month);
            }

            var monthNames = new string[]
            {
                    "January", "February", "March", "April", "May", "June",
                    "July", "August", "September", "October", "November", "December",
            };

            var yearLabels = monthNames;
            var yearData = tasks;

            this.ViewBag.YearDates = yearLabels;
            this.ViewBag.YearCompletedTasks = yearData;

            return this.View();
        }
    }
}
