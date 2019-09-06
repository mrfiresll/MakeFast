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
using System.ComponentModel.DataAnnotations;

namespace MF_Base.Model
{
    [Description("角色")]
    public class MF_Role : Entity
    {
        public MF_Role()
        {
           
        }
        public string Code { get; set; }
        public String Name { get; set; }
        [Required]
        public string EnumRoleType { get; set; }
        public String Remark { get; set; }
    }
}
