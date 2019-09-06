using MFTool;
using AutoUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UIBase;
using MF_Base.Model;
using EFBase;
using MF_Base;

namespace AutoUI.Controllers
{
    [Export]
    [NoCheckLoginAttr]
    public class LoginController : BaseController
    {
        [NoCheckLoginAttr]
        public ActionResult Index()
        {
            if (Session[CommonStr.SessionUserKey] == null)// && Session[CommonStr.SessionRoleFuncKey] == null)
            {
                //先判断cookie是否有存储已登录信息，如果有赋值给session并直接跳转到主页
                HttpCookie cookie = Request.Cookies[CommonStr.LoginCookieKey];

                if (cookie != null && cookie[CommonStr.LoginCookieNameKey] != null)
                {
                    string name = cookie[CommonStr.LoginCookieNameKey];
                    MF_User currentUser = UnitOfWork.GetSingle<MF_User>(a => a.LoginName == name);

                    if (currentUser != null)
                    {
                        Session[CommonStr.SessionUserKey] = currentUser.ToDictionary();
                        //List<OAFuncDTO> all = _service.GetModels(currentUser.RoleArrs);
                        //Session[CommonStr.SessionRoleFuncKey] = new ArrRoleFunc(all);
                        return RedirectToAction("Index", "Main");
                    }
                }
            }
            return View();
        }

        [NoCheckLoginAttr]
        //[LoggerFilter(Description="用户登录操作")]
        public JsonResult UserLogin()
        {
            string name = "";
            if (Request.Form["Name"] != null)
            {
                name = Request.Form["Name"].ToString();
            }

            string pwd = "";
            if (Request.Form["Pwd"] != null)
            {
                pwd = Md5.GetMd5Str32(Request.Form["Pwd"]);
            }

            MF_User dto = UnitOfWork.GetSingle<MF_User>(a => a.LoginName == name);

            //可登录
            if (dto != null && dto.PassWord.ToLower() == pwd)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, dto.RealName, DateTime.Now, DateTime.Now.AddMinutes(30), true, dto.Id);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                cookie.HttpOnly = true;
                cookie.Domain = FormsAuthentication.CookieDomain;
                cookie.Path = FormsAuthentication.FormsCookiePath;
                HttpContext.Response.Cookies.Add(cookie);
                return Json(true);
            }
            else
            {
                return Json(false);
            }

        }
    }
}
