using MF_WorkFlow;
using MF_WorkFlow.Model;
using MFTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIBase;

namespace WorkFlow.Controllers
{
    [Export]
    public class WFListController : BaseController
    {
        public ActionResult ListTab()
        {
            return View();
        }

        public JsonResult GetToDoList()
        {
            int totalCount = 0;
            int pageId = Convert.ToInt32(QueryString("page"));
            int pageSize = Convert.ToInt32(QueryString("rows"));
            IEnumerable<dynamic> baseForms = UnitOfWork.GetByPage<WFInst, DateTime?>(out totalCount, pageSize, pageId, a => a.ModifyTime, false, null,
                "WDefInst", "WFStep", "WFStep.WFNodeDefInst").Select(a => new
                {
                    Id = a.Id,
                    FormInstId = a.FormInstId,
                    CurrentStepName = a.GetCurrentStep() == null ? "" : a.GetCurrentStep().WFNodeDefInst.Name,
                    DefName = a.WDefInst.Name,
                    SendTime = a.GetCurrentStep() == null ? "" : a.GetCurrentStep().CreateTime.ToString(),
                    StepUserId = a.GetCurrentStep() == null ? "" : a.GetCurrentStep().StepUserId,
                    StepUserName = a.GetCurrentStep() == null ? "" : a.GetCurrentStep().StepUserName,
                }).Where(a => a.StepUserId.Contains(GetCurrentUserID()));
            return Json(new { rows = baseForms, total = totalCount });
        }
        public JsonResult GetDoneList()
        {
            int totalCount = 0;
            int pageId = Convert.ToInt32(QueryString("page"));
            int pageSize = Convert.ToInt32(QueryString("rows"));
            IEnumerable<dynamic> baseForms = UnitOfWork.GetByPage<WFStep, DateTime?>(out totalCount, pageSize, pageId, a => a.ModifyTime, false, null,
                "WFInst", "WFNodeDefInst", "WFInst.WDefInst").Select(a => new
                {
                    Id = a.Id,
                    FormInstId = a.WFInst.FormInstId,
                    StepName = a.WFNodeDefInst.Name,
                    DefName = a.WFInst.WDefInst.Name,
                    SendTime = a.CreateTime.ToString(),
                    OperateTime = a.OperateTime == null ? "" : a.OperateTime.Value.ToString(),
                    OperateUserId = a.OperateUserId,
                    OperateUserName = a.OperateUserName
                }).Where(a => a.OperateUserId == GetCurrentUserID());
            return Json(new { rows = baseForms, total = totalCount });
        }
    }
}
