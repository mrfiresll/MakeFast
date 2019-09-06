using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel;

namespace MF_WorkFlow.Model
{
    [Description("节点定义实例")]
    public class WFNodeDefInst : Entity
	{
        public string WFDefInstId { get; set; }
        public string Name { get; set; }
        public bool CanSave { get; set; }
        public string WFNodeType { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }

        public WFDefInst WFDefInst { get; set; }
	}
}

namespace MF_WorkFlow
{
    public partial class WFNodeDefInstConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFNodeDefInst>
    {
        public WFNodeDefInstConfiguration()
        {
            this.HasRequired(a => a.WFDefInst).WithMany(a => a.WFNodeDefInst).HasForeignKey(a => a.WFDefInstId);
        }
    }
}
