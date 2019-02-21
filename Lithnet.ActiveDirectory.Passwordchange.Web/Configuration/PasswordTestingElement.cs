using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class PasswordTestingElement : ConfigurationElement
    {
        private const string PropHibpEnabled = "hibpEnabled";
        private const string PropLppEnabled = "lppEnabled";
        private const string PropTestPasswordManagerEnabled = "tpmEnabled";


        [ConfigurationProperty(PropHibpEnabled, IsRequired = false, DefaultValue = false)]
        public bool HibpEnabled => (bool)this[PropHibpEnabled];

        [ConfigurationProperty(PropLppEnabled, IsRequired = false, DefaultValue = false)]
        public bool LppEnabled => (bool)this[PropLppEnabled];

        [ConfigurationProperty(PropTestPasswordManagerEnabled, IsRequired = false, DefaultValue = false)]
        public bool TestPasswordManagerEnabled => (bool)this[PropTestPasswordManagerEnabled];
    }
}