using System;
using System.Web.Mvc;
using lithnet.activedirectory.passwordchange.web.Models;
using System.Threading.Tasks;
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
                return this.View(pageModel);
            }

            return this.View(pageModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Do(PasswordChangeRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                model.FailureReason = Resources.UIMessages.PasswordsDoNotMatch;
                return View(model);
            }

            try
            {
                PasswordTestResult result = await this.passwordManager.TestPassword(model.UserName, model.NewPassword).ConfigureAwait(false);

                if (result.Code != PasswordTestResultCode.Approved)
                {
                    model.FailureReason = result.ToString();
                    return View(model);
                }

                await ChangePassword(model.UserName, model.CurrentPassword, model.NewPassword).ConfigureAwait(false);

                model.Success = true;
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
                return RedirectToAction("Do");
            }

            if (model.Redirect != null)
            {
                return new RedirectResult(model.Redirect);
            }

            return this.View();
        }

        public async Task ChangePassword(string username, string oldPassword, string newPassword)
        {
            var userPrincipal = await WindowsSamController.GetUserPrincipal(username);

            await WindowsSamController.ChangeUserPassword(userPrincipal, oldPassword, newPassword);
        }

    }
}