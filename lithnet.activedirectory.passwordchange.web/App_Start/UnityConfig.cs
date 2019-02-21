using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            List<IPasswordManager> EnabledPasswordServices = new List<IPasswordManager>();

            if (PasswordChangeConfigSection.Configuration.PasswordTesting.HibpEnabled)
            {
                EnabledPasswordServices.Add(new HibpPasswordManager());
            }

            if (PasswordChangeConfigSection.Configuration.PasswordTesting.LppEnabled)
            {
                EnabledPasswordServices.Add(new LppPasswordManager());
            }

            if (PasswordChangeConfigSection.Configuration.PasswordTesting.TestPasswordManagerEnabled)
            {
                EnabledPasswordServices.Add(new TestPasswordManager());
            }

            AggregatePasswordManager passwordManager = new AggregatePasswordManager(EnabledPasswordServices);

            container.RegisterInstance<IPasswordManager>(passwordManager);
            container.RegisterInstance<RateLimiter>(new RateLimiter(new IpAddressParser()));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}