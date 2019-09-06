using System.Web.Mvc;

namespace WebBase.Areas.KendoUI
{
    public class KendoUIAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "KendoUI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "KendoUI_default",
                "KendoUI/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
