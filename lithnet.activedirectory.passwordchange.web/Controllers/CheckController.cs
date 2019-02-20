using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lithnet.ActiveDirectory.PasswordChange.Web.Controllers
{
    public class CheckController : Controller
    {
        private readonly IPasswordManager passwordManager;

        public CheckController(IPasswordManager passwordManager)
        {
            this.passwordManager = passwordManager;
        }

        [HttpPost]
        public async Task<ActionResult> Password(string username, string newPassword)
        {
            PasswordTestResult testResult = await this.passwordManager.TestPartialPassword(username, newPassword);

            if (testResult != null)
            {
                return this.Json(new { valid = testResult.Code == PasswordTestResultCode.Approved, details = testResult.ToString() });
            }
            else
            {
                return null;
            }
        }
    }
}