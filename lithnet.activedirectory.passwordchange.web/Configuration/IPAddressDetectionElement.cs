using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class IpAddressDetectionElement : ConfigurationElement
    {
        private const string PropXffHeaderName = "xffHeaderName";
        private const string PropClientIPHeaderName = "clientIPHeaderName";
        private const string PropTrustedProxyList = "trustedProxyList";
        private const string PropTrustedProxyDepth = "trustedProxyDepth";

        private List<string> proxyIPs;
        
        [ConfigurationProperty(PropXffHeaderName, IsRequired = false, DefaultValue = "X-Forwarded-For")]
        public string XffHeaderName => (string)this[PropXffHeaderName];

        [ConfigurationProperty(PropClientIPHeaderName, IsRequired = false)]
        public string ClientIpHeaderName => (string)this[PropClientIPHeaderName];

        //[ConfigurationProperty(PropTrustedProxyList, IsRequired = false)]
        //[TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        //public List<string> TrustedProxyIPs
        //{
        //    get
        //    {
        //        if (this.proxyIPs == null)
        //        {
        //            this.proxyIPs = ((string)this[PropTrustedProxyList] ?? string.Empty).Split(',').ToList();
        //        }

        //        return this.proxyIPs;
        //    }
        //}


        [ConfigurationProperty(PropTrustedProxyList, IsRequired = false)]
        [TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        public StringCollection TrustedProxyIPs => (StringCollection)this[PropTrustedProxyList];
      
        [ConfigurationProperty(PropTrustedProxyDepth, IsRequired = false, DefaultValue = 0)]
        public int TrustedProxyDepth => (int)this[PropTrustedProxyDepth];
    }
}