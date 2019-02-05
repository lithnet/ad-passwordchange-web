using lithnet.activedirectory.passwordchange.web.Exceptions;
using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace lithnet.activedirectory.passwordchange.web
{
    public static class WindowsSamController
    {

        /// <summary>
        /// Connect to the Security Account Manager service and retrieve the user principal
        /// structure for a given username or user email address.
        /// </summary>
        /// <param name="userOrEmail">User login name or account email address</param>
        /// <returns>User Principal data structure if found</returns>
        public static async Task<UserPrincipal> GetUserPrincipal(string userOrEmail)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
            {
                // Try to retrieve the user principal data from the security account manager
                UserPrincipal userItem = null;

                // If user has provided an email address instead of username, use this.
                // Note the check is a simple regular expression to confirm the format is <text>@<text>.<2-4 chars>
                if (userOrEmail.IndexOf('@') >= 0)
                {
                    return await GetUserPrincipalByUPN(userOrEmail, context) ?? 
                        await GetUserPrincipalByEmail(userOrEmail, context) ??
                        throw new NotFoundException();
                }
                else
                {
                    userItem = await Task.Run(() =>
                    {
                        return UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userOrEmail);
                    });
                }
                
                // If we couldn't find a matching account, throw an error to the calling routine
                if (userItem == null)
                {
                    throw new NotFoundException();
                }

                // Otherwise, return the principal structure located
                return userItem;
            }
        }

        private static async Task<UserPrincipal> GetUserPrincipalByEmail(string email, PrincipalContext context)
        {
            return await Task.Run(() =>
            {
                UserPrincipal searchUser = new UserPrincipal(context);
                searchUser.EmailAddress = email;
                PrincipalSearcher searcher = new PrincipalSearcher(searchUser);

                UserPrincipal foundUser = null;
                foreach (var result in searcher.FindAll().OfType<UserPrincipal>())
                {
                    if (foundUser != null)
                    {
                        throw new MultipleMatchesException();
                    }

                    foundUser = result as UserPrincipal;
                }

                return foundUser;
            });
        }

        private static async Task<UserPrincipal> GetUserPrincipalByUPN(string upn, PrincipalContext context)
        {
            // Username is going to be string before the '@' symbol
            return await Task.Run(() =>
            {
                return UserPrincipal.FindByIdentity(context, IdentityType.UserPrincipalName, upn);
            });
        }

        public static async Task ChangeUserPassword(UserPrincipal user, string oldPassword, string newPassword)
        {
            try
            {
                await Task.Run(() =>
                {
                    user.ChangePassword(oldPassword, newPassword);
                });

            }
            catch (PasswordException ex)
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
