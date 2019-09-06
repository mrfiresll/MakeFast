using MF_Base.Model;
using MF_WorkFlow;
using MF_WorkFlow.Model;
using MFTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIBase;

namespace WorkFlow.Controllers
{
    [Export]
    public class WFFormController : AutoUI.Areas.ConfigUI.Controllers.FormController
    {
        public override ActionResult PageView(string UICode)
        {
            string formInstId = QueryString("FormInstId");
            string currentStepId = "";
            ViewBag.CanSave = false;//当前环节是否可以保存数据
            //获取步骤id
            if (!string.IsNullOrEmpty(formInstId))
            {
                var wfInst = UnitOfWork.GetSingle<WFInst>(a => a.FormInstId == formInstId);
                wfInst.CheckNotNull("FormInstId为【" + formInstId + "】流程实例为空");
                UICode = wfInst.WDefInst.Code;
                ViewBag.WFDefInstId = wfInst.WFDefInstId;
                var currentStep = wfInst.GetCurrentStep(GetCurrentUserID());

                //TODO:流程已执行完毕的判断
                if (currentStep == null)
                {
                    //TODO:提示没有该环节权限退出
                }
                else
                {
                    currentStepId = currentStep.Id;
                    //起始节点始终可以保存
                    ViewBag.CanSave = currentStep.WFNodeDefInst.CanSave;
                }

                ViewBag.NextRoutingDefInstList = new List<WFRoutingDefInst>();
                if (!string.IsNullOrEmpty(currentStepId))
                    ViewBag.NextRoutingDefInstList = GetNextRoutingDefInstsByStepId(currentStepId);
            }
            //首环节
            else
            {
                UICode.CheckNotNullOrEmpty("编号UICode不能为空");
                var wfDef = UnitOfWork.GetSingle<WFDef, double>(d => d.OrderIndex, false, a => a.Code == UICode);
                wfDef.CheckNotNull("未找到编号为【" + UICode + "】的流程定义");
                if (wfDef.IsPublish.ToLower() != "true")
                {
                    throw new BusinessException("流程还未发布");
                }
                var wfDefInst = UnitOfWork.GetSingle<WFDefInst, double>(d => d.OrderIndex, false, a => a.Code == UICode);
                wfDefInst.CheckNotNull("未找到编号为【" + UICode + "】的流程实例定义");                

                ViewBag.WFDefInstId = wfDefInst.Id;
                var wfNodeStart = wfDefInst.GetStartNode();
                wfNodeStart.CheckNotNull("流程起点为空");
                ViewBag.CanSave = wfNodeStart.CanSave;

                ViewBag.NextRoutingDefInstList = GetNextRoutingDefInstsByNodeDefInstId(wfNodeStart.Id);
            }
            //表单部分逻辑
            base.PageView(UICode);
            ViewBag.CurrentStepId = currentStepId;

            return View();
        }

        public ActionResult Trace()
        {
            string formInstId = QueryString("FormInstId");
            ViewBag.FormInstId = formInstId;
            formInstId.CheckNotNullOrEmpty("表实例id不能为空");
            var wfInst = UnitOfWork.GetSingle<WFInst>(a => a.FormInstId == formInstId);
            wfInst.CheckNotNull("FormInstId为【" + formInstId + "】流程实例为空");
            var UICode = wfInst.WDefInst.Code;
            //表单部分逻辑
            base.PageView(UICode);
            return View();
        }

        protected virtual void OnFlowEnd(Dictionary<string, object> entityDic)
        {

        }

        //新建
        protected override void BeforeAdd(Dictionary<string, object> dic)
        {
            //表单FlowPhase字段赋值
            dic.SetValue("FlowPhase", FlowState.Create.ToString());
            //如果仅仅是保存表单
            if (!string.IsNullOrEmpty(QueryString("JustSave")) && QueryString("JustSave").ToLower() == "true")
            {
                return;
            }

            string wFDefInstId = QueryString("wFDefInstId");
            wFDefInstId.CheckNotNullOrEmpty("wFDefInstId");
            var wfDefInst = UnitOfWork.GetByKey<WFDefInst>(wFDefInstId);
            wfDefInst.CheckNotNull("wfDefInst");

            #region 创建流程实例
            WFInst wfInst = new WFInst();
            wfInst.Id = GuidHelper.CreateTimeOrderID();
            wfInst.WFDefInstId = wFDefInstId;

            wfInst.FormInstId = dic.GetValue("Id");
            wfInst.FlowState = FlowState.Create.ToString();
            UnitOfWork.Add<WFInst>(wfInst);
            #endregion

            #region 增加流程实例开始步骤
            WFStep step = new WFStep();
            step.Id = GuidHelper.CreateTimeOrderID();
            step.WFInstId = wfInst.Id;
            var startNodeDefInst = wfDefInst.GetStartNode();
            startNodeDefInst.CheckNotNull("startNodeDefInst");
            step.WFNodeDefInstId = startNodeDefInst.Id;
            UnitOfWork.Add<WFStep>(step);
            #endregion

            #region 下一步骤
            string nextRoutingDefInstId = QueryString("nextRoutingDefInstId");
            if (!string.IsNullOrEmpty(nextRoutingDefInstId))
            {
                GoToNextStep(nextRoutingDefInstId, step, dic);
            }
            #endregion
        }

