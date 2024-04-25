namespace EFCore.Service
{
    public interface ITaskProvider
    {
        List<Task> GetAllTasks();

        List<Task> getAllTasksByDayUsingUserId(int userId);

        List<Task> getAllTasksByWeekUsingUserId(int userId);

        List<Task> getAllTasksByMonthUsingUserId(int userId);

        List<Task> GetAllTasksByUserID(int userID);

        int GetAmountOfTasksByDate(DateOnly date, int userID);

        int GetAmountOfCompletedTasksByDate(DateOnly date, int userID);

        int getTaskId(Task task);

        string getName(Task task);

        string getDescription(Task task);

        DateOnly getExpectedFinishTime(Task task);

        DateOnly getFinishTime(Task task);

        int getPriority(Task task);

        bool getCompleted(Task task);

        TimeSpan getExecutionTime(Task task);

        EFCore.Task getTaskById(int id);

        void addNewTask(string name, string description, DateOnly expectedDate, int priority, int userRef);

        void updateTaskById(int id, string newName, string newDesc, string newDate, int newpriority);

        void updateTaskExecutiontimeById(int id, TimeSpan time, bool finished);
    }
}
