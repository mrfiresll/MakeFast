using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    public class MessageLayout : PatternLayout
    {
        public MessageLayout()
        {
            this.AddConverter("name", typeof(NamePatternConverter));
            this.AddConverter("content", typeof(ContentPatternConverter));
            this.AddConverter("param", typeof(ParamPatternConverter));
            this.AddConverter("result", typeof(ResultPatternConverter));
        }

        public MessageLayout(string p)
            :base(p)
        {
            
        }
    }
}
