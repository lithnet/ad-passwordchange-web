
namespace lithnet.activedirectory.passwordchange.web
{
    public class TestPasswordManager : IPasswordManager
    {
        public void ChangePassword(string username, string oldPassword, string newPassword)
        {
            
        }

        public PasswordTestResult TestPartialPassword(string username, string password)
        {
            return new PasswordTestResult();
        }

        public PasswordTestResult TestPassword(string username, string password)
        {
            return new PasswordTestResult();
        }
    }
}