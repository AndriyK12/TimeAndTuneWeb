namespace EFCore.Service
{
    public class DatabaseTaskProvider : ITaskProvider
    {
        public void addNewTask(string name, string description, DateOnly expectedDate, int priority,int userRef)
        {
            Task newTask = new Task();
            using (var context = new TTContext())
            {
                newTask.Name = name;
                newTask.Description = description;
                newTask.Expectedfinishtime = expectedDate;
                newTask.Priority = priority;
                newTask.Dateofcreation = DateOnly.FromDateTime(DateTime.Now);
                newTask.Useridref = userRef;
                context.Tasks.Add(newTask);
                context.SaveChanges();
            }
        }

        public List<Task> GetAllTasks()
        {
            using (var context = new TTContext())
            {
                var allTasks = context.Tasks.ToList();
                return allTasks;
            }
        }

        public int GetAmountOfTasksByDate(DateOnly date, int userID)
        {
            using (var context = new TTContext())
            {
                var allTasks = context.Tasks.ToList();
                var dateTasksAmount = 0;
                foreach (var task in allTasks)
                {
                    if (task.Finishtime == date || (task.Expectedfinishtime == date && task.Completed == false))
                    {
                        if (task.Useridref == userID)
                        {
                            dateTasksAmount++;
                        }
                    }
                }
                return dateTasksAmount;
            }
        }

        public int GetAmountOfCompletedTasksByDate(DateOnly date, int userID)
        {
            using (var context = new TTContext())
            {
                var allTasks = context.Tasks.ToList();
                var dateTasksAmount = 0;
                foreach (var task in allTasks)
                {
                    if (task.Finishtime == date && this.getCompleted(task))
                    {
                        if (task.Useridref == userID)
                        {
                            dateTasksAmount++;
                        }
                    }
                }
                return dateTasksAmount;
            }
        }

        public List<Task> GetAllTasksByUserID(int userID)
        {
            using (var context = new TTContext())
            {
                var allTasks = context.Tasks.ToList();
                var userTasks = new List<Task>();
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
                foreach (var task in allTasks)
                {
                    if (task.Useridref == userID)
                    {
                        userTasks.Add(task);
                    }
                }

                return userTasks;
            }
        }

        public bool getCompleted(Task task)
        {
            bool task_completed = (bool)task.Completed;
            return task_completed;
        }

        public int getTaskId(Task task)
        {
            return task.Taskid;
        }

        public string getName(Task task)
        {
            return task.Name;
        }

        public string getDescription(Task task)
        {
            return task.Description;
        }

        public DateOnly getExpectedFinishTime(Task task)
        {
            return task.Expectedfinishtime;
        }

        public DateOnly getFinishTime(Task task)
        {
            return (DateOnly)task.Finishtime;
        }

        public int getPriority(Task task)
        {
            return task.Priority;
        }

        public EFCore.Task? getTaskById(int id)
        {
            foreach (EFCore.Task item in this.GetAllTasks())
            {
                if (item.Taskid == id)
                {
                    return item;
                }
            }
            return null;
        }

        public TimeSpan getExecutionTime(Task task)
        {
            return (TimeSpan)task.Executiontime;
        }

        public void updateTaskById(int id, string newName, string newDesc, string newDate, int newpriority)
        {
            using (var context = new TTContext())
            {
                Task? task = this.getTaskById(id);
                task.Name = newName;
                task.Description = newDesc;
                DateOnly.TryParse(newDate, out DateOnly dateOnly);
                task.Expectedfinishtime = dateOnly;
                task.Priority = newpriority + 1;
                context.Update(task);
                context.SaveChanges();
            }
        }

        public void updateTaskExecutiontimeById(int id, TimeSpan time, bool finished)
        {
            using (var context = new TTContext())
            {
                Task task = this.getTaskById(id);
                task.Executiontime = time;
                task.Completed = finished;
                task.Finishtime = DateOnly.FromDateTime(DateTime.Now);
                context.Update(task);
                context.SaveChanges();
            }
        }

        public List<Task> getAllSpecificTaskByUserId(int userID)
        {
            List<EFCore.Task> items = this.GetAllTasksByUserID(userID);
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

            List<Task> tasks = items
                .Where(task => (task.Expectedfinishtime < currentDate &&
                task.Completed == false) ||
                task.Expectedfinishtime >= currentDate).ToList();

            return tasks;
        }

        public List<Task> getAllTasksByDayUsingUserId(int userID)
        {
            List<EFCore.Task> items = this.GetAllTasksByUserID(userID);
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            List<Task> tasks = items
                .Where(task => (task.Expectedfinishtime < currentDate &&
                task.Completed == false) ||
                (task.Expectedfinishtime == currentDate)).ToList();

            return tasks;
        }

        public List<Task> getAllTasksByWeekUsingUserId(int userID)
        {
            List<EFCore.Task> items = this.GetAllTasksByUserID(userID);
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

            int currentDayOfWeek = (int)currentDate.DayOfWeek;
            int daysToMonday = currentDayOfWeek - (int)DayOfWeek.Monday;
            DateOnly endOfWeekDate = currentDate.AddDays(daysToMonday);

            List<Task> tasks = items
                .Where(task => task.Expectedfinishtime >= currentDate &&
                task.Expectedfinishtime <= endOfWeekDate).ToList();
            return tasks;
        }

        public List<Task> getAllTasksByMonthUsingUserId(int userID)
        {
            List<EFCore.Task> items = this.GetAllTasksByUserID(userID);

            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly endOfMonthDate = new DateOnly(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));

            List<Task> tasks = items
                .Where(task => task.Expectedfinishtime >= currentDate &&
                task.Expectedfinishtime <= endOfMonthDate).ToList();

            return tasks;
        }
    }
}
