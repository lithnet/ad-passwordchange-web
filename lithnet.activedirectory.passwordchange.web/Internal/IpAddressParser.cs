using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class IpAddressParser
    {
        private IpAddressDetectionElement config;

        public IpAddressParser()
        {
            this.config = PasswordChangeConfigSection.Configuration.IPAddressDetection;
        }

        public string GetRequestIP(HttpRequestBase request)
        {
            if (!string.IsNullOrWhiteSpace(this.config.ClientIpHeaderName))
            {
                string knownClientIP = request.Headers[this.config.ClientIpHeaderName];
                if (!string.IsNullOrWhiteSpace(knownClientIP))
                {
                    return knownClientIP;
                }
            }

            if ((this.config.TrustedProxyDepth <= 0 && this.config.TrustedProxyIPs.Count == 0) || string.IsNullOrWhiteSpace(this.config.XffHeaderName))
            {
                return request.UserHostAddress;
            }

            List<string> hostList = request.Headers[this.config.XffHeaderName]?.Split(',').ToList() ?? new List<string>();

            if (hostList.Count == 0)
            {
                return request.UserHostAddress;
            }

            if (this.config.TrustedProxyIPs.Count > 0)
            {
                for (int i = hostList.Count - 1; i >= 0; i--)
                {
                    if (!this.config.TrustedProxyIPs.OfType<string>().Any(t => t.Equals(hostList[i], StringComparison.OrdinalIgnoreCase)))
                    {
                        return hostList[i];
                    }
                }

                return request.UserHostAddress;
            }

            if (this.config.TrustedProxyDepth >= hostList.Count)
            {
                return hostList[0];
            }
            else
            {
                return hostList[hostList.Count - this.config.TrustedProxyDepth - 1];
            }
        }
    }
}
