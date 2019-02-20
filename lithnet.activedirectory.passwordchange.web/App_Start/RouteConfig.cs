using System.Web.Mvc;
using System.Web.Routing;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Change",
                url: "{controller}/{action}/{username}",
                defaults: new { controller = "Change", action = "Do", username = UrlParameter.Optional }
            );
        }
    }
}
