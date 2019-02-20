using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text.RegularExpressions;
using Lithnet.ActiveDirectory.PasswordChange.Web.Exceptions;
using Lithnet.ActiveDirectory.PasswordChange.Web.Models;

namespace Lithnet.ActiveDirectory.PasswordChange.Web.Controllers
{
    public class ChangeController : Controller
    {
        private readonly IPasswordManager passwordManager;

        private static List<string> redirectWhitelist;

        public ChangeController(IPasswordManager passwordManager)
        {
            this.passwordManager = passwordManager;
        }

        public ActionResult Do(string username, string redirect)
        {
            PasswordChangeRequestModel pageModel = new PasswordChangeRequestModel();
           
            if (!String.IsNullOrEmpty(username))
            {
                pageModel.UserName = this.Server.HtmlEncode(username);
            }

            if (!String.IsNullOrEmpty(redirect))
            {
                redirect = this.Server.HtmlEncode(redirect);
                if (this.ValidRedirectUri(redirect))
                {
                    pageModel.Redirect = redirect;
                }
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(pageModel);
            }

            return this.View(pageModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Do(PasswordChangeRequestModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                model.FailureReason = Resources.UIMessages.PasswordsDoNotMatch;
                return this.View(model);
            }

            try
            {
                PasswordTestResult result = await this.passwordManager.TestPassword(model.UserName, model.NewPassword).ConfigureAwait(false);

                if (result.Code != PasswordTestResultCode.Approved)
                {
                    model.FailureReason = result.ToString();
                    return this.View(model);
                }

                await this.ChangePassword(model.UserName, model.CurrentPassword, model.NewPassword).ConfigureAwait(false);

                model.Success = true;
                if (model.Redirect != null)
                {
                    if (this.ValidRedirectUri(model.Redirect))
                    {
                        return new RedirectResult(model.Redirect);
                    }
                    else
                    {
                        model.Redirect = null;
                    }
                }
                return this.View("Success", model);
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
        }

        public async Task<ActionResult> Success(PasswordChangeRequestModel model)
        {
            if (!model.Success)
            {
                return this.RedirectToAction("Do");
            }

            if (model.Redirect != null)
            {
                if (this.ValidRedirectUri(model.Redirect))
                {
                    return new RedirectResult(model.Redirect);
                }
                else
                {
                    model.Redirect = null;
                }
            }

            return this.View();
        }

        public async Task ChangePassword(string username, string oldPassword, string newPassword)
        {
            UserPrincipal userPrincipal = await WindowsSamController.GetUserPrincipal(username);
            await WindowsSamController.ChangeUserPassword(userPrincipal, oldPassword, newPassword);
        }

        public bool ValidRedirectUri(string uri)
        {
            // Only load the whitelist once from file
            if (redirectWhitelist == null)
            {
                redirectWhitelist = System.IO.File.ReadAllLines(this.Server.MapPath("~/App_Data/RedirectWhitelist.txt")).ToList();
            }

            if (redirectWhitelist != null)
            {
                foreach (string u in redirectWhitelist)
                {
                    string pattern = Regex.Escape(u).Replace("%%", ".*");
                    Match result = Regex.Match(uri.ToLower(), pattern.ToLower());
                    if (result.Success)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}