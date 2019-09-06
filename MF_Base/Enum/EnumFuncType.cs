using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MF_Base
{
    public enum EnumFuncType
    {
        [Description("模块级")]
        Module = 1,
        [Description("菜单级")]
        Menu =2,
        [Description("页面级")]
        Page,
        [Description("按钮级")]
        Button,
    }
}
