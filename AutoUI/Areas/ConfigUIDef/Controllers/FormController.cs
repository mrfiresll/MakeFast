using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using EFBase;
using MF_Base;
using MFTool;
using AutoUI.Controllers;
using System.Text;
using MF_Base.Model;
using UIBase;
using System.Linq.Expressions;

namespace AutoUI.Areas.ConfigUIDef.Controllers
{
    [Export]
    public class FormController : BaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Ctrl()
        {
            return View();
        }

        public ActionResult Layout()
        {
            return View();
        }

        public ActionResult EntityList()
        {
            return View();
        }

        public ActionResult TextBoxDefine()
        {
            return View();
        }

        public JsonResult AddForm()
        {
            var dataStr = QueryString("data");
            FormConfig cForm = dataStr.JsonToObject<FormConfig>();
            cForm.Id = GuidHelper.CreateTimeOrderID();
            cForm.MainTypeFullId = cForm.DBName + "."+ cForm.Id;
            cForm.UICode = cForm.EntityFullName.Replace(".", "_");//xx_xxx_xx
            cForm.ReCreateCtrl();            
            UnitOfWork.Add(cForm);
            bool b = UnitOfWork.Commit();
            return Json(b);
        }

        [ValidateInput(false)]
        public JsonResult SaveForm()
        {
            var dic = Request.Form["formData"].JsonToObject<Dictionary<string, object>>();

            if (string.IsNullOrEmpty(dic.GetValue("Id")))
            {
                FormConfig cForm = ConvertHelper.ConvertToObj<FormConfig>(dic);
                cForm.Id = GuidHelper.CreateTimeOrderID();
                cForm.ReCreateCtrl();
                UnitOfWork.Add(cForm);
            }
            else
            {
                FormConfig cForm = UnitOfWork.GetByKey<FormConfig>(dic.GetValue("Id"));
                ConvertHelper.UpdateEntity(cForm, dic, true, "CtrlSetting", "LayOutSetting");
                UnitOfWork.UpdateEntity(cForm);
            }

            bool b = UnitOfWork.Commit();
            return Json(b);
        }

        public JsonResult GetForm()
        {
            string id = QueryString("id");
            id.CheckNotNullOrEmpty("id");
            FormConfig cForm = UnitOfWork.GetByKey<FormConfig>(id);
            cForm.CheckNotNull("FormConfig");
            return Json(cForm);
        }

        public JsonResult GetEntityList()
        {
            string entityFullName = QueryString("EntityFullName").ToLower();
            var dbNames = WebConfigHelper.GetDBNames();
            List<dynamic> list = new List<dynamic>();
            var exsitEntityFullNames = UnitOfWork.Get<FormConfig>().Select(a => a.EntityFullName);

            //根据entityfullname筛选，同时排除已经定义的实体
            foreach (var name in dbNames)
            {
                string baseEntityName = "Entity";
                list.AddRange(CommonHelper.GetClassFullNames(name, baseEntityName)
                    .Where(a => (string.IsNullOrEmpty(entityFullName) ? true : a.GetValue("FullName").ToLower().Contains(entityFullName)) && !exsitEntityFullNames.Contains(a.GetValue("FullName")))
                    .Select(a => new { DBName = name, EntityFullName = a.GetValue("FullName"), Name = a.GetValue("Description") }));
            }
            
            return Json(list);
        }

        public JsonResult GetList()
        {
            int totalCount = 0;
            int pageId = Convert.ToInt32(QueryString("page"));
            int pageSize = Convert.ToInt32(QueryString("rows"));
            string mainType = QueryString("MainType");
            string dbName = QueryString("DBName");
            Expression<Func<FormConfig, bool>> condition = null;
            if (!string.IsNullOrEmpty(mainType))
            {
                condition = a => a.MainTypeFullId.Contains(mainType);
            }
            else if (!string.IsNullOrEmpty(dbName))
            {
                condition = a => a.DBName == dbName;
            }

            IEnumerable<FormConfig> baseForms = UnitOfWork.GetByPage<FormConfig,string>(out totalCount, pageSize, pageId, a => a.Id, false, condition);
            return Json(new { rows = baseForms, total = totalCount });
        }

        public JsonResult Delete()
        {
            string ids = QueryString("ids");
            var idArr = ids.Split(',');
            UnitOfWork.Delete<FormConfig>(a => idArr.Contains(a.Id));
            UnitOfWork.Commit();
            return Json("删除成功");
        }

        public JsonResult GetCtrlList()
        {
            string id = QueryString("formId");
            FormConfig fc = UnitOfWork.GetByKey<FormConfig>(id);
            fc.CheckNotNull("FormConfig");
            return Json(fc.GetCtrlAttrList());
        }

