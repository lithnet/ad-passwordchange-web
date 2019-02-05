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
        public object[] Arguments { get; set; }

        /// <summary>
        /// Empty constructor without any error codes implies successful test.
        /// </summary>
        public PasswordTestResult()
        {
            Code = PasswordTestResultCode.Approved;
            Arguments = null;
        }

        /// <summary>
        /// Create a new test result for a given result code and optional descriptive message
        /// </summary>
        /// <param name="code">Password test error code</param>
        /// <param name="description">Optional string containing formatted description of the result</param>
        public PasswordTestResult(PasswordTestResultCode code, params object[] args)
        {
            Code = code;
            Arguments = args;
        }

        public override string ToString()
        {
            return ExpandMessage(Resources.UIMessages.ResourceManager.GetString(Code.ToString()), Arguments);
        }

        private string ExpandMessage(string message, object[] args)
        {
            if (args == null || args.Length == 0)
                return message;
            else
                return string.Format(message, args);
        }
    }
}