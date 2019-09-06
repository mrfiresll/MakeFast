using BaseConfig;
using MFTool;
using WebBase.Controllers;
using WebBase.Models.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseConfig.Model.Sys;

namespace WebBase.Areas.FreeTest.Controllers
{
    [Export]
    public class DataTablesController : BaseController
    {
        [Import]
        IFormConfigRepository _repository;
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Param()
        {
            return View();
        }

        public ActionResult Form()
        {
            string baseDirectory = System.AppDomain.CurrentDomain.RelativeSearchPath;
            string modelDllPath = baseDirectory + "\\BaseConfig.dll";
            string baseEntityName = "Entity";
            List<KeyValuePair<string, string>> fullNames = ReflectionHelper.GetClassFullNames(modelDllPath, baseEntityName);
            ViewBag.NameSpaceList = fullNames.ToJson();
            return View();
        }

        public ActionResult TextBoxDefine()
        {
            return View();
        }

        public JsonResult Save()
        {
            var dic = Request.Form["formData"].JsonToObject<Dictionary<string, object>>();
            FormConfig cForm = ConvertHelper.ConvertToObj<FormConfig>(dic);

            if (string.IsNullOrEmpty(cForm.Id))
            {
                cForm.Id = GuidHelper.CreateTimeOrderID();
                _repository.R_Add(cForm);
            }
            else
            {
                _repository.R_Update(cForm);
            }

            bool b = _repository.Commit();
            return Json(b);
        }

        public JsonResult Get()
        {
            string id = QueryString("id");
            id.CheckNotNullOrEmpty("id");
            FormConfig cForm = _repository.R_Get(id);
            cForm.CheckNotNull("ConfigForm");
            return Json(cForm);
        }

        public JsonResult GetList()
        {
            int totalCount = 0;
            //IEnumerable<FormConfig> baseForms = _repository.R_GetByPage<string>(out totalCount, request.PageSize, request.Page, null, false, null);
            IReadOnlyList<FormConfig> baseForms = _repository.R_Get();
            var resultJson = new DataTablesResult<FormConfig>(baseForms);
            return Json(resultJson);
        }

        public JsonResult Delete(string listData)
        {
            var dicList = listData.JsonToObject<List<Dictionary<string, object>>>();
            var idList = dicList.Select(a => a.GetValue("Id").ToString());
            _repository.R_Delete(a => idList.Contains(a.Id));
            _repository.Commit();
            return Json("Success");
        }

        public JsonResult GetCtrlList()
        {
            string id = QueryString("formId");
            FormConfig fc = _repository.R_Get(id);
            fc.CheckNotNull("FormConfig");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(fc.CtrlSetting))
            {
                dic = fc.CtrlSetting.JsonToDictionary();
            }
            return Json(dic);
        }

        public JsonResult SaveParam(string formId, string models)
        {
            FormConfig fc = _repository.R_Get(formId);
            fc.CheckNotNull("FormConfig");
            fc.CtrlSetting = models;
            _repository.R_Update(fc);
            bool res = _repository.Commit();
            return Json(res);
        }
    }
}