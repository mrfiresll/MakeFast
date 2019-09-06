using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MFTool
{
    /// <summary>
    ///     通用类型扩展方法类
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        ///     把对象类型转化为指定类型，转化失败时返回该类型默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败返回类型的默认值 </returns>
        public static T CastTo<T>(this object value)
        {
            object result;
            Type type = typeof(T);
            try
            {
                if (type.IsEnum)
                {
                    result = Enum.Parse(type, value.ToString());
                }
                else if (type == typeof(Guid))
                {
                    result = Guid.Parse(value.ToString());
                }
                else
                {
                    result = Convert.ChangeType(value, type);
                }
            }
            catch
            {
                result = default(T);
            }

            return (T)result;
        }

        public static Dictionary<string, object> ToDictionary<T>(this T obj)
        {
            obj.Required(a => a.GetType() != typeof(ICollection), "不能是集合类型");
            if (obj.GetType() == typeof(NameValueCollection) || obj.GetType().BaseType == typeof(NameValueCollection))
            {
                var nvc = obj as NameValueCollection;
                return nvc.NameValueCollectToDictionary();
            }
            else
            {
                try
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    PropertyInfo[] arrPtys = typeof(T).GetProperties();
                    foreach (PropertyInfo destPty in arrPtys)
                    {
                        if (destPty.CanRead == false)
                            continue;
                        //if (destPty.PropertyType.Name == "ICollection`1")
                        //    continue;
                        //if ((destPty.PropertyType.IsClass && destPty.PropertyType.Name != "String") || destPty.PropertyType.IsArray || destPty.PropertyType.IsInterface)
                        //    continue;
                        object value = destPty.GetValue(obj, null);
                        dic.Add(destPty.Name, value);
                    }
                    return dic;
                }
                catch (Exception ex)
                {
                    throw new Exception("取数据对象时发生不能匹配的错误！", ex);
                }
            }
        }

        /// <summary>
        ///     把对象类型转化为指定类型，转化失败时返回该类型默认值
        /// </summary>
        /// <param name="value">要转换为Json的字符串</param>
        /// <param name="bBigCamelCase">首字母是否大写</param>
        /// <returns> json</returns>
        public static string ToJson(this object value, bool bBigCamelCase = true)
        {
            string json = "";
            if (bBigCamelCase)
            {
                json = JsonConvert.SerializeObject(value);
            }
            else
            {
                json = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
            return json;
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回该类型默认值,有循环引用时可调用这个函数
        /// </summary>
        /// <param name="value">要转换为Json的字符串</param>
        /// <param name="bBigCamelCase">首字母是否大写</param>
        /// <returns>json</returns>
        public static string ToJsonIgnoreLoop(this object value, bool bBigCamelCase = true)
        {
            string json = "";
            if (bBigCamelCase)
            {
                json = JsonConvert.SerializeObject(value, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            else
            { 
                json = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            return json;
        }
        /// <summary>
        ///     把对象类型转化为指定类型，转化失败时返回该类型默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败返回类型的默认值 </returns>
        public static T JsonToObject<T>(this string value,bool ignoreNull = true)
        {
            T t = default(T);
            try
            {
                if (ignoreNull)
                {
                    t = JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore });
                }
                else
                {
                    t = JsonConvert.DeserializeObject<T>(value);
                }                
            }
            catch (Exception e)
            {
                throw new Exception("Json值:" + value + "转换失败", e);
            }

            return t;
        }

        /// <summary>
        /// 返回 Diction<string,object>
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Dictionary<string, object> JsonToDictionary(this string json)
        {
            if (string.IsNullOrEmpty(json)) return new Dictionary<string, object>();
            return json.JsonToObject<Dictionary<string, object>>(false);
        }

        /// <summary>
        /// 返回 List<Diction<string,object>>
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> JsonToDictionaryList(this string json)
        {
            if (string.IsNullOrEmpty(json)) return new List<Dictionary<string, object>>();
            return json.JsonToObject<List<Dictionary<string, object>>>(false);
        }

        /// <summary>
        ///     把对象类型转化为指定类型，转化失败时返回指定的默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="defaultValue"> 转化失败返回的指定默认值 </param>
        /// <returns> 转化后的指定类型对象，转化失败时返回指定的默认值 </returns>
        public static T CastTo<T>(this object value, T defaultValue)
        {
            object result;
            Type type = typeof(T);
            try
            {
                result = type.IsEnum ? Enum.Parse(type, value.ToString()) : Convert.ChangeType(value, type);
            }
            catch
            {
                result = defaultValue;
            }
             
            return (T)result;
        }  

        //TODO 无限递归转Dictionary
        ///// <summary>
        ///// 普通类字段名转换成首字母小写的字典
        ///// 用于 MVC 的 new JsonResult(){Data=xxx} 后前端Javascript使用
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static dynamic ToFirstCharLowerJsonResultData<T>(this object obj)
        //{
        //    var objJson = obj.ToJson();
        //    if (objJson.Length > 0 && objJson[0] == '[' && objJson[objJson.Length - 1] == ']')
        //    {
        //        List<Dictionary<string, object>> tmpList = objJson.JsonToObject<List<Dictionary<string, object>>>();
        //        foreach (var tmp in tmpList)
        //        {
        //            foreach (var child in tmp)
        //            {
        //                if(child.ToString())
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return objJson.JsonToDictionary();
        //    }
        //}
    }
}