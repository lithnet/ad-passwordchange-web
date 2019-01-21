using System.Web;
using System.Web.Mvc;

namespace lithnet.activedirectory.passwordchange.web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
