using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.Composition;
using System.ComponentModel;
using MFTool;
using Newtonsoft.Json;

namespace MF_Base.Model
{
    [Description("部门")]
    public class MF_Department : Entity
    {
        public MF_Department()
        {
           
        }

        [ForeignKey("Parent")]
        public string ParentId { get; set; }
        public string FullId { get; set; }
        public string Code { get; set; }
        public String Name { get; set; }
        public String Remark { get; set; }
        [JsonIgnore]
        public virtual MF_Department Parent { get; set; }
        [JsonIgnore]
        [ForeignKey("ParentId")]
        public virtual List<MF_Department> Children { get; set; }
    }
}
