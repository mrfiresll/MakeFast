using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MFTool;
using EFBase;
using System.ComponentModel.Composition;
using UIBase;
using System.Web.Security;

namespace UIBase
{
    [Export]
    public class BaseController : Controller
    {
        [Import]
        protected IUnitOfWork UnitOfWork;

        Dictionary<string, object> _formDic;
        /// <summary>
        /// 获取地址栏参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string QueryString(string key)
        {
            if (_formDic == null)
                _formDic = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(Request["FormData"]) && Request["FormData"] != "[]")
                _formDic = Request["FormData"].JsonToObject<Dictionary<string, object>>(); 

            string value = Request.QueryString[key];
            if (string.IsNullOrEmpty(value))
                value = Request.Form[key];
            if (string.IsNullOrEmpty(value))
            {
                if (_formDic.ContainsKey(key))
                    value = _formDic[key].ToString();
            }

            if (value != null)
                return value;

            return string.Empty;
        }

        #region 处理不存在的Action

        protected override void HandleUnknownAction(string actionName)
        {
            if (Request.HttpMethod == "POST")
            {
                HttpContext.ClearError();
                HttpContext.Response.Clear();
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.Write("没有Action:" + actionName);
                HttpContext.Response.End();
            }

            // 搜索文件是否存在
            var filePath = "";
            if (RouteData.DataTokens["area"] != null)
                filePath = string.Format("~/Areas/{2}/Views/{1}/{0}.cshtml", actionName, RouteData.Values["controller"], RouteData.DataTokens["area"]);
            else
                filePath = string.Format("~/Views/{1}/{0}.cshtml", actionName, RouteData.Values["controller"]);
            if (System.IO.File.Exists(Server.MapPath(filePath)))
            {
                View(filePath).ExecuteResult(ControllerContext);
            }
            else
            {
                HttpContext.ClearError();
                HttpContext.Response.Clear();
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.Write("没有Action:" + actionName);
                HttpContext.Response.End();
            }
        }
        #endregion

        protected MFJsonResult Json(object obj, bool bBigCamelCase = true)
        {
            //if (obj != null && obj.GetType() == typeof(Boolean))
            //{
            //    string res = (Boolean)obj ? "操作成功" : "操作失败";
            //    return Json(res);
            //}
            return new MFJsonResult(obj, bBigCamelCase);
        }

        protected string GetCurrentUserID()
        {
            var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            cookie.CheckNotNull("cookie已过期");
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            return ticket.UserData;
        }
        protected string GetCurrentUserName()
        {
            return User.Identity.Name;
        }
    }
}