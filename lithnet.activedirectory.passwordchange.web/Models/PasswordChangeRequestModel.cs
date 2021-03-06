﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Resources;

namespace Lithnet.ActiveDirectory.PasswordChange.Web.Models
{
    [Localizable(true)]
    public class PasswordChangeRequestModel
    {
        [Required(ErrorMessageResourceType = typeof(UIMessages), ErrorMessageResourceName = "UserNameIsRequired")]
        public string UserName { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(UIMessages), ErrorMessageResourceName = "CurrentPasswordIsRequired")]
        public string CurrentPassword { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(UIMessages), ErrorMessageResourceName = "NewPasswordIsRequired")]
        public string NewPassword { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(UIMessages), ErrorMessageResourceName = "ConfirmPasswordIsRequired")]
        public string ConfirmNewPassword { get; set; }

        public bool Success { get; set; }

        public string Redirect { get; set; }
    }
}