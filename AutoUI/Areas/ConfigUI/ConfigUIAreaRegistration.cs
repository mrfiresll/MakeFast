using System.Web.Mvc;

namespace AutoUI.Areas.ConfigUI
{
    public class ConfigUIAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ConfigUI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ConfigUI_default",
                "ConfigUI/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
