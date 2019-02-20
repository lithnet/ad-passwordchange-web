using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class AllowedReturnUrlElement : ConfigurationElement
    {
        private const string PropValue = "value";

        [ConfigurationProperty(AllowedReturnUrlElement.PropValue, IsRequired = true)]
        public string Value => (string)this[AllowedReturnUrlElement.PropValue];
    }
}