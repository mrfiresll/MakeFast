using MFTool;
using MF_Base;
using AutoUI.Controllers;
using AutoUI.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using MF_Base.Model;
using UIBase;

namespace AutoUI.Areas.Sys.Controllers
{
    [Export]
    public class AuthorTreeManageController : BaseController
    {
        public ActionResult List()
        {
            //ViewBag.RoleFunc = RoleFunc;
            return View();
        }

        public ActionResult Item()
        {
            return View();
        }

        public JsonResult GetFuncTreeJson()
        {
            var res = UnitOfWork.Get<MF_Func>();
            return Json(EasyUIHelper.GetTreeJson(res));
        }

        public JsonResult GetFunc(string id)
        {
            id.CheckNotNullOrEmpty("id");
            var func = UnitOfWork.GetByKey<MF_Func>(id);
            return Json(func);
        }

        public JsonResult AddEqFunc(string data)
        {
            MF_Func func = data.JsonToObject<MF_Func>();
            MF_Func funcToAdd = new MF_Func();
            funcToAdd.Id = GuidHelper.CreateTimeOrderID();
            funcToAdd.Text = "新增项";
            funcToAdd.ParentId = func.ParentId;
            if (func.EnumFuncType == EnumFuncType.Module.ToString())
            {
                funcToAdd.EnumFuncType = EnumFuncType.Module.ToString();
            }
            else if (func.EnumFuncType == EnumFuncType.Menu.ToString())
            {
                funcToAdd.EnumFuncType = EnumFuncType.Menu.ToString();
            }
            else if (func.EnumFuncType == EnumFuncType.Page.ToString())
            {
                funcToAdd.EnumFuncType = EnumFuncType.Page.ToString();
            }
            else if (func.EnumFuncType == EnumFuncType.Button.ToString())
            {
                funcToAdd.EnumFuncType = EnumFuncType.Button.ToString();
            }

            var res = UnitOfWork.Add(funcToAdd);
            UnitOfWork.Commit();
            return Json(res);
        }

        public JsonResult AddSubFunc(string data)
        {
            MF_Func func = data.JsonToObject<MF_Func>();
            MF_Func funcToAdd = new MF_Func();
            funcToAdd.Id = GuidHelper.CreateTimeOrderID();
            funcToAdd.Text = "新增项";
            funcToAdd.ParentId = func.Id;
            if (func.EnumFuncType == EnumFuncType.Module.ToString())
            {
                funcToAdd.EnumFuncType = EnumFuncType.Menu.ToString();
            }
            else if (func.EnumFuncType == EnumFuncType.Menu.ToString())
            {
                funcToAdd.EnumFuncType = EnumFuncType.Page.ToString();
            }
            else if (func.EnumFuncType == EnumFuncType.Page.ToString())
            {
                funcToAdd.EnumFuncType = EnumFuncType.Button.ToString();
            }

            var res = UnitOfWork.Add(funcToAdd);
            UnitOfWork.Commit();
            return Json(res);
        }

        public JsonResult ExChangeOrder(string aId, string bId)
        {
            var a = UnitOfWork.GetByKey<MF_Func>(aId);
            var b = UnitOfWork.GetByKey<MF_Func>(bId);
            var orderIndex = a.OrderIndex;
            a.OrderIndex = b.OrderIndex;
            b.OrderIndex = orderIndex;

            return Json(UnitOfWork.Commit());
        }

        public JsonResult DeleteFunc(string id)
        {
            UnitOfWork.Delete<MF_Func>(GetAllChildren(id));
            UnitOfWork.DeleteByKey<MF_Func>(id);
            return Json(UnitOfWork.Commit());
        }

        public JsonResult Save()
        {
            var dic = Request.Form["formData"].JsonToObject<Dictionary<string, object>>();
            MF_Func cForm = ConvertHelper.ConvertToObj<MF_Func>(dic);

            if (string.IsNullOrEmpty(cForm.Id))
            {
                cForm.Id = GuidHelper.CreateTimeOrderID();
                UnitOfWork.Add(cForm);
            }
            else
            {
                UnitOfWork.UpdateEntity(cForm);
            }

            return Json(UnitOfWork.Commit());
        }

        private List<MF_Func> GetAllChildren(string funcId)
        {
            List<MF_Func> temps = new List<MF_Func>();
            var children = UnitOfWork.Get<MF_Func>(a => a.ParentId == funcId);
            foreach (var child in children)
            {
                temps.Add(child);
                temps.AddRange(GetAllChildren(child.Id));
            }
            return temps;
        }
    }
}
