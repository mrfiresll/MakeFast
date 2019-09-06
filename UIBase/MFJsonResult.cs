using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MFTool;
using System.Text;

namespace UIBase
{
    /// <summary>
    /// JsonResult,驼峰式转换
    /// </summary>
    public class MFJsonResult : JsonResult
    {
        public bool bBigCamelCase { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        public MFJsonResult() { }

        /// <summary>
        /// 构造器
        /// </summary>
        public MFJsonResult(object data, bool bBigCamelCase = false)
        {
            this.Data = data;
            this.bBigCamelCase = bBigCamelCase;
        }

        /// <summary>
        /// 重写执行结果
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            
            var json = this.Data == null ? "" : this.Data.ToJsonIgnoreLoop(bBigCamelCase);           
            response.Write(json);
        }
    }
}