using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    public class ConvertHelper
    {
        public static T ConvertToObj<T>(IDictionary<string, object> dic, bool ignoreCase = true)
        {
            T model = Activator.CreateInstance<T>();
            PropertyInfo[] modelPro = model.GetType().GetProperties();
            if (modelPro.Length > 0 && dic.Count() > 0)
            {
                for (int i = 0; i < modelPro.Length; i++)
                {
                    var findKeyValuePair = dic.SingleOrDefault(a => a.Key.ToLower() == modelPro[i].Name.ToLower());

                    if (modelPro[i].CanWrite && !default(KeyValuePair<string, object>).Equals(findKeyValuePair))
                    {
                        var value = findKeyValuePair.Value;

                        if (modelPro[i].PropertyType.BaseType == typeof(Enum))
                        {
                            value = Enum.Parse(modelPro[i].PropertyType, findKeyValuePair.Value.ToString(), true);
                        }
                        else if (modelPro[i].PropertyType == typeof(byte[]))
                        {
                            value = System.Text.Encoding.Default.GetBytes(findKeyValuePair.Value.ToString());
                        }
                        else if (modelPro[i].PropertyType == typeof(Int32))
                        {
                            value = Convert.ToInt32(findKeyValuePair.Value);
                        }
                        modelPro[i].SetValue(model, value, null);
                    }
                }
            }
            return model;
        }

        public static object UpdateEntity(object obj, Dictionary<string, object> dic, bool ignoreId = true, params string[] otherIgnoreFields)
        {
            try
            {
                bool ignoreNullValue = false;
                foreach (string key in dic.Keys)
                {
                    if (otherIgnoreFields.Contains(key)) continue;
                    PropertyInfo pi = obj.GetType().GetProperty(key);

                    if (key.ToLower() == "id")
                    {
                        if (ignoreId) continue;
                        //ID字段在基类上
                        pi = obj.GetType().BaseType.GetProperty(key);
                    }                    

                    if (pi == null || pi.CanWrite == false)
                        continue;
                    if (ignoreNullValue)
                    {
                        if (dic[key] == null || string.IsNullOrEmpty(dic.GetValue(key)))
                            continue;
                    }
                    if (pi.PropertyType.FullName == "System.String")
                    {
                        //为兼容Oracle，不能使用bool型，因此使用char(1)
                        string value = "";
                        if (dic[key] != null)
                        {
                            value = dic[key].ToString().Trim();
                            if (value == "true")
                                value = "1";
                            else if (value == "false")
                                value = "0";

                            pi.SetValue(obj, value, null);
                        }
                        else
                        {
                            pi.SetValue(obj, null, null);
                        }
                    }
                    else if (dic[key] == null || dic[key].ToString() == "")
                    {
                        pi.SetValue(obj, null, null);
                    }
                    else if (pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(Nullable<bool>))
                    {
                        string value = dic[key].ToString();
                        if (value.ToLower() == "true" || value == "1")
                            pi.SetValue(obj, true, null);
                        else
                            pi.SetValue(obj, false, null);
                    }
                    else if (pi.PropertyType.IsValueType)
                    {
                        Object value = null;
                        Type t = System.Nullable.GetUnderlyingType(pi.PropertyType);
                        if (t == null)
                            t = pi.PropertyType;
                        MethodInfo mis = t.GetMethod("Parse", new Type[] { typeof(string) });
                        try
                        {
                            value = mis.Invoke(null, new object[] { dic[key].ToString() });
                        }
                        catch
                        {
                            throw new Exception(string.Format("数据类型转换失败:将‘{0}’转换为{1}类型时.", dic[key].ToString(), t.Name));
                        }
                        pi.SetValue(obj, value, null);
                    }

                }
                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
