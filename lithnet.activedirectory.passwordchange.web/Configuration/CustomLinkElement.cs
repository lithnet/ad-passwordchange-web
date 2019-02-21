using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class CustomLinkElement : ConfigurationElement
    {
        private const string PropId = "id";
        private const string PropLinkText = "text";
        private const string PropLinkUrl = "url";

        [ConfigurationProperty(PropId, IsRequired = true)]
        public string ID => (string)this[PropId];

        [ConfigurationProperty(PropLinkText, IsRequired = true)]
        public string LinkText => (string)this[PropLinkText];

        [ConfigurationProperty(PropLinkUrl, IsRequired = true)]
        public string LinkUrl => (string)this[PropLinkUrl];
    }
}