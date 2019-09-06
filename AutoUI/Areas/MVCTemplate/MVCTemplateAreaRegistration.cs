using System.Web.Mvc;

namespace WebBase.Areas.MVCTemplate
{
    public class MVCTemplateAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MVCTemplate";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MVCTemplate_default",
                "MVCTemplate/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
