using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace lithnet.activedirectory.passwordchange.web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            List<IPasswordManager> EnabledPasswordServices = new List<IPasswordManager>();

            if (ConfigurationManager.AppSettings["HIBPEnabled"] == "true")
            {
                EnabledPasswordServices.Add(new HIBPPasswordManager());
            }
            if (ConfigurationManager.AppSettings["LPPEnabled"] == "true")
            {
                EnabledPasswordServices.Add(new LppPasswordManager());
            }
            if (ConfigurationManager.AppSettings["TestEnabled"] == "true")
            {
                EnabledPasswordServices.Add(new TestPasswordManager());
            }

            AggregatePasswordManager passwordManager = new AggregatePasswordManager(EnabledPasswordServices);

            container.RegisterInstance<IPasswordManager>(passwordManager);

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}