namespace Lithnet.ActiveDirectory.PasswordChange.Web
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
            this.Code = PasswordTestResultCode.Approved;
            this.Arguments = null;
        }

        /// <summary>
        /// Create a new test result for a given result code and optional descriptive message
        /// </summary>
        /// <param name="code">Password test error code</param>
        public PasswordTestResult(PasswordTestResultCode code, params object[] args)
        {
            this.Code = code;
            this.Arguments = args;
        }

        public override string ToString()
        {
            return this.ExpandMessage(Resources.UIMessages.ResourceManager.GetString(this.Code.ToString()), this.Arguments);
        }

        private string ExpandMessage(string message, object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return message;
            }
            else
            {
                return string.Format(message, args);
            }
        }
    }
}