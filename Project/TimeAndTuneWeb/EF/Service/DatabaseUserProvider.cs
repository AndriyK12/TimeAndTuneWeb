namespace EFCore.Service
{
    public class DatabaseUserProvider : IUserProvider
    {
        public void addNewUser(string username, string email, string password)
        {
            User newUser = new User();
            using (var context = new TTContext())
            {
                newUser.Username = username;
                newUser.Email = email;
                newUser.Password = password;
                context.Users.Add(newUser);
                context.SaveChanges();
            }
        }

        public User getUserByEmail(string email)
        {
            User user = new User();
            using (var context = new TTContext())
            {
                var allUsers = context.Users.ToList();
                foreach (User u in allUsers)
                {
                    if (this.getEmail(u) == email)
                    {
                        user = u;
                    }
                }
            }

            return user;
        }

        //public bool isUserAlreadyExist(string email)
        //{
        //    using (var context = new TTContext())
        //    {
        //        var user = context.Users.FirstOrDefault(u => u.Email == email);
        //        return user != null;
        //    }
        //}


        public int getCoinsAmount(User user)
        {
            return (int)user.Coinsamount;
        }

        public string getEmail(User user)
        {
            return user.Email;
        }

        public int getUserID(User user)
        {
            return user.Userid;
        }

        public string getUserName(User user)
        {
            return user.Username;
        }

        public void setCoinsAmount(User user, int amount)
        {
            user.Coinsamount = amount;
        }

        public bool isUserAlreadyExist(string email)
        {
            User user = this.getUserByEmail(email);
            if (user.Email != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void addCoinsForAUserById(int id, int coinsAmount)
        {
            User user = new User();
            using (var context = new TTContext())
            {
                var allUsers = context.Users.ToList();
                foreach (User u in allUsers)
                {
                    if (this.getUserID(u) == id)
                    {
                        user = u;
                    }
                }

                user.Coinsamount += coinsAmount;
                context.SaveChanges();
            }
        }

        public void changePassword(int id, string newPassword)
        {
            User user = new User();
            using (var context = new TTContext())
            {
                var allUsers = context.Users.ToList();
                foreach (User u in allUsers)
                {
                    if (this.getUserID(u) == id)
                    {
                        user = u;
                    }
                }

                user.Password = newPassword;
                context.SaveChanges();
            }
        }
    }
}
