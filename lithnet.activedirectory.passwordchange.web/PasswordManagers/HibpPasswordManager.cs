using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NLog;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class HibpPasswordManager : IPasswordManager
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
                int pwnCount = await this.GetPwnCount(password);

                if (pwnCount > 0)
                {
                    // Password has been pwned, so return error code
                    Logger.Warn($"User {username} attempted to set a password that has been seen in HIBP {pwnCount} times");
                    return new PasswordTestResult(PasswordTestResultCode.PasswordIsPwned, pwnCount);
                }

                // Otherwise, return successful test result
                return new PasswordTestResult();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An unexpected error occurred when trying to check the password against the HIBP service");
                return new PasswordTestResult(PasswordTestResultCode.GeneralError);
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