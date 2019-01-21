using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using lithnet.activedirectory.passwordchange.web.Models;
using System.Net;

namespace lithnet.activedirectory.passwordchange.web.Controllers
{
    public class ChangeController : Controller
    {
        private IPasswordManager passwordManager;

        public ChangeController(IPasswordManager passwordManager)
        {
            this.passwordManager = passwordManager;
        }

        public ActionResult Do(string username, string redirect)
        {
            PasswordChangeRequestModel pageModel = new PasswordChangeRequestModel();
           
            if (!String.IsNullOrEmpty(username))
            {
                pageModel.UserName = Server.HtmlEncode(username);
            }

            if (!String.IsNullOrEmpty(redirect))
            {
                pageModel.Redirect = Server.HtmlEncode(redirect);
            }

            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            return this.View(pageModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Do(PasswordChangeRequestModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                model.FailureReason = Resources.UIMessages.PasswordsDoNotMatch;
                return this.View(model);
            }

            try
            {
                if (ConfigurationManager.AppSettings["HIBPEnabled"] == "true")
                {
                    try
                    {
                        int pwnCount = CheckPasswordHIBP(model.NewPassword);

                        if (pwnCount > 0)
                        {
                            model.SecurityAlert = String.Format(Resources.UIMessages.PasswordPwned, pwnCount);
                            return this.View(model);
                        }

                        // Todo: Log that password is good
                    }
                    catch (Exception)
                    {
                        if (ConfigurationManager.AppSettings["HIBPAllowPasswordChangeOnError"] != "true")
                        {
                            model.FailureReason = Resources.UIMessages.HaveIBeenPwnedGeneralError;
                            return this.View(model);
                        }

                        // Todo: Log the exception
                    }

                }
                
                ChangePassword(model.UserName, model.CurrentPassword, model.NewPassword);
            }
            catch (NotFoundException)
            {
                model.FailureReason = Resources.UIMessages.InvalidUserOrPassword;
                return this.View(model);
            }
            catch (PasswordIncorrectException)
            {
                model.FailureReason = Resources.UIMessages.InvalidUserOrPassword;
                return this.View(model);
            }
            catch (PasswordDoesNotMeetPolicyException)
            {
                model.FailureReason = Resources.UIMessages.PasswordDoesNotMeetPolicy;
                return this.View(model);
            }
            catch (Exception ex)
            {
                model.FailureReason = Resources.UIMessages.UnhandledError;
                return this.View(model);
            }

            return this.View();

        }

        public ActionResult Redirect(PasswordChangeRequestModel model)
        {
            if (!String.IsNullOrEmpty(model.Redirect))
            {

                // If this doesn't look like a relative url, try to convert to http scheme
                if (!model.Redirect.StartsWith("/") && !model.Redirect.Contains("://"))
                {
                    model.Redirect = "http://" + model.Redirect;
                }

                return new RedirectResult(model.Redirect);
            }
            
            return RedirectToAction("New");
        }

        private int CheckPasswordHIBP(string newPassword)
        {
            // Hash the supplied password using SHA1
            var hasher = SHA1.Create();
            byte[] hashedPasswordBytes = hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(newPassword));
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
                if (result.Length != 2) {
                    continue;
                }
                if (result[0] == hashedPasswordRemainder)
                {
                    return int.Parse(result[1]);
                }
            }

            return 0;
        }

        public static void ChangePassword(string username, string oldPassword, string newPassword)
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

        private static string GetSamAccountNameFromSamAccountNameOrEmail(string username)
        {
            return username;
        }
    }
}