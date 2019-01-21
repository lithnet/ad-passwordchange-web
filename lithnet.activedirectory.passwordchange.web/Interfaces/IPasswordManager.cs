using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lithnet.activedirectory.passwordchange.web
{
    public interface IPasswordManager
    {
        void ChangePassword(string username, string oldPassword, string newPassword);

        PasswordTestResult TestPassword(string username, string password);

        PasswordTestResult TestPartialPassword(string username, string password);
    }
}
