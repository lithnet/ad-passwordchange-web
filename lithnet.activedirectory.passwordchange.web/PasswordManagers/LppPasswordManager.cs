using System;
using System.Threading.Tasks;
using Lithnet.ActiveDirectory.PasswordProtection;
using LppResult = Lithnet.ActiveDirectory.PasswordProtection.PasswordTestResult;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class LppPasswordManager : IPasswordManager
    {
        public async Task<PasswordTestResult> TestPartialPassword(string username, string password)
        {
            try
            {
                PasswordTestResultCode result = (PasswordTestResultCode)(int)FilterInterface.TestPassword(username, username, password, false);
                return new PasswordTestResult(result);
            }
            catch (Exception)
            {
                return new PasswordTestResult(PasswordTestResultCode.GeneralError);

                // Todo: Log the exception
            }
        }

        public async Task<PasswordTestResult> TestPassword(string username, string password)
        {
            try
            {
                PasswordTestResultCode result = (PasswordTestResultCode)(int)FilterInterface.TestPassword(username, username, password, false);
                return new PasswordTestResult(result);

                // Todo: Log that password is good
            }
            catch (Exception)
            {
                return new PasswordTestResult(PasswordTestResultCode.GeneralError);

                // Todo: Log the exception
            }
        }
    }
}