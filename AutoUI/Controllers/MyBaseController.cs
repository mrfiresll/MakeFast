//using AutoUI;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace JJOA.Controllers
//{
//    [Export]
//    public class MyBaseController : Controller
//    {
//        /// <summary>
//        /// 当前登录的用户属性
//        /// </summary>
//        public StaffInfoDTO CurrentStraffInfo { get { return Session[CommonStr.SessionUserKey] as StaffInfoDTO; } }
//        //当前用户权限信息
//        public ArrRoleFunc RoleFunc { get { return Session[CommonStr.SessionRoleFuncKey] as ArrRoleFunc; } }

//        ///// <summary>
//        ///// 重新基类在Action执行之前的事情
//        ///// </summary>
//        ///// <param name="filterContext">重写方法的参数</param>
//        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
//        //{
//        //    if (filterContext.HttpContext.Session != null)
//        //    {
//        //        if (Session[CommonStr.SessionUserKey] == null || Session[CommonStr.SessionRoleFuncKey] == null)
//        //        {
//        //            //if (filterContext.HttpContext.Request.Url != null)
//        //            //{
//        //            //    string firstRqUrl = filterContext.HttpContext.Request.Url.ToString();
//        //            //    filterContext.HttpContext.Session[CommonStr.SessionRawUrl] = firstRqUrl;
//        //            //}

//        //            MessageBox.ShowAndRedirect("非法访问或者访问已过期，请重新登录系统!", "/Login/Index", "top");
//        //        }
//        //    }
//        //    base.OnActionExecuting(filterContext);
//        //}

//        protected override void OnException(ExceptionContext filterContext)
//        {
//            base.OnException(filterContext);

//            //错误记录
//            //WHC.Framework.Commons.LogTextHelper.Error(filterContext.Exception);

//            // 当自定义显示错误 mode = On，显示友好错误页面
//            if (filterContext.HttpContext.IsCustomErrorEnabled)
//            {
//                filterContext.ExceptionHandled = true;
//                this.View("Error").ExecuteResult(this.ControllerContext);
//            }
//        }
//    }
//}
