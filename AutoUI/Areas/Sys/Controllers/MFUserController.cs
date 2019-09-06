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
using MF_Base;

namespace AutoUI.Areas.Sys.Controllers
{
    [Export]
    public class MFUserController : BaseController
    {
        public ActionResult Pwd()
        {
            return View();
        }

        public ActionResult Role()
        {
            return View();
        }

        public ActionResult SingleSelector()
        {
            return View();
        }

        public ActionResult MultiSelector()
        {
            return View();
        }

        public JsonResult ResetPwd()
        {
            string id = QueryString("Id");
            string pwd = QueryString("PassWord");
            id.CheckNotNullOrEmpty("id不能为空");
            UnitOfWork.Update<MF_User>(a => new MF_User { PassWord = Md5.GetMd5Str32(pwd) }, d => d.Id == id);
            return Json(UnitOfWork.Commit());
        }

        public JsonResult GetUserList()
        {
            int totalCount = 0;
            int pageId = Convert.ToInt32(QueryString("page"));
            int pageSize = Convert.ToInt32(QueryString("rows"));
            string realName = QueryString("RealName");
            Expression<Func<MF_User, bool>> express = null;
            if (!string.IsNullOrEmpty(realName))
                express = a => a.RealName.Contains(realName);
            IEnumerable<MF_User> baseForms = UnitOfWork.GetByPage<MF_User, string>(out totalCount, pageSize, pageId, a => a.Id, false, express);
            return Json(new { rows = baseForms, total = totalCount });
        }

        public JsonResult GetUserRoleList()
        {
            string userId = QueryString("UserId");
            string roleType = QueryString("RoleType");
            var query = UnitOfWork.GetQuery<MF_Role>().GroupJoin(UnitOfWork.GetQuery<MF_RoleUser>().Where(a => a.MF_UserId == userId).DefaultIfEmpty(), a => a.Id, b => b.MF_RoleId,
                (a, b) => new { a.Id, a.Code, a.Name, a.EnumRoleType, Set = b.Count() != 0 ? "true" : "false" }).Where(a => a.EnumRoleType == roleType);
            return Json(query);
        }

        public JsonResult SaveUserRole()
        {
            string userId = QueryString("UserId");
            var dicList = QueryString("rows").JsonToDictionaryList();
            foreach (var dic in dicList)
            {
                string roleId = dic.GetValue("Id");
                if (dic.GetValue("Set") == "true")
                {
                    if (!UnitOfWork.IsExist<MF_RoleUser>(a => a.MF_UserId == userId
                     && a.MF_RoleId == roleId))
                    {
                        MF_RoleUser toAdd = new MF_RoleUser();
                        toAdd.Id = GuidHelper.CreateTimeOrderID();
                        toAdd.MF_RoleId = roleId;
                        toAdd.MF_UserId = userId;
                        UnitOfWork.Add<MF_RoleUser>(toAdd);
                    }
                }
                else
                {
                    UnitOfWork.Delete<MF_RoleUser>(a => a.MF_RoleId == roleId && a.MF_UserId == userId);
                }                
            }
            return Json(UnitOfWork.Commit());
        }

        public JsonResult GetRoleList()
        {
            return Json(UnitOfWork.Get<MF_Role>());
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