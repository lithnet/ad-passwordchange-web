using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class RateLimitUserElement : ConfigurationElement
    {
        private const string PropReqPerMinute = "requestsPerMinute";
        private const string PropReqPerSecond = "requestsPerSecond";
       
        [ConfigurationProperty(PropReqPerMinute, IsRequired = false, DefaultValue = 15)]
        public int ReqPerMinute => (int)this[PropReqPerMinute];

        [ConfigurationProperty(PropReqPerSecond, IsRequired = false, DefaultValue = 5)]
        public int ReqPerSecond => (int)this[PropReqPerSecond];
    }
}