using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    /// <summary>
    /// 业务逻辑的异常
    /// </summary>
    public class BusinessException : Exception
    {
        public static void Throw(string message)
        {
            BusinessException excep = new BusinessException(message);
            throw excep;
        }

        #region 构造函数
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public BusinessException()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        public BusinessException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常的消息</param>
        /// <param name="inner">内部的异常</param>
        public BusinessException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info">存有有关所引发异常的序列化的对象数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        public BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        #endregion
    }
}
