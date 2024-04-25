namespace EFCore.Service
{
    public interface IUserProvider
    {
        int getUserID(User user);

        string getUserName(User user);

        string getEmail(User user);

        int getCoinsAmount(User user);

        void setCoinsAmount(User user, int amount);

        User getUserByEmail(string email);

        void addNewUser(string username, string email, string password);

        void addCoinsForAUserById(int id, int coinsAmount);
    }
}
