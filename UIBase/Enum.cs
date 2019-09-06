using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIBase
{
    public enum FlowState
    {
        [Description("创建")]
        Create = 1,
        [Description("过程")]
        Process = 2,
        [Description("结束")]
        End
    }
}
