using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MFTool
{
    /// <summary>
    /// 自定义布尔类型的转换
    /// </summary>
    public class JsonCustomBoolConvert : CustomCreationConverter<bool>
    {
        public JsonCustomBoolConvert()
        {

        }        

        /// <summary>
        /// 重载是否可写
        /// </summary>
        public override bool CanWrite { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override bool Create(Type objectType)
        {
            return false;
        }

        /// <summary>
        /// 重载序列化方法
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value);
            }

        }
    }
}