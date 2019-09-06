using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel;
using MFTool;

namespace MF_WorkFlow.Model
{
    [Description("流程定义")]
    public class WFDef : Entity
	{
        public WFDef()
        {

        }

        public string MainTypeFullId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DBName { get; set; }
        public string EntityFullName { get; set; }

        public string IsPublish { get; set; }
        [ForeignKey("WFDefId")]
        public virtual List<WFNodeDef> WFNodeDef { get; set; }
        [ForeignKey("WFDefId")]
        public virtual List<WFRoutingDef> WFRoutingDef { get; set; }

        public string EntityName
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityFullName))
                {
                    int index = EntityFullName.LastIndexOf('.');
                    if (index == -1) index = 0;
                    return EntityFullName.Substring(index + 1);
                }
                return "";
            }
        }

        public WFDefInst CreateInst()
        {
            WFDefInst defInst = new WFDefInst();
            var dic = this.ToDictionary();
            ConvertHelper.UpdateEntity(defInst, dic);
            defInst.Id = GuidHelper.CreateTimeOrderID();
            defInst.WFDefId = this.Id;
            defInst.WFNodeDefInst = new List<WFNodeDefInst>();
            defInst.WFRoutingDefInst = new List<WFRoutingDefInst>();

            foreach (var node in WFNodeDef)
            {
                WFNodeDefInst nDefInst = new WFNodeDefInst();
                var tmp = node.ToDictionary();
                ConvertHelper.UpdateEntity(nDefInst, tmp, false);
                nDefInst.WFDefInstId = defInst.Id; //不需要，否则反而加不进去
                nDefInst.WFDefInst = defInst;// 不需要，否则反而加不进去
                defInst.WFNodeDefInst.Add(nDefInst);
            }

            foreach (var routine in WFRoutingDef)
            {
                WFRoutingDefInst rDefInst = new WFRoutingDefInst();
                var tmp = routine.ToDictionary();
                ConvertHelper.UpdateEntity(rDefInst, tmp, false);
                rDefInst.SNodeDefInstId = routine.SNodeDefId;
                rDefInst.ENodeDefInstId = routine.ENodeDefId;
                rDefInst.WFDefInstId = defInst.Id;
                rDefInst.WFDefInst = defInst;
                defInst.WFRoutingDefInst.Add(rDefInst);
            }

            //id处理，避免多个WFNodeDefInst都有同一个id造成主键冲突
            foreach(var node in defInst.WFNodeDefInst)
            {
                string nGuid = GuidHelper.CreateTimeOrderID();
                defInst.WFRoutingDefInst.Where(a => a.SNodeDefInstId == node.Id)
                    .ToList().ForEach(a => a.SNodeDefInstId = nGuid);
                defInst.WFRoutingDefInst.Where(a => a.ENodeDefInstId == node.Id)
                    .ToList().ForEach(a => a.ENodeDefInstId = nGuid);
                defInst.WFRoutingDefInst.ForEach(a => a.Id = a.SNodeDefInstId + "_" + a.ENodeDefInstId);
                node.Id = nGuid;
            }

            return defInst;
        }
	}
}
