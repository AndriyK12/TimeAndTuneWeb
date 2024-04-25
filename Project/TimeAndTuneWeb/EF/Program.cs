namespace EFCore
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            using (var context = new TTContext())
            {
                var allUsers = context.Users.ToList();
                var allTasks = context.Tasks.ToList();

                var firstTaskUsername = allTasks[0].UseridrefNavigation?.Username;
                var secondTaskUsername = allTasks[1].UseridrefNavigation?.Username;
                Console.WriteLine("Username of the user with the first task: " + firstTaskUsername);
                Console.WriteLine("Username of the user with the second task: " + secondTaskUsername);

                foreach (var user in allUsers)
                {
                    Console.WriteLine("===================================================================================");
                    Console.WriteLine($" User ID: {user.Userid},\n Username: {user.Username},\n Email {user.Email},\n Coins amout: {user.Coinsamount},\n Password: {user.Password}");
                    Console.WriteLine("===================================================================================");
                }

                foreach (var task in allTasks)
                {
                    Console.WriteLine("===================================================================================");
                    Console.WriteLine($" Task ID: {task.Taskid},\n UserRef: {task.Useridref},\n Name {task.Name},\n Description: {task.Description},\n Priority: {task.Priority},\n Completed status: {task.Completed},\n Date of creation: {task.Dateofcreation},\n Expected finish time{task.Expectedfinishtime},\n Finish time {task.Finishtime},\n Execution time: {task.Executiontime}");
                    Console.WriteLine("===================================================================================");
                }
            }
        }
    }
}