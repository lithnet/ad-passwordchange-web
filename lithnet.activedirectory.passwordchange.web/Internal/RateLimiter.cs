using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class RateLimiter
    {
        private IpAddressParser ipParser;

        public RateLimiter(IpAddressParser ipParser)
        {
            this.ipParser = ipParser;
        }

        public void ThrowOnRateLimitExceeded(string username, HttpRequestBase r)
        {
            string ip = this.ipParser.GetRequestIP(r);

            this.ThrowOnIpThresholdExceeded(username, ip, PasswordChangeConfigSection.Configuration.RateLimitIP.ReqPerSecond, 1);
            this.ThrowOnIpThresholdExceeded(username, ip, PasswordChangeConfigSection.Configuration.RateLimitIP.ReqPerMinute, 60);

            if (!string.IsNullOrWhiteSpace(username))
            {
                this.ThrowOnUserThresholdExceeded(username, ip, PasswordChangeConfigSection.Configuration.RateLimitUser.ReqPerSecond, 1);
                this.ThrowOnUserThresholdExceeded(username, ip, PasswordChangeConfigSection.Configuration.RateLimitUser.ReqPerMinute, 60);
            }
        }

        private void ThrowOnIpThresholdExceeded(string username, string ip, int threshold, int duration)
        {
            if (this.IsThresholdExceeded(ip, threshold, duration))
            {
                throw new RateLimitExceededException($"User {username} on IP address {ip} exceeded the per-IP rate limit of {threshold} requests in {duration} sections");
            }
        }

        private void ThrowOnUserThresholdExceeded(string username, string ip, int threshold, int duration)
        {
            if (this.IsThresholdExceeded(username, threshold, duration))
            {
                throw new RateLimitExceededException($"User {username} on IP address {ip} exceeded the per-user rate limit of {threshold} requests in {duration} sections");
            }
        }

        private bool IsThresholdExceeded(string usernameOrIP, int threshold, int duration)
        {
            string key1 = string.Join(@"-", duration, threshold, usernameOrIP);
            return this.IsThresholdExceededForKey(key1, threshold, duration);
        }

        private bool IsThresholdExceededForKey(string key, int threshold, int duration)
        {
            if (threshold <= 0)
            {
                return false;
            }

            int count = 1;

            if (HttpRuntime.Cache[key] != null)
            {
                count = (int)HttpRuntime.Cache[key] + 1;
            }

            HttpRuntime.Cache.Insert(
                key,
                count,
                null,
                DateTime.UtcNow.AddSeconds(duration),
                Cache.NoSlidingExpiration,
                CacheItemPriority.Low,
                null
            );

            return count > threshold;
        }
    }
}
