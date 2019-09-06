using MF_Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoUI.Controllers;
using MFTool;
using MF_Base.Model;
using UIBase;
using AutoUI.Helper;
using System.Linq.Expressions;
using System.Data;

namespace AutoUI.Areas.ConfigUIDef.Controllers
{
    [Export]
    public class ListController : BaseController
    {
        #region List
        public ActionResult List()
        {
            return View();
        }

        public JsonResult GetList()
        {            
            int totalCount = 0;
            int pageId = Convert.ToInt32(QueryString("page"));
            int pageSize = Convert.ToInt32(QueryString("rows"));
            string mainType = QueryString("MainType");
            string dbName = QueryString("DBName");
            Expression<Func<ListConfig, bool>> condition = null;
            if (!string.IsNullOrEmpty(mainType))
            {
                condition = a => a.MainTypeFullId.Contains(mainType);
            }
            else if (!string.IsNullOrEmpty(dbName))
            {
                condition = a => a.DBName == dbName;
            }

            IEnumerable<ListConfig> baseForms = UnitOfWork.GetByPage<ListConfig, string>(out totalCount, pageSize, pageId, a => a.Id, false, condition);
            return Json(new { rows = baseForms, total = totalCount });
        }

        public JsonResult Delete()
        {
            string ids = QueryString("ids");
            var idArr = ids.Split(',');
            UnitOfWork.Delete<ListConfig>(a => idArr.Contains(a.Id));
            UnitOfWork.Commit();
            return Json("删除成功");
        }
        #endregion

        #region Basic
        public ActionResult Basic()
        {
            return View();
        }

        public ActionResult EntityList()
        {
            return View();
        }

        public JsonResult Get()
        {
            string id = QueryString("id");
            id.CheckNotNullOrEmpty("id");
            ListConfig cList = UnitOfWork.GetByKey<ListConfig>(id);
            cList.CheckNotNull("cList");
            return Json(cList);
        }

        public JsonResult GetDBNameList()
        {
            var dbNames = WebConfigHelper.GetDBNames().Select(a => new { id = a, text = a });
            return Json(dbNames);
        }

        [ValidateInput(false)]
        public JsonResult Save()
        {
            var dic = Request.Form["formData"].JsonToObject<Dictionary<string, object>>();

            if (string.IsNullOrEmpty(dic.GetValue("Id")))
            {
                ListConfig cList = ConvertHelper.ConvertToObj<ListConfig>(dic);
                cList.Id = GuidHelper.CreateTimeOrderID();
                cList.ButtonSetting = DefaultValue.GetListButtonJson();
                cList.PropertySetting = DefaultValue.GetListPropertyJson();
                UnitOfWork.Add(cList);
            }
            else
            {
                ListConfig cList = UnitOfWork.GetByKey<ListConfig>(dic.GetValue("Id"));
                ConvertHelper.UpdateEntity(cList, dic);
                UnitOfWork.UpdateEntity(cList);
            }

            bool b = UnitOfWork.Commit();
            return Json(b);
        }

        public JsonResult GetEntityList()
        {
            var dbNames = WebConfigHelper.GetDBNames();
            List<dynamic> list = new List<dynamic>();
            var exsitEntityFullNames = UnitOfWork.Get<FormConfig>().Select(a => a.EntityFullName);

            //根据entityfullname筛选，同时排除已经定义的实体
            foreach (var name in dbNames)
            {
                string baseEntityName = "Entity";
                list.AddRange(CommonHelper.GetClassFullNames(name, baseEntityName)
                    .Select(a => new { DBName = name, EntityFullName = a.GetValue("FullName"), Name = a.GetValue("Description"), TableName = a.GetValue("ClassName") }));
            }

            return Json(list);
        }

        #endregion

        #region Property
        public ActionResult Property()
        {
            return View();
        }
        public JsonResult GetProperty()
        {
            string id = QueryString("id");
            id.CheckNotNullOrEmpty("id");
            ListConfig cList = UnitOfWork.GetByKey<ListConfig>(id);
            cList.CheckNotNull("cList");
            return Json(cList.PropertySetting);
        }

        public JsonResult SaveProperty(string id)
        {
            id.CheckNotNullOrEmpty("id");
            var property = Request.Form["formData"];
            property.CheckNotNullOrEmpty("formData");
            UnitOfWork.Update<ListConfig>(a => new ListConfig { PropertySetting = property }, d => d.Id == id);
            bool b = UnitOfWork.Commit();
            return Json(b);
        }
        #endregion

        #region Column
        public ActionResult Column()
        {
            return View();
        }

        public ActionResult ColumnDetail()
        {
            return View();
        }

        public JsonResult SaveColumn(string listId)
        {
            var dicList = QueryString("rows").JsonToDictionaryList();
            ListConfig fc = UnitOfWork.GetByKey<ListConfig>(listId);
            fc.CheckNotNull("ListConfig");
            fc.UpdateColumn(dicList);
            UnitOfWork.UpdateEntity(fc);
            bool res = UnitOfWork.Commit();
            return Json(res);
        }

        public JsonResult GetColumnList()
        {
            string id = QueryString("listId");
            ListConfig fc = UnitOfWork.GetByKey<ListConfig>(id);
            fc.CheckNotNull("ListConfig");
            return Json(fc.GetColumnList());
        }

        public JsonResult GetColumnFromSQL()
        {
            string id = QueryString("listId");
            ListConfig list = UnitOfWork.GetByKey<ListConfig>(id);
            list.CheckNotNull("ListConfig");
            string connName = WebConfigHelper.GetConnSettingNameByDBName(list.DBName);

            if (!string.IsNullOrEmpty(list.TableName) && !string.IsNullOrEmpty(connName))
            {
                string sql = string.Format("select top 1 * FROM {0}", list.TableName);
                SqlHelper sqlHelper = new SqlHelper(connName);
                var dt = sqlHelper.ExcuteTable(sql); 

                List<string> colNames = new List<string>();
                foreach(DataColumn dc in dt.Columns)
                    colNames.Add(dc.ColumnName);

                if (colNames.Count() > 0)
                {
                    return Json(colNames);
                }
            }

            return Json(false);
        }
        #endregion

        #region Button
        public ActionResult Button()
        {
            return View();
        }

        public ActionResult ButtonDetail()
        {
            string id = QueryString("Id");
            ListConfig fc = UnitOfWork.GetByKey<ListConfig>(id);
            fc.CheckNotNull("ListConfig");
            ViewBag.FunctionList = fc.GetScriptFunctionList().Select(a => new { text = a, value = a }).ToJson();
            return View();
        }

        public JsonResult SaveButton(string listId)
        {
            var dicList = QueryString("rows").JsonToDictionaryList();
            ListConfig fc = UnitOfWork.GetByKey<ListConfig>(listId);
            fc.CheckNotNull("ListConfig");
            fc.UpdateButton(dicList);
            UnitOfWork.UpdateEntity(fc);
            bool res = UnitOfWork.Commit();
            return Json(res);
        }

        public JsonResult GetButtonList()
        {
            string id = QueryString("listId");
            ListConfig fc = UnitOfWork.GetByKey<ListConfig>(id);
            fc.CheckNotNull("ListConfig");
            return Json(fc.GetButtonList());
        }
        #endregion
    }
}