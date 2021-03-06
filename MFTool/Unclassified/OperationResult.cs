﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;

namespace MFTool
{
    /// <summary>
    /// 业务操作结果信息类，对操作结果进行封装
    /// </summary>
    public class OperaResult
    {
        #region 构造函数
        /// <summary>
        ///     初始化一个 业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        public OperaResult(OperationResultType resultType)
        {
            ResultType = resultType;
        }

        public OperaResult(bool bSuccess, object appendData = null)
        {
            ResultType = bSuccess ? OperationResultType.Success : OperationResultType.Error;
            AppendData = appendData; 
        }

        /// <summary>
        ///     初始化一个 定义返回消息的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        public OperaResult(OperationResultType resultType, string message)
            : this(resultType)
        {
            //if (resultType != OperationResultType.Success)
            {
                //ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                //switch (resultType)
                //{
                //    case OperationResultType.Error:
                //        log.Error()
                //}
            }
            Message = message;
        }

        /// <summary>
        ///     初始化一个 定义返回消息与附加数据的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        /// <param name="appendData">业务返回数据</param>
        public OperaResult(OperationResultType resultType, string message, object appendData)
            : this(resultType, message)
        {
            AppendData = appendData;
        }

        /// <summary>
        ///     初始化一个 定义返回消息与日志消息的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        /// <param name="logMessage">业务日志记录消息</param>
        public OperaResult(OperationResultType resultType, string message, string logMessage)
            : this(resultType, message)
        {
            LogMessage = logMessage;
        }

        /// <summary>
        ///     初始化一个 定义返回消息、日志消息与附加数据的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        /// <param name="logMessage">业务日志记录消息</param>
        /// <param name="appendData">业务返回数据</param>
        public OperaResult(OperationResultType resultType, string message, string logMessage, object appendData)
            : this(resultType, message, logMessage)
        {
            AppendData = appendData;
        }

        #endregion

        #region 属性

        /// <summary>
        ///     获取或设置 操作结果类型
        /// </summary>
        public OperationResultType ResultType { get; set; }

        private string _message;
        /// <summary>
        ///     获取或设置 操作返回信息
        /// </summary>
        public string Message 
        {
            get 
            {
                if (string.IsNullOrEmpty(_message))
                {
                    return ResultType.ToDescription();
                }
                return _message;
            }
            set 
            {
                _message = value;
            }
        }

        /// <summary>
        ///     获取或设置 操作返回的日志消息，用于记录日志
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        ///     获取或设置 操作结果附加信息
        /// </summary>
        public object AppendData { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 得到Json格式的回调值
        /// 格式{Success:'true',Message:'message'}
        /// </summary>
        /// <returns>格式{Success:'true',Message:'message'}</returns>
        public JsonMess GetJsonMess()
        {
            bool bSuccess = ResultType == OperationResultType.Success || ResultType == OperationResultType.Warning;
            string mess = string.IsNullOrEmpty(Message) ? ResultType.ToDescription() : Message;
            return new JsonMess(bSuccess, mess, AppendData);
        }
        #endregion
    }
}