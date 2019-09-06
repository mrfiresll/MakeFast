using AutoUI;
using System.Web;
using System.Web.Mvc;

namespace AutoUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CheckLoginAttr());
            //FilterProviders.Providers.Add()
        }
    }
}