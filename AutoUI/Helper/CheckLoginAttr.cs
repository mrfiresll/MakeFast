using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UIBase;

namespace AutoUI
{
    public class CheckLoginAttr : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
             object[] attrs = filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoCheckLoginAttr), true);
            if(attrs.Length == 1)
            {
                 
            }
            else
            {
                var cookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie == null)// || filterContext.HttpContext.Session[CommonStr.SessionRoleFuncKey] == null)
                    {
                        //if (filterContext.HttpContext.Request.Url != null)
                        //{
                        //    string firstRqUrl = filterContext.HttpContext.Request.Url.ToString();
                        //    filterContext.HttpContext.Session[CommonStr.SessionRawUrl] = firstRqUrl;
                        //}

                        MessageBox.ShowAndRedirect("非法访问或者访问已过期，请重新登录系统!", "/AutoUI/Login/Index", "top");
                    }
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}