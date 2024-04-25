using EFCore.Service;
using Newtonsoft.Json;

namespace TestProject
{
    public class DatabaseTaskProviderTests
    {
        [Fact]
        public void getTaskById()
        {
            var properTask = new EFCore.Task();
            properTask.Taskid = 1;
            properTask.Name = "Complete Project A";
            properTask.Description = "Finish all tasks related to Project A";
            properTask.Dateofcreation = new DateOnly(2024, 4, 11);
            properTask.Expectedfinishtime = new DateOnly(2024, 4, 30);
            properTask.Finishtime = new DateOnly(0001, 01, 01);
            properTask.Priority = 1;
            properTask.Completed = false;
            properTask.Executiontime = new TimeSpan(0, 0, 0, 1);
            properTask.Useridref = 1;
            var task = new EFCore.Task();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            task = taskService.getTaskById(1);
            var obj1Str = JsonConvert.SerializeObject(properTask);
            var obj2Str = JsonConvert.SerializeObject(task);
            Assert.Equal(obj1Str, obj2Str);
        }

        [Fact]
        public void getTaskId()
        {
            var task = new EFCore.Task();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            task = taskService.getTaskById(1);
            int foundID = taskService.getTaskId(task);
            Assert.Equal(task.Taskid, foundID);
        }

        [Fact]
        public void getCompleted()
        {
            var task = new EFCore.Task();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            task = taskService.getTaskById(1);
            bool foundCompleted = taskService.getCompleted(task);
            Assert.Equal(task.Completed, foundCompleted);
        }

        [Fact]
        public void getName()
        {
            var task = new EFCore.Task();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            task = taskService.getTaskById(1);
            string foundName = taskService.getName(task);
            Assert.Equal(task.Name, foundName);
        }

        [Fact]
        public void getDescription()
        {
            var task = new EFCore.Task();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            task = taskService.getTaskById(1);
            string foundDescription = taskService.getDescription(task);
            Assert.Equal(task.Description, foundDescription);
        }

        [Fact]
        public void getExpectedFinishTime()
        {
            var task = new EFCore.Task();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            task = taskService.getTaskById(1);
            DateOnly foundExpectedfinishtime = taskService.getExpectedFinishTime(task);
            Assert.Equal(task.Expectedfinishtime, foundExpectedfinishtime);
        }

        [Fact]
        public void getFinishTime()
        {
            var task = new EFCore.Task();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            task = taskService.getTaskById(1);
            DateOnly foundFinishtime = taskService.getFinishTime(task);
            Assert.Equal(task.Finishtime, foundFinishtime);
        }

        [Fact]
        public void getPriority()
        {
            var task = new EFCore.Task();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            task = taskService.getTaskById(1);
            int foundPriority = taskService.getPriority(task);
            Assert.Equal(task.Priority, foundPriority);
        }

        [Fact]
        public void getExecutionTime()
        {
            var task = new EFCore.Task();
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            task = taskService.getTaskById(1);
            TimeSpan foundExecutiontime = taskService.getExecutionTime(task);
            Assert.Equal(task.Executiontime, foundExecutiontime);
        }

        [Fact]
        public void GetAmountOfTasksByDate()
        {
            int actuall_amount = 1;
            int foundAmount = 0;
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            foundAmount = taskService.GetAmountOfTasksByDate(new DateOnly(2024, 04, 30), 1);
            Assert.Equal(actuall_amount, foundAmount);
        }

        [Fact]
        public void GetAmountOfCompletedTasksByDate()
        {
            int actuall_amount = 1;
            int foundAmount = 0;
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            foundAmount = taskService.GetAmountOfCompletedTasksByDate(new DateOnly(2024, 04, 12), 1);
            Assert.Equal(actuall_amount, foundAmount);
        }

        [Fact]
        public void updateTaskById()
        {
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            EFCore.Task properTask = taskService.getTaskById(2);

            string fakerName = Faker.Lorem.Sentence();
            string fakerDesc = Faker.Lorem.Paragraph();
            DateOnly fakerExpectedFinishTime = DateOnly.FromDateTime(Faker.DateTimeFaker.DateTime());
            int fakerPrior = Faker.RandomNumber.Next(1, 3);

            properTask.Name = fakerName;
            properTask.Description = fakerDesc;
            properTask.Expectedfinishtime = fakerExpectedFinishTime;
            properTask.Priority = fakerPrior;

            taskService.updateTaskById(2, fakerName, fakerDesc, fakerExpectedFinishTime.ToString(), fakerPrior - 1);
            EFCore.Task task = taskService.getTaskById(2);

            var obj1Str = JsonConvert.SerializeObject(properTask);
            var obj2Str = JsonConvert.SerializeObject(task);
            Assert.Equal(obj1Str, obj2Str);
        }

