using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Resources;
using System.Configuration;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using MFTool;

namespace UIBase
{
    public class ScriptModule : IHttpModule
    {
        #region IHttpModule 成员

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            // 捕获全局未处理的异常
            context.Error += new EventHandler(context_Error);
            context.PreSendRequestHeaders += new EventHandler(PreSendRequestHeadersHandler);
            context.BeginRequest += new EventHandler(BeginRequest);

        }
        #endregion

        private void PreSendRequestHeadersHandler(object sender, EventArgs args)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpResponse response = application.Response;


            #region 处理页面权限

            #endregion

            if (response.StatusCode == 302)
            {
                if (application.Request.Headers["X-Requested-With"] == "XMLHttpRequest") //Ajax请求
                {

                }
            }
        }

        private void context_Error(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;
            HttpResponse response = ctx.Response;
            HttpRequest request = ctx.Request;
            Exception ex = ctx.Server.GetLastError();

            string sheader = request.Headers["X-Requested-With"];

            bool isDebug = false;
            if (System.Configuration.ConfigurationManager.AppSettings["IsDebug"] != null)
            {
                isDebug = System.Configuration.ConfigurationManager.AppSettings["IsDebug"] == "true";
            }
            bool isAjaxRequest = (sheader != null && sheader == "XMLHttpRequest") ? true : false;
            bool isBusinessException = ex.GetType() == typeof(BusinessException);

            if (isAjaxRequest)
            {
                string msg = ex.ToString();
                if (isBusinessException) msg = ex.Message;
                else
                {
                    Logger.GetCurMethodLog().Error("出错了", ex);
                    msg = isDebug ? ex.ToString() : "出错了,请联系管理员!";
                }
                ctx.ClearError();
                ctx.Response.StatusCode = 500;//恢复异常代号设置
                ctx.Response.Clear();
                ctx.Response.Write(new { IsNoticeBox = (isDebug && !isBusinessException), Msg = msg }.ToJson());
                ctx.Response.End();
            }
            else
            {
                ctx.ClearError();
                ctx.Response.Clear();
                string js = GetSource();
                if (isDebug && !isBusinessException)
                {
                    Logger.GetCurMethodLog().Error("出错了", ex);
                    string settings = "{ title:\"异常\", maxmin: true}";
                    js += "<script>noticeBox('<div style=\"padding:10px\">" + ex.ToString().Replace("\r\n", "<br/>") + "</div>'," + settings + ")</script>";
                }
                else
                {
                    string msg = isBusinessException ? ex.Message : "出错了,请联系管理员!";
                    js += "<script>msgBox('" + msg + "')</script>";
                }
                ctx.Response.Write(js);
                ctx.Response.End();
            }           

            #region to be deleted
            //业务逻辑异常,让ajax回调仍然走success,但是要附带参数说明未通过内部业务逻辑验证
            //见 commitAjax mfRealUpLoadFile
            //if (ex.GetType() == typeof(BusinessException))
            //{
            //    if (!isDebug)
            //    {
            //        //清除错误代码，避免前端ajax走fail的回调
            //        ctx.ClearError();
            //        ctx.Response.Clear();
            //        ctx.Response.Write(new { IsNoPass = true, Msg = ex.Message }.ToJson());
            //        ctx.Response.End();
            //    }
            //    //走fail回调
            //    else
            //    {
            //        string js = GetSource();
            //        js += "<script>msgBox('" + ex.Message + "')</script>";
            //        ctx.Response.Write(js);
            //        ctx.Response.End();
            //    }
            //}
            //else
            //{
            //    string sheader = request.Headers["X-Requested-With"];
            //    bool isAjaxRequest = (sheader != null && sheader == "XMLHttpRequest") ? true : false;
            //    //ajax 错误直接由前端获取并提示错误信息
            //    if (!isAjaxRequest)
            //    {
            //        if (request.RawUrl.Contains(".aspx") == false)
            //        {
            //            ctx.ClearError();
            //            ctx.Response.Clear();
            //            string js = GetSource();
            //            string settings = "{ title:\"异常\", maxmin: true}";
            //            js += "<script>noticeBox('<div style=\"padding:10px\">" + ex.ToString().Replace("\r\n", "<br/>") + "</div>'," + settings + ")</script>";
            //            ctx.Response.Write(js);
            //            ctx.Response.End();
            //        }
            //    }
            //}    
            #endregion
        }

        private void BeginRequest(object sender, EventArgs args)
        {
            var request = HttpContext.Current.Request;
            string charFilter = System.Configuration.ConfigurationManager.AppSettings["CharFilter"];
            if (string.IsNullOrEmpty(charFilter))
            {
                return;
            }

            foreach (var str in charFilter.Split('|'))
            {
                for (int i = request.Cookies.Count - 1; i >= 0; i--)
                {
                    var cookie = request.Cookies[i];
                    if (".ASPXAUTH,ASP.NET_SessionId".Split(',').Contains(cookie.Name) == false)
                        continue;
                    if (cookie.Value.Contains(str))
                    {
                        returnErr("Cookie【" + cookie.Name + "】中包含非法字符，请重新输入或者联系管理员！");
                    }
                }

                foreach (var key in request.Form.AllKeys.Clone() as string[])
                {
                    var value = request.Form[key];
                    if (value.Contains(str))
                    {
                        returnErr("页面【" + key + "】的内容包含了非法字符，请重新输入或者联系管理员！");
                    }
                }

                foreach (var key in request.QueryString.AllKeys)
                {
                    var value = request.QueryString[key];
                    if (value.Contains(str))
                    {
                        returnErr("地址栏参数包含非法字符，请重新输入或者联系管理员！");
                    }
                }
            }

        }

        private void returnErr(string err)
        {
            HttpContext.Current.ClearError();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(err);
            HttpContext.Current.Response.StatusCode = 500;
            HttpContext.Current.Response.End();
        }

        private string GetSource()
        {
            string res = "<script type='text/javascript' src='/BaseResource/Plugin/jquery/jquery.min.js'></script>";
            res += "<script type='text/javascript' src='/BaseResource/Plugin/layer/layer.js'></script>";
            res += "<script type='text/javascript' src='/BaseResource/Script/mfbase.js'></script>";
            res += "<link type='text/css' href='/BaseResource/Css/Site.css' rel='stylesheet' />";
            return res;
        }
    }
}
