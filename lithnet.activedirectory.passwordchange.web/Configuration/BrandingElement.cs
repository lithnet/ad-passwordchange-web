using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class BrandingElement : ConfigurationElement
    {
        private const string PropCompanyName = "companyName";
        private const string PropAppName = "appName";

        [ConfigurationProperty(PropCompanyName, IsRequired = false)]
        public string CompanyName => (string)this[PropCompanyName];

        [ConfigurationProperty(PropAppName, IsRequired = false)]
        public string AppName => (string)this[PropAppName];

        public string Header => string.Join(" - ", this.AppName, this.CompanyName);
    }
}