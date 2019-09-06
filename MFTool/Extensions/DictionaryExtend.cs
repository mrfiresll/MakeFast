using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Data;
using System.Runtime.Serialization;

namespace MFTool
{
    public static class DictionaryExtend
    {
        public static DataTable ConvertDicToTable(List<Dictionary<string, object>> dicList)
        {
            DataTable dt = new DataTable();
            if (dicList.Count == 0)
                return dt;

            foreach (var colName in dicList[0].Keys)
            {
                dt.Columns.Add(colName, typeof(string));
            }

            foreach (var dicDep in dicList)
            {
                DataRow dr = dt.NewRow();
                foreach (KeyValuePair<string, object> item in dicDep)
                {
                    if (dt.Columns.Contains(item.Key))
                        dr[item.Key] = item.Value;
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public static string GetValue(this Dictionary<string, object> dic, string key)
        {
            string result = string.Empty;
            if (!dic.ContainsKey(key)) return result;
            if (dic[key] == null || dic[key] == DBNull.Value) return result;
            result = dic[key].ToString();
            return result;
        }

        public static object GetObject(this Dictionary<string, object> dic, string key)
        {
            string result = string.Empty;
            if (!dic.ContainsKey(key)) return null;
            if (dic[key] == null || dic[key] == DBNull.Value) return null;
            return dic[key];
        }

        public static void SetValue(this Dictionary<string, object> dic, string key, string value)
        {
            dic[key] = value;
        }

        public static void SetValue(this Dictionary<string, object> dic, string key, object value)
        {
            dic[key] = value;
        }

        //泛型操作
        public static T GetValue<T>(this Dictionary<string, T> dic, string key)
        {
            T result = default(T);
            if (!dic.ContainsKey(key)) return result;
            result = dic[key];
            return result;
        }

        public static void SetValue<T>(this Dictionary<string, T> dic, string key, T value)
        {
            if (!dic.ContainsKey(key))
                dic.Add(key, value);
            else
                dic[key] = value;
        }

        /// <summary>
        /// 用字典填充对象 
        /// 包含try语句
        /// 如果数据匹配问题较多，可能造成性能问题
        /// </summary>
        /// <returns></returns>
        public static T ToModel<T>(this Dictionary<string, object> dic) where T : class,new()
        {
            T result = new T();

            PropertyInfo[] arrPtys = typeof(T).GetProperties();
            foreach (PropertyInfo destPty in arrPtys)
            {
                if (!dic.ContainsKey(destPty.Name))
                    continue;
                if (destPty.CanRead == false)
                    continue;
                try
                {
                    destPty.SetValue(result, dic.GetValue<object>(destPty.Name), null);
                }
                catch { }
            }
            return result;
        }

        static bool IsNullOrEmpty(object value)
        {
            if (value == null || value == DBNull.Value)
                return true;
            if (string.IsNullOrEmpty(value.ToString()))
                return true;
            return false;
        }
    }
}
