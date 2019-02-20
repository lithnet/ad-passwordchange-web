using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class HibpPasswordManager : IPasswordManager
    {
        public async Task<PasswordTestResult> TestPartialPassword(string username, string password)
        {
            try
            {
                int pwnCount = await this.GetPwnCount(password);

                if (pwnCount > 0)
                {
                    // Password has been pwned, so return error code
                    return new PasswordTestResult( PasswordTestResultCode.PasswordIsPwned, pwnCount);
                }

                // Otherwise, return successful test result
                return new PasswordTestResult();
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
                int pwnCount = await this.GetPwnCount(password);

                if (pwnCount > 0)
                {
                    // Password has been pwned, so return error code
                    return new PasswordTestResult(PasswordTestResultCode.PasswordIsPwned, pwnCount);
                }

                // Otherwise, return successful test result
                return new PasswordTestResult();
            }
            catch (Exception)
            {
                return new PasswordTestResult(PasswordTestResultCode.GeneralError);

                // Todo: Log the exception
            }
        }

        private async Task<int> GetPwnCount(string password)
        {
            // Hash the supplied password using SHA1
            SHA1 hasher = SHA1.Create();
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