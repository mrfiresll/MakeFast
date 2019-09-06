using MF_WorkFlow;
using MF_WorkFlow.Model;
using MFTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using UIBase;

namespace WorkFlow.Controllers
{
    [Export]
    public class WFDefController : BaseController
    {
        public ActionResult FlowGraph()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public JsonResult AddWorkFlow()
        {
            var dataStr = QueryString("data");
            WFDef wfd = dataStr.JsonToObject<WFDef>();
            wfd.Id = GuidHelper.CreateTimeOrderID();
            wfd.IsPublish = "false";
            wfd.Code = wfd.EntityFullName.Replace(".", "_");//xx_xxx_xx
            UnitOfWork.Add(wfd);

            WFDefInst defInst = new WFDefInst();            
            ConvertHelper.UpdateEntity(defInst, dataStr.JsonToDictionary());
            defInst.Id = GuidHelper.CreateTimeOrderID();
            defInst.WFDefId = wfd.Id;            
            UnitOfWork.Add(defInst);

            bool b = UnitOfWork.Commit();
            return Json(b);
        }

        public JsonResult GetWorkFlowDef()
        {
            string id = QueryString("id");
            id.CheckNotNullOrEmpty("id");
            WFDef cForm = UnitOfWork.GetByKey<WFDef>(id);
            cForm.CheckNotNull("WFDef");
            return Json(cForm);
        }

        public JsonResult GetEntityList()
        {
            string entityFullName = QueryString("EntityFullName").ToLower();
            var dbNames = WebConfigHelper.GetDBNames();
            List<dynamic> list = new List<dynamic>();
            //根据entityfullname筛选，同时排除已经定义的实体
            var exsitEntityFullNames = UnitOfWork.Get<WFDef>().Select(a => a.EntityFullName);

            foreach (var name in dbNames)
            {
                string baseEntityName = "Entity";
                list.AddRange(CommonHelper.GetClassFullNames(name, baseEntityName)
                    .Where(a => (string.IsNullOrEmpty(entityFullName) ? true : a.GetValue("FullName").ToLower().Contains(entityFullName)) && !exsitEntityFullNames.Contains(a.GetValue("FullName")))
                    .Select(a => new { DBName = name, EntityFullName = a.GetValue("FullName"), Name = a.GetValue("Description"), }));
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
            Expression<Func<WFDef, bool>> condition = null;
            if (!string.IsNullOrEmpty(mainType))
            {
                condition = a => a.MainTypeFullId.Contains(mainType);
            }
            else if (!string.IsNullOrEmpty(dbName))
            {
                condition = a => a.DBName == dbName;
            }

            IEnumerable<WFDef> baseForms = UnitOfWork.GetByPage<WFDef, string>(out totalCount, pageSize, pageId, a => a.Id, false, condition);
           // var res = baseForms.sel
            return Json(new { rows = baseForms, total = totalCount });
        }

        public JsonResult SaveWorkFlowDef()
        {
            var dic = Request.Form["formData"].JsonToObject<Dictionary<string, object>>();
            string id = dic.GetValue("Id");
            #region Def
            if (string.IsNullOrEmpty(id))
            {
                WFDef flowDef = ConvertHelper.ConvertToObj<WFDef>(dic);
                flowDef.Id = GuidHelper.CreateTimeOrderID();
                UnitOfWork.Add(flowDef);
            }
            else
            {
                WFDef flowDef = UnitOfWork.GetByKey<WFDef>(id);
                ConvertHelper.UpdateEntity(flowDef, dic);
                UnitOfWork.UpdateEntity(flowDef);
            }
            #endregion

            bool b = UnitOfWork.Commit();
            return Json(b);
        }

        public JsonResult SaveFlowDesign(string nodes, string routings)
        {
            #region 节点
            var nodeDicList = nodes.JsonToDictionaryList();
            var nodeIdList = nodeDicList.Select(a=>a.GetValue("Id"));
            foreach (var nodeDic in nodeDicList)
            {
                WFNodeDef nodeToAdd = UnitOfWork.GetByKey<WFNodeDef>(nodeDic.GetValue("Id"));
                if (nodeToAdd == null)
                {
                    nodeToAdd = new WFNodeDef();
                    ConvertHelper.UpdateEntity(nodeToAdd, nodeDic, false);
                    UnitOfWork.Add<WFNodeDef>(nodeToAdd);
                }
                else
                {
                    ConvertHelper.UpdateEntity(nodeToAdd, nodeDic, true);
                }
            }
            UnitOfWork.Delete<WFNodeDef>(a => !nodeIdList.Contains(a.Id));
            #endregion

            #region 路由
            var routingDicList = routings.JsonToDictionaryList();
            var routingIdList = routingDicList.Select(a => a.GetValue("Id"));
            foreach (var routingDic in routingDicList)
            {
                WFRoutingDef routingToAdd = UnitOfWork.GetByKey<WFRoutingDef>(routingDic.GetValue("Id"));
                if (routingToAdd == null)
                {
                    routingToAdd = new WFRoutingDef();
                    ConvertHelper.UpdateEntity(routingToAdd, routingDic, false);
                    UnitOfWork.Add<WFRoutingDef>(routingToAdd);
                }
                else
                {
                    ConvertHelper.UpdateEntity(routingToAdd, routingDic, true);
                }
            }
            UnitOfWork.Delete<WFRoutingDef>(a => !routingIdList.Contains(a.Id));
            #endregion

            bool b = UnitOfWork.Commit();
            return Json(b);
        }

        public JsonResult GetFlowDesign(string flowDefId)
        {
            var nodes = UnitOfWork.Get<WFNodeDef>(a => a.WFDefId == flowDefId);
            var routings = UnitOfWork.Get<WFRoutingDef>(a => a.WFDefId == flowDefId);
            return Json(new { Nodes = nodes, Routings = routings });
        }

        public JsonResult Publish()
        {
            string bPublish = QueryString("bPublish");
            bPublish.CheckBoolType("bPublish");
            string id = QueryString("id");
            id.CheckNotNullOrEmpty("id");
            WFDef wfDef = UnitOfWork.GetByKey<WFDef>(id);
            wfDef.CheckNotNull("WFDef");
            wfDef.IsPublish = bPublish;
            if (wfDef.IsPublish == "true")
            {                
                var defInst = wfDef.CreateInst();
                UnitOfWork.Add<WFDefInst>(defInst);
            }
            
            return Json(UnitOfWork.Commit());
        }

        public JsonResult Delete()
        {
            string list = QueryString("list");
            if(!string.IsNullOrEmpty(list))
            {
                var idList = list.JsonToDictionaryList().Select(a => a.GetValue("Id"));
                UnitOfWork.Delete<WFDef>(a => idList.Contains(a.Id));
                return Json(UnitOfWork.Commit());
            }
            
            return Json("");
        }
    }
}
