using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MF_WorkFlow
{
    public enum WFNodeType
    {
        [Description("开始")]
        Start = 1,
        [Description("过程")]
        Process =2,
        [Description("结束")]
        End
    }
}