        //更新
        protected override void BeforeUpdate(Dictionary<string, object> dic)
        {
            dic.SetValue("FlowPhase", FlowState.Process.ToString());
            //如果仅仅是保存表单
            if (!string.IsNullOrEmpty(QueryString("JustSave")) && QueryString("JustSave").ToLower() == "true")
            {
                return;
            }

            #region 下一步骤
            string currentStepId = QueryString("currentStepId");
            currentStepId.CheckNotNullOrEmpty("currentStepId");
            var currentStep = UnitOfWork.GetByKey<WFStep>(currentStepId);
            currentStep.CheckNotNull("currentStep");
            if (currentStep.WFNodeDefInst.WFNodeType == WFNodeType.End.ToString())
            {
                throw new BusinessException("该流程已经结束");
            }
            else if (!string.IsNullOrEmpty(currentStep.NextStepId))
            {
                throw new BusinessException("该任务已经执行过了");
            }
            string nextRoutingDefInstId = QueryString("nextRoutingDefInstId");
            nextRoutingDefInstId.CheckNotNullOrEmpty("nextRoutingDefInstId");
            GoToNextStep(nextRoutingDefInstId, currentStep, dic);
            #endregion
        }

        public void GoToNextStep(string nextRoutingDefInstId, WFStep currentStep, Dictionary<string, object> entityDic)
        {
            var nextRoutingDefInst = UnitOfWork.GetByKey<WFRoutingDefInst>(nextRoutingDefInstId);
            nextRoutingDefInst.CheckNotNull("id为{0}的WFRoutingDefInst不存在".ReplaceArg(nextRoutingDefInstId));

            WFStep step = new WFStep();
            step.Id = GuidHelper.CreateTimeOrderID();
            step.PreStepId = currentStep.Id;
            step.WFNodeDefInstId = nextRoutingDefInst.ENodeDefInstId;
            step.WFInstId = currentStep.WFInstId;

            //如果下一节点不是结束节点,必须给执行人
            var nextNodeDefInst = UnitOfWork.GetByKey<WFNodeDefInst>(nextRoutingDefInst.ENodeDefInstId);
            nextNodeDefInst.CheckNotNull("id为{0}的WFNodeDefInst不存在".ReplaceArg(nextRoutingDefInst.ENodeDefInstId));
            if (nextNodeDefInst.WFNodeType != WFNodeType.End.ToString())
            {
                string nextUserId = QueryString("nextUserId");
                nextUserId.CheckNotNullOrEmpty("nextUserId");
                string nextUserName = QueryString("nextUserName");
                nextUserName.CheckNotNullOrEmpty("nextUserName");
                step.StepUserId = nextUserId;
                step.StepUserName = nextUserName;
                step.Name = nextNodeDefInst.Name;
            }
            //结束
            else
            {
                entityDic.SetValue("FlowPhase", FlowState.End.ToString());
                OnFlowEnd(entityDic);
            }

            currentStep.NextStepId = step.Id;//
            currentStep.OperateUserId = GetCurrentUserID();
            currentStep.OperateUserName = GetCurrentUserName();
            currentStep.OperateTime = DateTime.Now;
            UnitOfWork.Add<WFStep>(step);
        }

        public JsonResult GetTraceList()
        {
            string formInstId = QueryString("FormInstId");
            formInstId.CheckNotNullOrEmpty("表实例id不能为空");
            var wfInst = UnitOfWork.GetSingle<WFInst>(a => a.FormInstId == formInstId);
            wfInst.CheckNotNull("FormInstId为【" + formInstId + "】流程实例为空");
            var res = wfInst.WFStep.Select(a => new
            {
                Name = a.Name,
                Receiver = a.StepUserName,
                ReceiveTime = a.CreateTime,
                Operator = a.OperateUserName,
                OperateTime = a.OperateTime,
                OperateTimeSpan = a.OperateTime == null ? "" :
                string.Format("{0}天,{1}小时,{2}分",
                (a.OperateTime.Value - a.CreateTime).Days,
                (a.OperateTime.Value - a.CreateTime).Hours,
                (a.OperateTime.Value - a.CreateTime).Minutes)
            });
            return Json(res);
        }

