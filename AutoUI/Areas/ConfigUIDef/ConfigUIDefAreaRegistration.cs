using System.Web.Mvc;

namespace AutoUI.Areas.ConfigUIDef
{
    public class ConfigUIDefAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ConfigUIDef";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ConfigUIDef_default",
                "ConfigUIDef/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