        [Fact]
        public void updateTaskExecutiontimeById()
        {
            DatabaseTaskProvider taskService = new DatabaseTaskProvider();
            EFCore.Task properTask = taskService.getTaskById(2);

            Random random = new Random();

            int hours = random.Next(24);
            int minutes = random.Next(60);
            int seconds = random.Next(60);
            int milliseconds = random.Next(1000);

            TimeSpan randomTimeSpan = new TimeSpan(hours, minutes, seconds, milliseconds);
            bool fakerCompleted = Faker.Boolean.Random();

            properTask.Executiontime = randomTimeSpan;
            properTask.Completed = fakerCompleted;

            taskService.updateTaskExecutiontimeById(2, randomTimeSpan, fakerCompleted);
            EFCore.Task task = taskService.getTaskById(2);

            var obj1Str = JsonConvert.SerializeObject(properTask);
            var obj2Str = JsonConvert.SerializeObject(task);
            Assert.Equal(obj1Str, obj2Str);
        }
    }
    public class DatabaseUserProviderTests
    {
        [Fact]
        public void getUserByEmail()
        {
            var properUser = new EFCore.User();
            properUser.Userid = 1;
            properUser.Username = "JohnDoe";
            properUser.Email = "johndoe@example.com";
            properUser.Coinsamount = 100;
            properUser.Password = "secretPassword";
            var user = new EFCore.User();
            DatabaseUserProvider userService = new DatabaseUserProvider();
            user = userService.getUserByEmail("johndoe@example.com");
            var obj1Str = JsonConvert.SerializeObject(properUser);
            var obj2Str = JsonConvert.SerializeObject(user);
            Assert.Equal(obj1Str, obj2Str);
        }

        [Fact]
        public void getCoinsAmount()
        {
            DatabaseUserProvider userService = new DatabaseUserProvider();
            var user = userService.getUserByEmail("johndoe@example.com");
            int userCoinsAmount = userService.getCoinsAmount(user);
            Assert.Equal(user.Coinsamount, userCoinsAmount);
        }

        [Fact]
        public void getEmail()
        {
            DatabaseUserProvider userService = new DatabaseUserProvider();
            var user = userService.getUserByEmail("johndoe@example.com");
            string userEmail = userService.getEmail(user);
            Assert.Equal(user.Email, userEmail);
        }

        [Fact]
        public void getUserID()
        {
            DatabaseUserProvider userService = new DatabaseUserProvider();
            var user = userService.getUserByEmail("johndoe@example.com");
            int userID = userService.getUserID(user);
            Assert.Equal(user.Userid, userID);
        }

        [Fact]
        public void getUserName()
        {
            DatabaseUserProvider userService = new DatabaseUserProvider();
            var user = userService.getUserByEmail("johndoe@example.com");
            string userUsername = userService.getUserName(user);
            Assert.Equal(user.Username, userUsername);
        }

        [Fact]
        public void setCoinsAmount()
        {
            DatabaseUserProvider userService = new DatabaseUserProvider();
            var user = userService.getUserByEmail("margaretbearson@gmail.com");
            int newCoinsAmount = Faker.RandomNumber.Next(100, 1000);
            userService.setCoinsAmount(user, newCoinsAmount);
            Assert.Equal(user.Coinsamount, newCoinsAmount);
        }

        [Fact]
        public void isUserAlreadyExist()
        {
            DatabaseUserProvider userService = new DatabaseUserProvider();
            bool userAlreadyExists = userService.isUserAlreadyExist("margaretbearson@gmail.com");
            Assert.Equal(userAlreadyExists, true);
        }

        [Fact]
        public void addCoinsForAUserById()
        {
            DatabaseUserProvider userService = new DatabaseUserProvider();
            var user = userService.getUserByEmail("margaretbearson@gmail.com");
            int userCoinsAmount = userService.getCoinsAmount(user);
            int additionalCoins = Faker.RandomNumber.Next(10, 50);
            userService.addCoinsForAUserById(userService.getUserID(user), additionalCoins);
            user = userService.getUserByEmail("margaretbearson@gmail.com");
            Assert.Equal(user.Coinsamount, userCoinsAmount + additionalCoins);
        }
    }
}