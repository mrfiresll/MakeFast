using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    public class MessageLog
    {
        /// <summary>
        /// 操作人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string Param { get; set; }
        /// <summary>
        /// 操作结果
        /// </summary>
        public string Result { get; set; }
    }
}
