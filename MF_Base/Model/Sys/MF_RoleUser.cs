using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace MF_Base.Model
{
    [Description("角色用户对应")]
    public class MF_RoleUser : Entity
    {
        [ForeignKey("MF_User")]
        public string MF_UserId { get; set; }
        [ForeignKey("MF_Role")]
        public string MF_RoleId { get; set; }

        public MF_User MF_User { get; set; }
        public MF_Role MF_Role { get; set; }
    }
}
