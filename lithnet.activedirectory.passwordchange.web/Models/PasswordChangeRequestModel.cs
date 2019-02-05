using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace lithnet.activedirectory.passwordchange.web.Models
{
    [Localizable(true)]
    public class PasswordChangeRequestModel
    {
        [Required(ErrorMessageResourceType = typeof(UIMessages), ErrorMessageResourceName = "UserNameIsRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(UIMessages), ErrorMessageResourceName = "CurrentPasswordIsRequired")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(UIMessages), ErrorMessageResourceName = "NewPasswordIsRequired")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(UIMessages), ErrorMessageResourceName = "ConfirmPasswordIsRequired")]
        public string ConfirmNewPassword { get; set; }

        public string FailureReason { get; set; }

        public bool Success { get; set; }

        public string Redirect { get; set; }

    }
}