using System.Threading.Tasks;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public interface IPasswordManager
    {
        Task<PasswordTestResult> TestPassword(string username, string password);

        Task<PasswordTestResult> TestPartialPassword(string username, string password);
    }
}
