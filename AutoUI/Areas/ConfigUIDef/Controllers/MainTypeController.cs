using MF_Base.Model;
using MFTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIBase;
using AutoUI.Helper;

namespace AutoUI.Areas.ConfigUIDef.Controllers
{
    [Export]
    public class MainTypeController : BaseController
    {
        public JsonResult GetTypeTree()
        {
            var mainTypeTree = CommonHelper.GetMainTypeTree();
            var easyuiTree = EasyUIHelper.GetTreeJson(mainTypeTree);
            return Json(easyuiTree, false);
        }

        public JsonResult GetTypeTreeTop()
        {
            var mainTypeTree = CommonHelper.GetMainTypeTopTree();
            var easyuiTree = EasyUIHelper.GetTreeJson(mainTypeTree);
            return Json(easyuiTree, false);
        }

        public JsonResult AddMainType(string dbName, string ParentId, string FullId)
        {
            MF_MainType mainType = new MF_MainType();
            mainType.Id = GuidHelper.CreateTimeOrderID();

            //非顶部节点
            if (ParentId != dbName)
            {
                mainType.ParentId = ParentId;
            }

            mainType.FullId = FullId + "." + mainType.Id;
            mainType.DBName = dbName;
            mainType.Text = "新增节点";
            UnitOfWork.Add(mainType);
            UnitOfWork.Commit();
            return Json(mainType);
        }

        public JsonResult RemoveMainType(string fullId)
        {
            UnitOfWork.Delete<MF_MainType>(a => a.FullId.Contains(fullId));
            return Json(UnitOfWork.Commit());
        }

        public JsonResult UpdateMainTypeName(string id, string text)
        {
            var mainType = UnitOfWork.GetByKey<MF_MainType>(id);
            UnitOfWork.Update<MF_MainType>(a => new MF_MainType { Text = text }, b => b.Id == id);
            return Json(UnitOfWork.Commit());
        }

        public JsonResult GetMainTypeList(string dbName)
        {
            var list = UnitOfWork.Get<MF_MainType>(a => a.DBName == dbName).Select(a => new { value = a.FullId, text = a.Text });
            return Json(list);
        }

        public JsonResult MoveMainType(string sourceID, string targetID, string point)
        {
            var source = UnitOfWork.GetByKey<MF_MainType>(sourceID);
            if (point == "append")
            {
                MF_MainType maxOrderItem = null;
                if (targetID == CommonStr.MainTypeTreeRootID)
                {
                    source.ParentId = null;
                    source.FullId = source.Id;
                    maxOrderItem = UnitOfWork.Get<MF_MainType>(a => a.ParentId == null).OrderByDescending(a => a.OrderIndex).FirstOrDefault();
                }
                else
                {
                    var target = UnitOfWork.GetByKey<MF_MainType>(targetID);
                    maxOrderItem = UnitOfWork.Get<MF_MainType>(a => a.ParentId == targetID).OrderByDescending(a => a.OrderIndex).FirstOrDefault();

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
                var target = UnitOfWork.GetByKey<MF_MainType>(targetID);
                var targetPre = UnitOfWork.Get<MF_MainType>(a => a.ParentId == target.ParentId && a.OrderIndex < target.OrderIndex)
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
                if (!string.IsNullOrEmpty(target.ParentId))
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
                var target = UnitOfWork.GetByKey<MF_MainType>(targetID);
                var targetNext = UnitOfWork.Get<MF_MainType>(a => a.ParentId == target.ParentId && a.OrderIndex > target.OrderIndex)
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