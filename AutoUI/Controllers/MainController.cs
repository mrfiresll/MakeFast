using MFTool;
using AutoUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MF_Base;
using EFBase;
using MF_Base.Model;
using UIBase;

namespace AutoUI.Controllers
{
    [Export]
    public class MainController : BaseController
    {
        [LoggerFilter(Description = "进入主界面")]
        public ActionResult Index()
        {
            ViewBag.AppTitle = ConfigurationManager.AppSettings["AppTitle"];
            ViewBag.CurrentUserName = GetCurrentUserName();
            return View();
        }
        public ActionResult Home()
        {
            return View();
        }

        public JsonResult GetChildrenFuncWithSub(string id)
        {
            var funcList = UnitOfWork.Get<MF_Func>(a => a.ParentId == id, "Children").OrderBy(a => a.OrderIndex);
            var dicList = new List<Dictionary<string, object>>();
            foreach (var func in funcList)
            {
                var dic = func.ToDictionary();
                dic.SetValue("Children", func.Children.OrderBy(a => a.OrderIndex));
                dicList.Add(dic);
            }
            return Json(dicList,false);
        }

        public JsonResult GetChildrenFunc(string id)
        {
            var res = UnitOfWork.Get<MF_Func>(a => a.ParentId == id).OrderBy(a => a.OrderIndex);
            return Json(res, false);
        }

        public JsonResult GetFirstMenus()
        {
            var modules = UnitOfWork.Get<MF_Func>(a => a.EnumFuncType == EnumFuncType.Module.ToString());
            if (modules.Count() > 0)
            {
                return GetChildrenFunc(modules.First().Id);
            }
            return Json("");
        }

        public JsonResult GetModules()
        {
            var res = UnitOfWork.Get<MF_Func>(a => a.EnumFuncType == EnumFuncType.Module.ToString()).OrderBy(a => a.OrderIndex);
            return Json(res,false);
        }

        public JsonResult GetFunc(string id)
        {
            var a = UnitOfWork.GetByKey<MF_Func>(id);
            return Json(a,false);
        }
    }
}