        public JsonResult SaveCtrl(string formId)
        {
            var dicList = QueryString("rows").JsonToDictionaryList();
            FormConfig fc = UnitOfWork.GetByKey<FormConfig>(formId);
            fc.CheckNotNull("FormConfig");
            fc.UpdateCtrl(dicList);
            UnitOfWork.UpdateEntity(fc);
            bool res = UnitOfWork.Commit();
            return Json(res);
        }

        public JsonResult RefreshProperty(string formId)
        {
            formId.CheckNotNullOrEmpty("formId");
            FormConfig cForm = UnitOfWork.GetByKey<FormConfig>(formId);
            cForm.CheckNotNull("FormConfig");
            cForm.ReCreateCtrl();
            bool res = UnitOfWork.Commit();
            return Json(res);
        }

        [ValidateInput(false)]
        public JsonResult SaveLayout(string formId, string html)
        {
            formId.CheckNotNullOrEmpty("formId");
            FormConfig cForm = UnitOfWork.GetByKey<FormConfig>(formId);
            cForm.CheckNotNull("FormConfig");
            cForm.LayOutSetting = Server.HtmlEncode(html);
            bool res = UnitOfWork.Commit();
            return Json(res);
        }

        public JsonResult GetLayout(string formId)
        {
            formId.CheckNotNullOrEmpty("formId");
            FormConfig cForm = UnitOfWork.GetByKey<FormConfig>(formId);
            cForm.CheckNotNull("FormConfig");
            return Json(Server.HtmlDecode(cForm.LayOutSetting ?? ""));
        }

        public JsonResult CreateLayout(string formId, string layoutType)
        {
            formId.CheckNotNullOrEmpty("formId");
            FormConfig cForm = UnitOfWork.GetByKey<FormConfig>(formId);
            cForm.CheckNotNull("FormConfig");
            var ctrlList = cForm.GetCtrlAttrList();
            StringBuilder formHtmlSB = new StringBuilder();
            //title
            //hidden dom
            formHtmlSB.Append("<input id='Id' name='Id' type='hidden' />");
            var noCtrlList = ctrlList.Where(a => string.IsNullOrEmpty(a.GetValue("CtrlType")));
            foreach (var ctrl in noCtrlList)
            {
                if (!string.IsNullOrEmpty(ctrl.GetValue("FieldName")))
                    formHtmlSB.Append(string.Format("<input id='{0}' name='{0}' type='hidden' />", ctrl.GetValue("FieldName")));
            }

            //visible dom
            var withCtrlList = ctrlList.Where(a => !string.IsNullOrEmpty(a.GetValue("CtrlType"))).ToList();
            int ctrlCount = withCtrlList.Count();
            double dCol = 0;
            double.TryParse(layoutType, out dCol);
            if (dCol == 0)
                return Json("");


            double tmp = ctrlCount / dCol;
            int rowCount = Convert.ToInt32(Math.Ceiling(tmp));

            double leftTdWithPercent = 15;
            double rightTdWithPercent = Math.Round(100 / dCol, 2) - 15;
            if(rightTdWithPercent < 0)
            {
                rightTdWithPercent = leftTdWithPercent = Math.Round(100 / Convert.ToDouble(2 * dCol), 0);
            }

            formHtmlSB.Append("<table class='groupTable'>");
            //title
            formHtmlSB.Append(string.Format("<tr><td colspan='{0}' class='title'>{1}</td></tr>", 2 * dCol, cForm.Name));

            //row
            int ctrlIndex = 0;
            for (int i = 0; i < rowCount; i++)
            {
                formHtmlSB.Append("<tr>");
                for (int j = 0; j < dCol; j++)
                {
                    string ctrlColName = ""; 
                    if(ctrlIndex < withCtrlList.Count)
                    {
                        var ctrl = withCtrlList[ctrlIndex];
                        ctrlColName = ctrl.GetValue("ColumnName");
                        if(string.IsNullOrEmpty(ctrlColName))
                        {
                            ctrlColName = ctrl.GetValue("FieldName");
                        }
                    }
                    
                    //left
                    formHtmlSB.Append(string.Format("<td style='width:{0}%'>{1}</td>", leftTdWithPercent,
                        string.IsNullOrEmpty(ctrlColName) ? "" : (ctrlColName + ":")));
                    //right
                    formHtmlSB.Append(string.Format("<td style='width:{0}%'>{1}</td>", rightTdWithPercent,
                        string.IsNullOrEmpty(ctrlColName) ? "" : ("{" + ctrlColName + "}")));
                    ctrlIndex++;
                }
                formHtmlSB.Append("</tr>");
            }
            formHtmlSB.Append("</table>");


            return Json(formHtmlSB.ToString());
        }
    }
}
