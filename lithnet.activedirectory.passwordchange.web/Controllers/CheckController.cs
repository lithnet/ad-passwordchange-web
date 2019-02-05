using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace lithnet.activedirectory.passwordchange.web.Controllers
{
    public class CheckController : Controller
    {
        private IPasswordManager passwordManager;

        public CheckController(IPasswordManager passwordManager)
        {
            this.passwordManager = passwordManager;
        }

        [HttpPost]
        public async Task<ActionResult> Password(string username, string newPassword)
        {
            PasswordTestResult testResult = await passwordManager.TestPartialPassword(username, newPassword);

            if (testResult != null)
            {
                return Json(new { valid = testResult.Code == PasswordTestResultCode.Approved, details = testResult.ToString() });
            }
            else
            {
                return null;
            }
        }
    }
}