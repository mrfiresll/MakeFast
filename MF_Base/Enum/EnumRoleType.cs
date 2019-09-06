using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MF_Base
{
    public enum EnumRoleType
    {
        [Description("系统角色")]
        Sys = 1,
        [Description("组织角色")]
        Org =2,
    }
}
