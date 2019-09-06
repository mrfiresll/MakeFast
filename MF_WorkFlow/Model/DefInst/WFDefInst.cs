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
    [Description("流程定义实例")]
    public class WFDefInst : Entity
	{
        public WFDefInst()
        {

        }

        public string WFDefId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DBName { get; set; }
        public string EntityFullName { get; set; }
        public WFDef WFDef { get; set; }

        [JsonIgnore]
        public virtual List<WFNodeDefInst> WFNodeDefInst { get; set; }
        [JsonIgnore]
        public virtual List<WFRoutingDefInst> WFRoutingDefInst { get; set; }

        public WFNodeDefInst GetStartNode()
        {
            //if (WFNodeDefInst.Count(a => a.WFNodeType == WFNodeType.Start.ToString()) != 1)
            //{
            //    Logger.GetCurMethodLog().Debug("流程id" + Id + ",开始节点不为1");
            //}
            return WFNodeDefInst.FirstOrDefault(a => a.WFNodeType == WFNodeType.Start.ToString());
        }

        //public WFNodeDefInst GetEndNode()
        //{
        //    //if (WFNodeDefInst.Count(a => a.WFNodeType == WFNodeType.Start.ToString()) != 1)
        //    //{
        //    //    Logger.GetCurMethodLog().Debug("流程id" + Id + ",开始节点不为1");
        //    //}
        //    return WFNodeDefInst.FirstOrDefault(a => a.WFNodeType == WFNodeType.End.ToString());
        //}
	}    
}

namespace MF_WorkFlow
{
    public partial class WFDefInstConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFDefInst>
    {
        public WFDefInstConfiguration()
        {
            this.HasRequired(a => a.WFDef).WithMany().HasForeignKey(a => a.WFDefId);
        }
    }
}
