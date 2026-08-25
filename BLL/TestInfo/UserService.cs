namespace BLL.TestInfo
{
    public class UserService
    {
        public void registerUser(string userName , string passWord)
        {
            userValidator(userName);
            HashCode(passWord);
            RegUser(userName , passWord);
            sayWelComeToUser(userName);
        }

        public void userValidator(string username)
        {

        }

        public void HashCode(string password)
        {

        }

        public void RegUser(string username , string password)
        {

        }

        public void sayWelComeToUser(string username)
        {

        }
    }
}
