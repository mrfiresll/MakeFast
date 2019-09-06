using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF_Base.Model
{
    [Description("功能菜单与角色映射")]
    public class MF_FuncRole : Entity
    {
        public string MF_FuncId { get; set; }
        [ForeignKey("MF_Role")]
        public string MF_RoleId { get; set; }

        public MF_Func MF_Func { get; set; }
        
        public MF_Role MF_Role { get; set; }
    }    
}
