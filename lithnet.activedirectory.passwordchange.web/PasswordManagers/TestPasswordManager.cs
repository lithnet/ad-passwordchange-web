
using System.Threading.Tasks;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class TestPasswordManager : IPasswordManager
    {
        public async Task ChangePassword(string username, string oldPassword, string newPassword)
        {
        }

        public async Task<PasswordTestResult> TestPartialPassword(string username, string password)
        {
            return new PasswordTestResult();
        }

        public async Task<PasswordTestResult> TestPassword(string username, string password)
        {
            return new PasswordTestResult();
        }
    }
}