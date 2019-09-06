using MF_Base.Model;
using MFTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIBase;
using AutoUI.Controllers;

namespace AutoUI.Areas.ConfigUIDef.Controllers
{
    [Export]
    public class FormCtrlDetailController : BaseController
    {
        //public ActionResult TextBox()
        //{
        //    return View();
        //}

        //public ActionResult ComboBox()
        //{
        //    return View();
        //}

        //public ActionResult EnumDef()
        //{ 
        //    return View();
        //}

        public JsonResult GetEnumList()
        {
            var res = UnitOfWork.Get<MF_Enum>().Select(a => new { Code = a.Code, Name = a.Name }).Distinct();
            return Json(res);
        }

        public JsonResult GetSubDataGrid(string formId,string fieldName)
        {
            formId.CheckNotNullOrEmpty("formId");
            FormConfig cForm = UnitOfWork.GetByKey<FormConfig>(formId);
            cForm.CheckNotNull("FormConfig");
            var resList = cForm.GetSubTableDicList(fieldName,false);

            //默认值
            foreach(var res in resList)
            {
                res.SetValue("IsVisible", "是");
                res.SetValue("Enable", "是");
                res.SetValue("ColumnName", res.GetValue("FieldName"));
            }
            return Json(resList);
        }

        public JsonResult RefreshSubDataGridProperty(string formId, string fieldName)
        {
            formId.CheckNotNullOrEmpty("formId");
            FormConfig cForm = UnitOfWork.GetByKey<FormConfig>(formId);
            cForm.CheckNotNull("FormConfig");
            var resList = cForm.GetSubTableDicList(fieldName, false);
            //默认值
            foreach (var res in resList)
            {
                res.SetValue("IsVisible", "是");
                res.SetValue("Enable", "是");
                res.SetValue("ColumnName", res.GetValue("FieldName"));
            }
            return Json(resList);
        }
    }
}