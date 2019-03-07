using System;
using System.Configuration;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using Lithnet.ActiveDirectory.PasswordChange.Web.Exceptions;
using Lithnet.ActiveDirectory.PasswordChange.Web.Models;
using NLog;

namespace Lithnet.ActiveDirectory.PasswordChange.Web.Controllers
{
    public class ChangeController : Controller
    {
        private readonly IPasswordManager passwordManager;

        private readonly RateLimiter rateLimiter;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ChangeController(IPasswordManager passwordManager, RateLimiter rateLimiter)
        {
            this.passwordManager = passwordManager;
            this.rateLimiter = rateLimiter;
        }

        public ActionResult Do(string username, string redirect)
        {
            try
            {
                if (ConfigurationManager.AppSettings["logHeaders"] == "true")
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine("--------------------");
                    message.AppendLine($"Request from: {this.Request.UserHostAddress}");
                    foreach (string key in this.Request.Headers.Keys)
                    {
                        message.AppendLine($"{key}: {this.Request.Headers.Get(key)}");
                    }

                    message.AppendLine("--------------------");
                    Logger.Trace(message.ToString());
                }

                this.rateLimiter.ThrowOnRateLimitExceeded(username, this.Request);

                PasswordChangeRequestModel pageModel = new PasswordChangeRequestModel();

                if (!string.IsNullOrEmpty(username))
                {
                    pageModel.UserName = this.Server.HtmlEncode(username);
                }

                if (!string.IsNullOrEmpty(redirect))
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
            catch (RateLimitExceededException ex)
            {
                Logger.Error(ex);
                return this.View("RateLimitExceeded");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An unhandled exception occurred in Do");
                return this.View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Do(PasswordChangeRequestModel model)
        {
            try
            {
                this.rateLimiter.ThrowOnRateLimitExceeded(model.UserName, this.Request);

                if (!this.ModelState.IsValid)
                {
                    return this.View(model);
                }

                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    model.FailureReason = Resources.UIMessages.PasswordsDoNotMatch;
                    return this.View(model);
                }

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

                return this.View("Success");
            }
            catch (NotFoundException)
            {
                Logger.Error($"The password change attempt for user {model.UserName} failed because the user was not found in the directory");
                model.FailureReason = Resources.UIMessages.InvalidUserOrPassword;
                return this.View(model);
            }
            catch (PasswordIncorrectException)
            {
                Logger.Error($"The password change attempt for user {model.UserName} failed because the current password was incorrect");
                model.FailureReason = Resources.UIMessages.InvalidUserOrPassword;
                return this.View(model);
            }
            catch (PasswordDoesNotMeetPolicyException)
            {
                Logger.Error($"The password change attempt for user {model.UserName} failed because AD rejected the password change");
                model.FailureReason = Resources.UIMessages.PasswordRejectedByAdPolicy;
                return this.View(model);
            }
            catch (RateLimitExceededException ex)
            {
                Logger.Error(ex.Message);
                return this.View("RateLimitExceeded");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An unhandled exception occurred when attempting the password change");
                model.FailureReason = Resources.UIMessages.UnhandledError;
                return this.View(model);
            }
        }

        public ActionResult Success()
        {
            return this.View();
        }

        public ActionResult SuccessEnd()
        {
            return this.View();
        }

        private async Task ChangePassword(string username, string oldPassword, string newPassword)
        {
            UserPrincipal userPrincipal = await WindowsSamController.GetUserPrincipal(username);
            await WindowsSamController.ChangeUserPassword(userPrincipal, oldPassword, newPassword);
        }

        private bool ValidRedirectUri(string uri)
        {
            return PasswordChangeConfigSection.Configuration.AllowedReturnUrls.OfType<AllowedReturnUrlElement>().Any(t => string.Equals(t.Value, uri, StringComparison.OrdinalIgnoreCase));
        }
    }
}