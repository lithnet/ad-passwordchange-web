using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace lithnet.activedirectory.passwordchange.web
{
    public class PasswordTestResult
    {
        public PasswordTestResultCode Code { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Empty constructor without any error codes implies successful test.
        /// </summary>
        public PasswordTestResult()
        {
            Code = PasswordTestResultCode.Approved;
        }

        /// <summary>
        /// Create a new test result for a given result code and optional descriptive message
        /// </summary>
        /// <param name="code">Password test error code</param>
        /// <param name="description">Optional string containing formatted description of the result</param>
        public PasswordTestResult(PasswordTestResultCode code, string description = "")
        {
            Code = code;
            Description = description;
        }

        /// <summary>
        /// Determine if the password complexity rules have been passed based on the result of the password check
        /// </summary>
        /// <returns>True if user is allowed this password</returns>
        public bool Passed()
        {
            switch (Code)
            {
                case PasswordTestResultCode.Approved:
                    return true;
                case PasswordTestResultCode.PasswordIsPwned:
                    return ConfigurationManager.AppSettings["HIBPAllowPasswordChangeOnError"] == "true";
                default:
                    return false;
            }
        }

        /// <summary>
        /// Retrieve any security warning message to be displayed as a result of a successful test
        /// </summary>
        /// <returns>Warning message for user</returns>
        public string GetWarningMessage()
        {
            if (Passed() && !String.IsNullOrEmpty(Description))
            {
                return Description;
            }

            return null;
        }

        /// <summary>
        /// Retrieve any rejection message to be displayed as a result of any failed test
        /// </summary>
        /// <returns>Rejection message for user</returns>
        public string GetErrorMessage()
        {
            if (!Passed() && !String.IsNullOrEmpty(Description))
            {
                return Description;
            }

            return null;
        }
    }
}