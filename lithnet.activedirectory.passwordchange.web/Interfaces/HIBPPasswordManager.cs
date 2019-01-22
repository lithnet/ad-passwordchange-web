using lithnet.activedirectory.passwordchange.web.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Web;

namespace lithnet.activedirectory.passwordchange.web
{
    public class HIBPPasswordManager : IPasswordManager
    {
        public void ChangePassword(string username, string oldPassword, string newPassword)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
            {
                username = GetSamAccountNameFromSamAccountNameOrEmail(username);

                using (UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username))
                {
                    if (user == null)
                    {
                        throw new NotFoundException();
                    }

                    try
                    {
                        user.ChangePassword(oldPassword, newPassword);
                    }
                    catch (System.DirectoryServices.AccountManagement.PasswordException ex)
                    {
                        COMException inner = ex.InnerException as COMException;
                        if (inner != null)
                        {
                            if (inner.ErrorCode == (unchecked((int)0x80070056)))
                            {
                                throw new PasswordIncorrectException(ex.Message);
                            }
                            else if (inner.ErrorCode == (unchecked((int)0x800708C5)))
                            {
                                throw new PasswordDoesNotMeetPolicyException(ex.Message);
                            }
                        }

                        throw;
                    }
                }
            }
        }

        public PasswordTestResult TestPartialPassword(string username, string password)
        {
            throw new NotImplementedException();
        }

        public PasswordTestResult TestPassword(string username, string password)
        {
            try
            {
                int pwnCount = getPwnCount(password);

                if (pwnCount > 0)
                {
                    // Password has been pwned, so return error code
                    return new PasswordTestResult(
                        PasswordTestResultCode.PasswordIsPwned,
                        String.Format(Resources.UIMessages.PasswordPwned, pwnCount)
                        );
                }

                // Otherwise, return successful test result
                return new PasswordTestResult();

                // Todo: Log that password is good
            }
            catch (Exception)
            {
                return new PasswordTestResult(
                    PasswordTestResultCode.PwnedGeneralError,
                    Resources.UIMessages.HaveIBeenPwnedGeneralError
                    );

                // Todo: Log the exception
            }

        }

        private int getPwnCount(string password)
        {
            // Hash the supplied password using SHA1
            var hasher = SHA1.Create();
            byte[] hashedPasswordBytes = hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            string hashedPasswordString = hashedPasswordBytes.ToHexString();
            string hashedPasswordPrefix = hashedPasswordString.Substring(0, 5);
            string hashedPasswordRemainder = hashedPasswordString.Substring(5);

            // Send the first five characters of the hash to the HaveIBeenPwned API
            string apiUrl = $"https://api.pwnedpasswords.com/range/{hashedPasswordPrefix}";

            WebClient client = new WebClient();
            string response = client.DownloadString(apiUrl);

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

        private static string GetSamAccountNameFromSamAccountNameOrEmail(string username)
        {
            return username;
        }

    }

}