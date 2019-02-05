using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lithnet.activedirectory.passwordchange.web
{
    public interface IPasswordManager
    { 

        Task<PasswordTestResult> TestPassword(string username, string password);

        Task<PasswordTestResult> TestPartialPassword(string username, string password);
    }
}
