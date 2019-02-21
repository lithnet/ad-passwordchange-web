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
        private const string PropCustomLinks = "custom-links";
        private const string PropIpDetection = "ip-detection";
        private const string PropBranding = "branding";
        private const string PropHibpEnabled = "hibpEnabled";
        private const string PropLppEnabled = "lppEnabled";
        private const string PropTestPasswordManagerEnabled = "tpmEnabled";

        internal static PasswordChangeConfigSection GetConfiguration()
        {
            PasswordChangeConfigSection section = (PasswordChangeConfigSection)ConfigurationManager.GetSection(SectionName);

            if (section == null)
            {
                section = new PasswordChangeConfigSection();
            }

            return section;
        }

        [ConfigurationProperty(PropIpDetection, IsRequired = false)]
        public IpAddressDetectionElement IPAddressDetection => (IpAddressDetectionElement)this[PropIpDetection];

        [ConfigurationProperty(PropRateLimitIP, IsRequired = false)]
        public RateLimitIPElement RateLimitIP => (RateLimitIPElement)this[PropRateLimitIP];

        [ConfigurationProperty(PropRateLimitUser, IsRequired = false)]
        public RateLimitUserElement RateLimitUser => (RateLimitUserElement)this[PropRateLimitUser];

        [ConfigurationProperty(PropBranding, IsRequired = false)]
        public BrandingElement Branding => (BrandingElement)this[PropBranding];

        [ConfigurationProperty(PropHibpEnabled, IsRequired = false, DefaultValue = false)]
        public bool HibpEnabled => (bool)this[PropHibpEnabled];

        [ConfigurationProperty(PropLppEnabled, IsRequired = false, DefaultValue = false)]
        public bool LppEnabled => (bool)this[PropLppEnabled];

        [ConfigurationProperty(PropTestPasswordManagerEnabled, IsRequired = false, DefaultValue = false)]
        public bool TestPasswordManagerEnabled => (bool)this[PropTestPasswordManagerEnabled];

        [ConfigurationProperty(PropReturnUrls)]
        [ConfigurationCollection(typeof(AllowedReturnUrlCollection), AddItemName = "allowed-return-url", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public AllowedReturnUrlCollection AllowedReturnUrls => (AllowedReturnUrlCollection)this[PropReturnUrls];

        [ConfigurationProperty(PropCustomLinks)]
        [ConfigurationCollection(typeof(CustomLinkCollection), AddItemName = "custom-link", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public CustomLinkCollection CustomLinks => (CustomLinkCollection)this[PropCustomLinks];

        public static PasswordChangeConfigSection Configuration { get; private set; }

        static PasswordChangeConfigSection()
        {
            PasswordChangeConfigSection.Configuration = PasswordChangeConfigSection.GetConfiguration();
        }
    }
}