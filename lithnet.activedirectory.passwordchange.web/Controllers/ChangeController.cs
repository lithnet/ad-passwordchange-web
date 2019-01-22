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
using lithnet.activedirectory.passwordchange.web.Exceptions;

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
                PasswordTestResult result = this.passwordManager.TestPassword(model.UserName, model.NewPassword);

                if (!result.Passed())
                {
                    model.FailureReason = result.GetErrorMessage();
                    return this.View(model);
                    
                }

                model.SecurityAlert = result.GetWarningMessage();

                this.passwordManager.ChangePassword(model.UserName, model.CurrentPassword, model.NewPassword);

                return this.View(model);
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
            
            return RedirectToAction("Do");
        }

    }
}