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
using AutoUI.Helper;
using System.Configuration;

namespace AutoUI.Areas.Sys.Controllers
{
    [Export]
    public class MFDepartmentController : BaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult DepartmentSelector()
        {
            return View();
        }

        public JsonResult GetTree()
        {
            var departList = UnitOfWork.Get<MF_Department>().ToDicList().ToList();
            string topId = CommonStr.MainTypeTreeRootID;
            string topName = ConfigurationManager.AppSettings["AppTitle"];
            foreach (var depart in departList)
            {
                if (string.IsNullOrEmpty(depart.GetValue("ParentId")))
                {
                    depart.SetValue("ParentId", topId);
                }
                depart.SetValue("Text", depart.GetValue("Name"));
            }
            var topDic = (new MF_Department()).ToDictionary();
            topDic.SetValue("Id", topId);
            topDic.SetValue("Text", topName);
            topDic.SetValue("ParentId", "");
            departList.Insert(0, topDic);
            return Json(EasyUIHelper.GetTreeJson(departList),false);
        }

        public JsonResult AddDepartment(string ParentId, string FullId)
        {
            MF_Department depart = new MF_Department();
            depart.Id = GuidHelper.CreateTimeOrderID();
             

            depart.ParentId = ParentId;
            depart.Name = "新增部门";
            depart.FullId = FullId + "." + depart.Id;
            UnitOfWork.Add(depart);
            UnitOfWork.Commit();
            return Json(depart);
        }

        public JsonResult RemoveDepartment(string fullId)
        {
            UnitOfWork.Delete<MF_Department>(a => a.FullId.Contains(fullId));
            return Json(UnitOfWork.Commit());
        }

        public JsonResult UpdateDepartmentName(string id, string name)
        {
            var mainType = UnitOfWork.GetByKey<MF_Department>(id);
            UnitOfWork.Update<MF_Department>(a => new MF_Department { Name = name }, b => b.Id == id);
            return Json(UnitOfWork.Commit());
        }

        public JsonResult GetDepartUser(string departmentFullId)
        {
            if (!string.IsNullOrEmpty(departmentFullId))
            {
                return Json(UnitOfWork.Get<MF_User>(a => departmentFullId.Contains(a.MF_DepartmentId)));
            }
            else
            {
                return Json(UnitOfWork.Get<MF_User>());
            }
        }

        public JsonResult removeUsersDepartment(string users)
        {
            var dicList = users.JsonToDictionaryList();
            var idArr = dicList.Select(a => a.GetValue("Id"));

            UnitOfWork.Update<MF_User>(a => new MF_User { MF_DepartmentId = null }, b => idArr.Contains(b.Id));
            return Json(UnitOfWork.Commit());
        }

        public JsonResult SetUserDepartment(string departmentId,string userId)
        {
            UnitOfWork.Update<MF_User>(a => new MF_User { MF_DepartmentId = departmentId }, b => b.Id == userId);
            return Json(UnitOfWork.Commit());
        }

        public JsonResult MoveDepartment(string sourceID, string targetID, string point)
        {
            var source = UnitOfWork.GetByKey<MF_Department>(sourceID);
            if (point == "append")
            {
                MF_Department maxOrderItem = null;
                if (targetID == CommonStr.MainTypeTreeRootID)
                {
                    source.ParentId = null;
                    source.FullId = source.Id;
                    maxOrderItem = UnitOfWork.Get<MF_Department>(a => a.ParentId == null).OrderByDescending(a => a.OrderIndex).FirstOrDefault();
                }
                else
                {
                    var target = UnitOfWork.GetByKey<MF_Department>(targetID);
                    maxOrderItem = UnitOfWork.Get<MF_Department>(a => a.ParentId == targetID).OrderByDescending(a => a.OrderIndex).FirstOrDefault();

                    source.ParentId = targetID;
                    source.FullId = target.FullId + "." + source.Id;                    
                }

                source.OrderIndex = 0;
                if (maxOrderItem != null)
                {
                    source.OrderIndex = maxOrderItem.OrderIndex + 1;
                }
            }
            else if (point == "top")
            {
                var target = UnitOfWork.GetByKey<MF_Department>(targetID);
                var targetPre = UnitOfWork.Get<MF_Department>(a => a.ParentId == target.ParentId && a.OrderIndex < target.OrderIndex)
                    .OrderByDescending(a => a.OrderIndex).FirstOrDefault();

                double orderIndex = 0;
                if (targetPre != null)
                {
                    orderIndex = (targetPre.OrderIndex + target.OrderIndex) / 2;
                }
                else
                {
                    orderIndex = target.OrderIndex - 1;
                }

                source.OrderIndex = orderIndex;
                source.ParentId = target.ParentId;
                if(!string.IsNullOrEmpty(target.ParentId))
                {
                    source.FullId = target.Parent.FullId + "." + source.Id;
                }
                else
                {
                    source.FullId = source.Id;
                }
            }
            else if (point == "bottom")
            {
                var target = UnitOfWork.GetByKey<MF_Department>(targetID);
                var targetNext = UnitOfWork.Get<MF_Department>(a => a.ParentId == target.ParentId && a.OrderIndex > target.OrderIndex)
                    .OrderBy(a => a.OrderIndex).FirstOrDefault();

                double orderIndex = 0;
                if (targetNext != null)
                {
                    orderIndex = (targetNext.OrderIndex + target.OrderIndex) / 2;
                }
                else
                {
                    orderIndex = target.OrderIndex + 1;
                }

                source.OrderIndex = orderIndex;
                source.ParentId = target.ParentId;
                if (!string.IsNullOrEmpty(target.ParentId))
                {
                    source.FullId = target.Parent.FullId + "." + source.Id;
                }
                else
                {
                    source.FullId = source.Id;
                }
            }
            return Json(UnitOfWork.Commit());
        }
    }       
}