using BaseConfig;
using MFTool;
using WebBase;
using WebBase.MetronicHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBase.Controllers
{
    [Export]
    public class MainMetronicController : Controller
    {
        [Import]
        IFuncRepository _funcService;

        [LoggerFilter(Description = "进入主界面")]
        public ActionResult Index()
        {
            var modules = _funcService.R_Get(a => a.EnumFuncType == EnumFuncType.Module.ToString());
            ViewBag.Modules = modules.Select(a => new MetronicHorizontalMenuItem() { Id = a.Id.ToString(), Name = a.Text, Icon = a.IconCls, Action = "insertDomFromAction('dynamicSideMenu','/Main/GetPages?parentId=" + a.Id + "')" }).ToList();
            return View();
        }

        public ActionResult IndexTest() { return View(); }

        public ActionResult IndexKendo() { return View(); }

        public JsonResult GetPages(string parentId)
        {
            var tmps = _funcService.R_Get(a => a.ParentId == parentId || a.Parent.ParentId == parentId);

            List<MetronicSideMenuItem> items = tmps.Select(a => new MetronicSideMenuItem() { Id = a.Id.ToString(), Url = a.Url, ParentId = a.ParentId.ToString(), Name = a.Text, Icon = a.IconCls }).ToList();
            var res = new MetronicSideMenu(items);
            return Json(res.GetHtml());
        }
    }
}
