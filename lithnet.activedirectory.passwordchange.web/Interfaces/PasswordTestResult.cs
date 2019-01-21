using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lithnet.activedirectory.passwordchange.web
{
    public class PasswordTestResult
    {
        public PasswordTestResultCode Code { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Empty constructor without any error codes implies successful test.
        /// </summary>
        public PasswordTestResult()
        {
            Code = PasswordTestResultCode.Approved;
        }

        /// <summary>
        /// Create a new test result for a given rejection code and optional error message
        /// </summary>
        /// <param name="code">Password test error code</param>
        /// <param name="errorMessage">Formatted error message</param>
        public PasswordTestResult(PasswordTestResultCode code, string errorMessage)
        {
            Code = code;
            ErrorMessage = errorMessage;
        }
    }
}