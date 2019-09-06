using MFTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace AutoUI
{
    public class LoggerFilter:FilterAttribute,IActionFilter
    {
        public string Description { get; set; }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //StaffInfoDTO who = filterContext.HttpContext.Session[CommonStr.SessionUserKey] as StaffInfoDTO;
            //string strWho = who.RealName;
            //string strDoWhat = Description;
            //if(string.IsNullOrEmpty(strDoWhat))
            //{
            //    strDoWhat = filterContext.RouteData.Values["controller"].ToString();
            //    strDoWhat +=  "/" + filterContext.RouteData.Values["action"].ToString();
            //}

            //string strParam;
            //if(filterContext.HttpContext.Request.RequestType == "GET")
            //{
            //    strParam = HttpUtility.UrlDecode(filterContext.HttpContext.Request.QueryString.ToString());
            //}
            //else if (filterContext.HttpContext.Request.RequestType == "POST")
            //{
            //    strParam = HttpUtility.UrlDecode(filterContext.HttpContext.Request.Form.ToString());
            //}
            //else
            //{
            //    strParam = "即非Get亦非post";
            //}
   
            //string strFinal = "";

            //if (filterContext.Result.GetType() == typeof(ContentResult))
            //{
            //    string strRes = ((ContentResult)filterContext.Result).Content;
            //    if (strRes.ToLower().Contains("true"))
            //    {
            //        strFinal = string.Format("操作人：{0}, 操作内容：{1}, 参数为：{2}, 执行结果：{3}", strWho, strDoWhat, strParam, "成功");
            //    }
            //    else if (strRes.ToLower().Contains("false"))
            //    {
            //        strFinal = string.Format("操作人：{0}, 操作内容：{1}, 参数为：{2}, 执行结果：{3}", strWho, strDoWhat, strParam, "失败");
            //        Logger.GetCurMethodLog().Warn(strFinal);
            //        return;
            //    }
            //    else
            //    {
            //        strFinal = string.Format("操作人：{0}, 操作内容：{1}, 参数为：{2}, 执行结果：{3}", strWho, strDoWhat, strParam, strRes);
            //    }
            //}
            //else 
            //{
            //    strFinal = string.Format("操作人：{0}, 操作内容：{1}, 参数为：{2}", strWho, strDoWhat, strParam);
            //}
            
            //Logger.GetCurMethodLog().InfoAsync(strFinal);
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            return;//throw new NotImplementedException();
        }

        //public void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    throw new NotImplementedException();
        //}
    }
}