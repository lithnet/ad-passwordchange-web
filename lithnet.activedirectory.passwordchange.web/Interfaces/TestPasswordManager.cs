
namespace lithnet.activedirectory.passwordchange.web
{
    public class TestPasswordManager : IPasswordManager
    {
        public void ChangePassword(string username, string oldPassword, string newPassword)
        {
            
        }

        public int TestPartialPassword(string username, string password, out string reason)
        {
            return 0;
        }

        public int TestPassword(string username, string password, out string reason)
        {
            return 0;
        }
    }
}