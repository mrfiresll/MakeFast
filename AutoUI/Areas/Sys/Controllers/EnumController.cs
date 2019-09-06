using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIBase;
using MFTool;
using MF_Base.Model;
using System.ComponentModel.Composition;
using System.Linq.Expressions;
using AutoUI.Areas.ConfigUI.Controllers;

namespace AutoUI.Areas.Sys.Controllers
{
    [Export]
    public class EnumController : EntityFormController<MF_Enum>
    {
        public ActionResult Item()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public JsonResult GetList()
        {
            int totalCount = 0;
            int pageId = Convert.ToInt32(QueryString("page"));
            int pageSize = Convert.ToInt32(QueryString("rows"));
            string dbName = QueryString("DBName");
            Expression<Func<MF_Enum, bool>> condition = null;
            if (!string.IsNullOrEmpty(dbName))
            {
                condition = a => a.DBName == dbName;
            }

            IEnumerable<MF_Enum> baseForms = UnitOfWork.GetByPage<MF_Enum, string>(out totalCount, pageSize, pageId, a => a.Id, false, condition);
            return Json(new { rows = baseForms, total = totalCount });
        }

        public JsonResult GetEnum()
        {
            string id = QueryString("id");
            return Json(UnitOfWork.GetByKey<MF_Enum>(id));
        }

        public JsonResult SaveEnum()
        {
            var dic = Request.Form["formData"].JsonToObject<Dictionary<string, object>>();

            if (string.IsNullOrEmpty(dic.GetValue("Id")))
            {
                MF_Enum mfEnum = ConvertHelper.ConvertToObj<MF_Enum>(dic);
                mfEnum.Id = GuidHelper.CreateTimeOrderID();
                mfEnum.CreateUserID = GetCurrentUserID();
                mfEnum.CreateUserName = GetCurrentUserName();
                mfEnum.ModifyUserID = GetCurrentUserID();
                mfEnum.ModifyUserName = GetCurrentUserName();
                UnitOfWork.Add(mfEnum);
            }
            else
            {
                MF_Enum mfEnum = UnitOfWork.GetByKey<MF_Enum>(dic.GetValue("Id"));
                ConvertHelper.UpdateEntity(mfEnum, dic);
                mfEnum.ModifyUserID = GetCurrentUserID();
                mfEnum.ModifyUserName = GetCurrentUserName();
                UnitOfWork.UpdateEntity(mfEnum);
            }

            bool b = UnitOfWork.Commit();
            return Json(b);
        }

        public JsonResult SaveEnumDetail(string data, string enumId)
        {
            data.CheckNotNull("枚举明细为空数据");
            var detailList = data.JsonToObject<List<MF_EnumDetail>>();
            int orderIndex = 0;
            foreach (var detail in detailList)
            {
                if (string.IsNullOrEmpty(detail.Id))
                {
                    detail.Id = GuidHelper.CreateTimeOrderID();
                    detail.MF_EnumId = enumId;
                    detail.OrderIndex = orderIndex;
                    UnitOfWork.Add(detail);
                }
                else
                {
                    var dbDetail = UnitOfWork.GetByKey<MF_EnumDetail>(detail.Id);                    
                    ConvertHelper.UpdateEntity(dbDetail, detail.ToDictionary());
                    dbDetail.OrderIndex = orderIndex;
                }
                orderIndex++;
            }
            var existIdArr = detailList.Select(a => a.Id);
            UnitOfWork.Delete<MF_EnumDetail>(a => !existIdArr.Contains(a.Id) && a.MF_EnumId == enumId);
            return Json(UnitOfWork.Commit());
        }

        public JsonResult GetEnumDetail()
        {
            string id = QueryString("EnumId");
            return Json(UnitOfWork.Get<MF_EnumDetail>().Where(a => a.MF_EnumId == id).OrderBy(a => a.OrderIndex));
        }
    }
}