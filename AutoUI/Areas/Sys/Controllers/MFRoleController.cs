using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIBase;
using MFTool;
using MF_Base.Model;
using System.ComponentModel.Composition;
using MF_Base;

namespace AutoUI.Areas.Sys.Controllers
{
    [Export]
    public class MFRoleController : BaseController
    {
        public ActionResult List()
        {
            ViewBag.RoleType = QueryString("RoleType");
            return View();
        }

        public ActionResult Item()
        {
            return View();
        }

        public JsonResult SaveRole()
        {
            string roleType = QueryString("RoleType");
            roleType.CheckNotNullOrEmpty("角色枚举类型不能为空");
            var dic = Request.Form["formData"].JsonToObject<Dictionary<string, object>>();
            MF_Role role = ConvertHelper.ConvertToObj<MF_Role>(dic);
            role.EnumRoleType = roleType;
            if (string.IsNullOrEmpty(role.Id))
            {
                role.Id = GuidHelper.CreateTimeOrderID();
                UnitOfWork.Add(role);
            }
            else
            {
                UnitOfWork.UpdateEntity(role);
            }

            return Json(UnitOfWork.Commit());
        }

        public JsonResult GetRole(string id)
        {
            id.CheckNotNullOrEmpty("id");
            var func = UnitOfWork.GetByKey<MF_Role>(id);
            return Json(func);
        }

        public JsonResult DeleteRole()
        {
            string ids = QueryString("ids");
            var idArr = ids.Split(',');
            UnitOfWork.Delete<MF_Role>(a => idArr.Contains(a.Id));
            UnitOfWork.Commit();
            return Json("删除成功");
        }

        public JsonResult GetList()
        {
            string roleType = QueryString("RoleType");
            roleType.CheckNotNullOrEmpty("角色枚举类型不能为空");

            int totalCount = 0;
            int pageId = Convert.ToInt32(QueryString("page"));
            int pageSize = Convert.ToInt32(QueryString("rows"));
            IEnumerable<MF_Role> baseForms = UnitOfWork.GetByPage<MF_Role, string>
                (out totalCount, pageSize, pageId, a => a.Id, false, a => a.EnumRoleType == roleType);
            return Json(new { rows = baseForms, total = totalCount });
        }

        public JsonResult GetRoleUserList()
        {
            string roleId = QueryString("RoleId");
            var query = UnitOfWork.GetQuery<MF_RoleUser>().GroupJoin(UnitOfWork.GetQuery<MF_User>(), a => a.MF_UserId, b => b.Id
                , (a, b) => new { a, b }).Where(a => a.a.MF_RoleId == roleId).SelectMany(a => a.b);
            return Json(query);
        }
    }
}