using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Lithnet.ActiveDirectory.PasswordProtection;
using LppResult = Lithnet.ActiveDirectory.PasswordProtection.PasswordTestResult;
using System.Collections.Generic;

namespace lithnet.activedirectory.passwordchange.web
{
    public class AggregatePasswordManager : IPasswordManager
    {
        private IEnumerable<IPasswordManager> passwordManagers;

        public AggregatePasswordManager(IEnumerable<IPasswordManager> passwordManagers)
        {
            this.passwordManagers = passwordManagers;
        }

        public async Task<PasswordTestResult> TestPartialPassword(string username, string password)
        {
            foreach (IPasswordManager p in passwordManagers)
            {
                var result = await p.TestPartialPassword(username, password);
                if (result.Code != PasswordTestResultCode.Approved)
                {
                    return result;
                }
            }
            return new PasswordTestResult();
        }

        public async Task<PasswordTestResult> TestPassword(string username, string password)
        {
            foreach (IPasswordManager p in passwordManagers)
            {
                var result = await p.TestPassword(username, password);
                if (result.Code != PasswordTestResultCode.Approved)
                {
                    return result;
                }
            }
            return new PasswordTestResult();

        }

    }

}