        public JsonResult GetStepGraph()
        {
            string formInstId = QueryString("FormInstId");
            formInstId.CheckNotNullOrEmpty("表实例id不能为空");
            var wfInst = UnitOfWork.GetSingle<WFInst>(a => a.FormInstId == formInstId);
            wfInst.CheckNotNull("FormInstId为【" + formInstId + "】流程实例为空");
            var stepList = wfInst.WFStep;
            var nodeDefList = wfInst.WDefInst.WFNodeDefInst;
            var routingList = wfInst.WDefInst.WFRoutingDefInst;

            List<Dictionary<string, object>> nodeDefDicList = new List<Dictionary<string, object>>();
            var currentStep = wfInst.GetCurrentStep();
            if (currentStep != null)
            {
                foreach (var nodeDef in nodeDefList)
                {
                    var dic = nodeDef.ToDictionary();
                    if (currentStep.WFNodeDefInstId == nodeDef.Id)
                    {
                        dic.SetValue("IsCurrent", true);
                    }
                    nodeDefDicList.Add(dic);
                }
            }

            List<Dictionary<string, object>> rountingDicList = new List<Dictionary<string, object>>();
            foreach (var routing in routingList)
            {
                var dic = routing.ToDictionary();
                dic.SetValue("IsFinish", false);
                if (stepList.Any(a => a.WFNodeDefInstId == routing.ENodeDefInstId))
                {
                    dic.SetValue("IsFinish", true);
                }
                rountingDicList.Add(dic);
            }
            return Json(new { Nodes = nodeDefDicList, Routings = rountingDicList });
        }

        public JsonResult GetNextExcuteUsers(string routingDefInstId)
        {
            var routingDefInst = UnitOfWork.GetByKey<WFRoutingDefInst>(routingDefInstId);
            routingDefInst.CheckNotNull("无法找到Id为{0}的路由实例".ReplaceArg(routingDefInstId));
            var resDicList = new List<Dictionary<string, object>>();
            //NextExcuteUserId
            if (!string.IsNullOrEmpty(routingDefInst.NextExcuteUserId))
            {
                var idArr = routingDefInst.NextExcuteUserId.Split(',');
                var nameArr = routingDefInst.NextExcuteUserName.Split(',');
                for (int i = 0; i < idArr.Length; i++)
                {
                    var dic = new Dictionary<string, object>();
                    dic.SetValue("Id", idArr[i]);
                    dic.SetValue("Name", nameArr[i]);
                    resDicList.Add(dic);
                }               
            }
            //NextExcuteUserRoleId
            var roleUsers = UnitOfWork.Get<MF_RoleUser>(a => routingDefInst.NextExcuteUserRoleId.Contains(a.MF_RoleId), "MF_User");
            foreach (var roleUser in roleUsers)
            {
                var dic = new Dictionary<string, object>();
                dic.SetValue("Id", roleUser.MF_UserId);
                dic.SetValue("Name", roleUser.MF_User.RealName);
                resDicList.Add(dic);
            }
            //NextExcuteUserSQLSourceId
            var sqlDataSource = UnitOfWork.GetByKey<SQLDataSource>(routingDefInst.NextExcuteUserSQLSourceId);
            if (sqlDataSource != null)
            {
                DataTable dt = sqlDataSource.GetDataTable();
                if (dt.Columns.Contains(routingDefInst.NextExcuteUserSQLSourceResKeyField)
                    && dt.Columns.Contains(routingDefInst.NextExcuteUserSQLSourceResValueField))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var dic = new Dictionary<string, object>();
                        dic.SetValue("Id", dr[routingDefInst.NextExcuteUserSQLSourceResKeyField]);
                        dic.SetValue("Name", dr[routingDefInst.NextExcuteUserSQLSourceResValueField]);
                        resDicList.Add(dic);
                    }
                }
            }
            var distinctDicList = resDicList.Distinct();
            return Json(new
            {
                Id = string.Join(",", distinctDicList.Select(a => a.GetValue("Id"))),
                Name = string.Join(",", distinctDicList.Select(a => a.GetValue("Name")))
            });
        }

        private IEnumerable<WFNodeDefInst> GetNextNodeDefInstsByStepId(string stepId)
        {
            var step = UnitOfWork.GetByKey<WFStep>(stepId);
            step.CheckNotNull("当前步骤为空");
            var res = UnitOfWork.Get<WFRoutingDefInst>(a => a.SNodeDefInstId == step.WFNodeDefInstId)
                .Select(a => a.E_WFNodeDefInst);
            return res;
        }

        private IEnumerable<WFRoutingDefInst> GetNextRoutingDefInstsByStepId(string stepId)
        {
            var step = UnitOfWork.GetByKey<WFStep>(stepId);
            step.CheckNotNull("当前步骤为空");
            var res = UnitOfWork.Get<WFRoutingDefInst>(a => a.SNodeDefInstId == step.WFNodeDefInstId);
            return res;
        }

        private IEnumerable<WFNodeDefInst> GetNextNodeDefInstsByNodeDefInstId(string wfNodeDefInstId)
        {
            var res = UnitOfWork.Get<WFRoutingDefInst>(a => a.SNodeDefInstId == wfNodeDefInstId)
                .Select(a => a.E_WFNodeDefInst);
            return res;
        }

        private IEnumerable<WFRoutingDefInst> GetNextRoutingDefInstsByNodeDefInstId(string wfNodeDefInstId)
        {
            var res = UnitOfWork.Get<WFRoutingDefInst>(a => a.SNodeDefInstId == wfNodeDefInstId);
            return res;
        }
    }
}
