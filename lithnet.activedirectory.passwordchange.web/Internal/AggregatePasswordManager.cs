using System.Threading.Tasks;
using System.Collections.Generic;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class AggregatePasswordManager : IPasswordManager
    {
        private readonly IEnumerable<IPasswordManager> passwordManagers;

        public AggregatePasswordManager(IEnumerable<IPasswordManager> passwordManagers)
        {
            this.passwordManagers = passwordManagers;
        }

        public async Task<PasswordTestResult> TestPartialPassword(string username, string password)
        {
            foreach (IPasswordManager p in this.passwordManagers)
            {
                PasswordTestResult result = await p.TestPartialPassword(username, password);

                if (result.Code != PasswordTestResultCode.Approved)
                {
                    return result;
                }
            }

            return new PasswordTestResult();
        }

        public async Task<PasswordTestResult> TestPassword(string username, string password)
        {
            foreach (IPasswordManager p in this.passwordManagers)
            {
                PasswordTestResult result = await p.TestPassword(username, password);

                if (result.Code != PasswordTestResultCode.Approved)
                {
                    return result;
                }
            }

            return new PasswordTestResult();
        }
    }
}