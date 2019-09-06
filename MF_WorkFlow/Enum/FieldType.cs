using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF_WorkFlow
{
    public enum FieldType
    {
        [Description("Int")]
        Int=1,
        [Description("Float")]
        Float,
        [Description("Varchar(40)")]
        Varchar_40,
        [Description("DateTime")]
        DateTime
    }
}
