using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MFTool
{
    public class JsonMess
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public object Data { get; set; }

        public JsonMess(bool Success, string Message, object Data = null)
        {
            this.Success = Success;
            this.Message = Message;
            this.Data = Data;
        }
    }
}