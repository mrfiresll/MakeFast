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
    [Description("节点定义")]
    public class WFNodeDef : Entity
	{
        public WFNodeDef()
        {

        }
        
        public string WFDefId { get; set; }
        public string Name { get; set; }
        public bool CanSave { get; set; }
        public string GraphId { get; set; }
        public string WFNodeType { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
        public WFDef WFDef { get; set; }
	}
}

namespace MF_WorkFlow
{
    public partial class WFNodeDefConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFNodeDef>
    {
        public WFNodeDefConfiguration()
        {
            this.HasRequired(a => a.WFDef).WithMany(a => a.WFNodeDef);
        }
    }
}