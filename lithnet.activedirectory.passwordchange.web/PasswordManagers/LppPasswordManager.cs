using System;
using System.Threading.Tasks;
using Lithnet.ActiveDirectory.PasswordProtection;
using NLog;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class LppPasswordManager : IPasswordManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<PasswordTestResult> TestPartialPassword(string username, string password)
        {
            return await this.TestPassword(username, password);
        }

        public async Task<PasswordTestResult> TestPassword(string username, string password)
        {
            try
            {
                PasswordTestResultCode result = (PasswordTestResultCode)(int)FilterInterface.TestPassword(username, username, password, false);
                if (result != PasswordTestResultCode.Approved)
                {
                    Logger.Warn($"User {username} attempted to set a password that was rejected by Lithnet Password Protection with the following code: {result}");
                }

                return new PasswordTestResult(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An unexpected error occurred checking the password against the Lithnet Password Protection service");

                if (PasswordChangeConfigSection.Configuration.PasswordTesting.LppIgnoreErrors)
                {
                    return new PasswordTestResult();
                }
                else
                {
                    return new PasswordTestResult(PasswordTestResultCode.GeneralError);
                }
            }
        }
    }
}