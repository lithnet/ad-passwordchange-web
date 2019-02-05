using lithnet.activedirectory.passwordchange.web.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace lithnet.activedirectory.passwordchange.web
{
    public class HIBPPasswordManager : IPasswordManager
    {

        public async Task<PasswordTestResult> TestPartialPassword(string username, string password)
        {
            try
            {
                int pwnCount = await getPwnCount(password);

                if (pwnCount > 0)
                {
                    // Password has been pwned, so return error code
                    return new PasswordTestResult(
                        PasswordTestResultCode.PasswordIsPwned,
                        pwnCount
                        );
                }

                // Otherwise, return successful test result
                return new PasswordTestResult();

                // Todo: Log that password is good
            }
            catch (Exception)
            {
                return new PasswordTestResult(
                    PasswordTestResultCode.GeneralError
                    );

                // Todo: Log the exception
            }
        }

        public async Task<PasswordTestResult> TestPassword(string username, string password)
        {
            try
            {
                int pwnCount = await getPwnCount(password);

                if (pwnCount > 0)
                {
                    // Password has been pwned, so return error code
                    return new PasswordTestResult(
                        PasswordTestResultCode.PasswordIsPwned,
                        pwnCount
                        );
                }

                // Otherwise, return successful test result
                return new PasswordTestResult();

                // Todo: Log that password is good
            }
            catch (Exception)
            {
                return new PasswordTestResult(
                    PasswordTestResultCode.GeneralError
                    );

                // Todo: Log the exception
            }

        }

        private async Task<int> getPwnCount(string password)
        {
            // Hash the supplied password using SHA1
            var hasher = SHA1.Create();
            byte[] hashedPasswordBytes = hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            string hashedPasswordString = hashedPasswordBytes.ToHexString();
            string hashedPasswordPrefix = hashedPasswordString.Substring(0, 5);
            string hashedPasswordRemainder = hashedPasswordString.Substring(5);

            // Send the first five characters of the hash to the HaveIBeenPwned API
            string apiUrl = $"https://api.pwnedpasswords.com/range/{hashedPasswordPrefix}";

            HttpClient client = new HttpClient();

            string response = await client.GetStringAsync(new Uri(apiUrl));

            foreach (string line in response.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                string[] result = line.Split(':');
                if (result.Length != 2)
                {
                    continue;
                }
                if (result[0] == hashedPasswordRemainder)
                {
                    return int.Parse(result[1]);
                }
            }

            return 0;
        }

    }

}