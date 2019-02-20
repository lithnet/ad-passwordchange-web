using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class PasswordChangeConfigSection : ConfigurationSection
    {
        private const string SectionName = "lithnet-password-change";
        private const string PropRateLimitIP = "rate-limit-ip";
        private const string PropRateLimitUser = "rate-limit-user";
        private const string PropReturnUrls = "allowed-return-urls";
        private const string PropIpDetection = "ip-detection";
        private const string PropHibpEnabled = "hibpEnabled";
        private const string PropLppEnabled = "lppEnabled";
        private const string PropTestPasswordManagerEnabled = "tpmEnabled";
        private const string PropGetHelpLink = "getHelpLink";

        internal static PasswordChangeConfigSection GetConfiguration()
        {
            PasswordChangeConfigSection section = (PasswordChangeConfigSection)ConfigurationManager.GetSection(SectionName);

            if (section == null)
            {
                section = new PasswordChangeConfigSection();
            }

            return section;
        }

        [ConfigurationProperty(PropGetHelpLink, IsRequired = false)]
        public string GetHelpLink => (string)this[PropGetHelpLink];

        [ConfigurationProperty(PropIpDetection, IsRequired = false)]
        public IpAddressDetectionElement IPAddressDetection => (IpAddressDetectionElement)this[PropIpDetection];

        [ConfigurationProperty(PropRateLimitIP, IsRequired = false)]
        public RateLimitIPElement RateLimitIP => (RateLimitIPElement)this[PropRateLimitIP];

        [ConfigurationProperty(PropRateLimitUser, IsRequired = false)]
        public RateLimitUserElement RateLimitUser => (RateLimitUserElement)this[PropRateLimitUser];

        [ConfigurationProperty(PropHibpEnabled, IsRequired = false, DefaultValue = false)]
        public bool HibpEnabled => (bool)this[PropHibpEnabled];

        [ConfigurationProperty(PropLppEnabled, IsRequired = false, DefaultValue = false)]
        public bool LppEnabled => (bool)this[PropLppEnabled];

        [ConfigurationProperty(PropTestPasswordManagerEnabled, IsRequired = false, DefaultValue = false)]
        public bool TestPasswordManagerEnabled => (bool)this[PropTestPasswordManagerEnabled];

        [ConfigurationProperty(PropReturnUrls)]
        [ConfigurationCollection(typeof(AllowedReturnUrlCollection), AddItemName = "allowed-return-url", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public AllowedReturnUrlCollection AllowedReturnUrls => (AllowedReturnUrlCollection)this[PropReturnUrls];

        public static PasswordChangeConfigSection Configuration { get; private set; }

        static PasswordChangeConfigSection()
        {
            PasswordChangeConfigSection.Configuration = PasswordChangeConfigSection.GetConfiguration();
        }
    }
}