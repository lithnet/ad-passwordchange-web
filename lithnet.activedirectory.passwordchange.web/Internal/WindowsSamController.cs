using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Lithnet.ActiveDirectory.PasswordChange.Web.Exceptions;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
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
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                // Try to retrieve the user principal data from the security account manager
                UserPrincipal userItem = null;

                if (userOrEmail.IndexOf('@') >= 0)
                {
                    return await WindowsSamController.GetUserPrincipalByUpn(userOrEmail, context) ??
                        await GetUserPrincipalByEmail(userOrEmail, context) ??
                        throw new NotFoundException();
                }
                else
                {
                    userItem = await Task.Run(() => UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userOrEmail));
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
                foreach (UserPrincipal result in searcher.FindAll().OfType<UserPrincipal>())
                {
                    if (foundUser != null)
                    {
                        throw new MultipleMatchesException();
                    }

                    foundUser = result;
                }

                return foundUser;
            });
        }

        private static async Task<UserPrincipal> GetUserPrincipalByUpn(string upn, PrincipalContext context)
        {
            return await Task.Run(() => UserPrincipal.FindByIdentity(context, IdentityType.UserPrincipalName, upn));
        }

        public static async Task ChangeUserPassword(UserPrincipal user, string oldPassword, string newPassword)
        {
            try
            {
                await Task.Run(() => user.ChangePassword(oldPassword, newPassword));
            }
            catch (PasswordException ex)
            {
                UnwrapAndRethrowPasswordException(ex);
                throw;
            }
            catch (PrincipalOperationException ex)
            {
                UnwrapAndRethrowPasswordException(ex);
                throw;
            }
        }

        private static void UnwrapAndRethrowPasswordException(Exception ex)
        {
            if (ex.InnerException is COMException inner)
            {
                if (inner.ErrorCode == (unchecked((int)0x80070056)))
                {
                    throw new PasswordIncorrectException(ex.Message);
                }
                else if (inner.ErrorCode == (unchecked((int)0x800708C5)))
                {
                    throw new PasswordDoesNotMeetPolicyException(ex.Message);
                }
                else if (inner.ErrorCode == (unchecked((int)0x8007052D)))
                {
                    throw new PasswordDoesNotMeetPolicyException(ex.Message);
                }
            }
        }
    }
}
