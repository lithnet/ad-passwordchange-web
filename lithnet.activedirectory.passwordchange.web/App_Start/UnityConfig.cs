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

            if (ConfigurationManager.AppSettings["HIBPEnabled"] == "true")
            {
                container.RegisterType<IPasswordManager, HIBPPasswordManager>();
            }
            else
            {
                container.RegisterType<IPasswordManager, TestPasswordManager>();
            }

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}