using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFTool;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MF_Base.Model
{
    [Description("配置的表单")]
    public class FormConfig : Entity
    {
        public string MainTypeFullId { get; set; }
        public string UICode { get; set; }
        public string Name { get; set; }
        public string DBName { get; set; }
        public string EntityFullName { get; set; }
        public string Remark { get; set; }

        public string CtrlSetting { get; set; }
        public string LayOutSetting { get; set; }

        public string EntityName 
        {
            get 
            {
                if (!string.IsNullOrEmpty(EntityFullName))
                {
                    int index = EntityFullName.LastIndexOf('.');
                    if (index == -1) index = 0;
                    return EntityFullName.Substring(index+1);
                }
                return "";
            }
        }
        public string EntityNameSpace 
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityFullName))
                {
                    int index = EntityFullName.LastIndexOf('.');
                    if (index == -1) index = 0;
                    return EntityFullName.Substring(0, index);
                }
                return "";
            }
        }

        public void UpdateCtrl(List<Dictionary<string, object>> dicList)
        {
            var oldDicList = CtrlSetting.JsonToDictionaryList();
            foreach (var dic in dicList)
            {
                if(dic.GetValue("state") == "insert")
                {
                    oldDicList.Add(dic);
                }
                else if (dic.GetValue("state") == "update")
                {
                    oldDicList.RemoveAll(a => a.GetValue("FieldName") == dic.GetValue("FieldName"));
                    oldDicList.Add(dic);
                }
                else if (dic.GetValue("state") == "delete")
                {
                    oldDicList.RemoveAll(a => a.GetValue("FieldName") == dic.GetValue("FieldName"));
                }
            }
            CtrlSetting = oldDicList.ToJson();
        }

        public void UpdateCtrl(Dictionary<string,object> dic)
        {
            var dicList = CtrlSetting.JsonToDictionaryList();
            var ctrlSet = dicList.SingleOrDefault(a => a.GetValue("FieldName") == dic.GetValue("FieldName"));
            ctrlSet.CheckNotNull("FieldName为" + dic.GetValue("FieldName") + "的控件设置不存在");
            dicList.Remove(ctrlSet);
            dicList.Add(dic);
            CtrlSetting = dicList.ToJson();
        }

        public void ReCreateCtrl()
        {
            Type type = ReflectionHelper.GetTypeBy(this.DBName, EntityFullName);
            var propertyDicList = GetPropertyDicList(type, true);
            var dicList = CtrlSetting.JsonToDictionaryList();

            List<string> toNoDelete = new List<string>();
            List<Dictionary<string, object>> toAdd = new List<Dictionary<string, object>>();
            foreach (var propertyDic in propertyDicList)
            {
                var dic = dicList.SingleOrDefault(a => a.GetValue("FieldName") == propertyDic.GetValue("FieldName"));
                if (dic == null)
                {
                    int maxOrderIndex = 0;
                    if (dicList.Count > 0)
                    {
                        maxOrderIndex = dicList.Max(a => Convert.ToInt32(
                           string.IsNullOrEmpty(a.GetValue("OrderIndex")) ? 0 : Convert.ToInt32(a.GetValue("OrderIndex"))));                       
                    }
                    //propertyDic.SetValue("CtrlType", "");
                    propertyDic.SetValue("IsVisible", "是");
                    propertyDic.SetValue("Enable", "是");
                    propertyDic.SetValue("Detail", "{ style_width: '100%'}");
                    propertyDic.SetValue("OrderIndex", maxOrderIndex + 1);
                    dicList.Add(propertyDic);
                }
                else
                {
                    dic.SetValue("FieldType", propertyDic.GetValue("FieldType"));
                }
                toNoDelete.Add(propertyDic.GetValue("FieldName"));
            }
            //dicList.AddRange(toAdd);
            dicList.RemoveAll(a => !toNoDelete.Contains(a.GetValue("FieldName")));
            CtrlSetting = dicList.ToJson();
        }

        public IEnumerable<Dictionary<string,object>> GetCtrlAttrList()
        {
            return CtrlSetting.JsonToDictionaryList().OrderBy(a => Convert.ToInt32(
                        string.IsNullOrEmpty(a.GetValue("OrderIndex")) ? 0 : Convert.ToInt32(a.GetValue("OrderIndex")))
                        );
        }

        /// <summary>
        /// key tableName  value FieldName
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetPopupSelectorForeignTableFieldPairList()
        {
            var popupSelectors = CtrlSetting.JsonToDictionaryList().Where(a => a.GetValue("CtrlType") == ControlType.PopupSelector.ToString());
            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach (var s in popupSelectors)
            {
                string detail = s.GetValue("Detail");
                if (!string.IsNullOrEmpty(detail))
                {
                    var detailDic = detail.JsonToDictionary();
                    res.SetValue(detailDic.GetValue("textName"), detailDic.GetValue("foreignTableFieldName"));
                }
            }
            return res;
        }

        public List<Dictionary<string, object>> GetSubTableDicList(string fieldName, bool bContainForeignKeyField = true)
        {
            Type type = ReflectionHelper.GetTypeBy(this.DBName, EntityFullName);

            var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var singOne = properties.FirstOrDefault(a => a.Name == fieldName);
            singOne.CheckNotNull("未找到fieldName为{0}的字段".ReplaceArg(fieldName));

            //singOne.PropertyType.GenericTypeArguments[0] 对应List<T>中T的类型
            var resList = GetPropertyDicList(singOne.PropertyType.GenericTypeArguments[0]);
            if (!bContainForeignKeyField)
            {
                var v = (ForeignKeyAttribute[])singOne.GetCustomAttributes(typeof(ForeignKeyAttribute), false);
                v.CheckNotNullOrEmpty("外键字段{0}未定义ForeignKeyAttribute".ReplaceArg(singOne.Name));
                var foreignKeyName = v[0].Name;
                foreignKeyName.CheckNotNull("外键字段{0}的ForeignKeyAttribute的未传外键名参数".ReplaceArg(singOne.Name));
                resList.RemoveAll(a => a.GetValue("FieldName") == foreignKeyName);
            }

            return resList;
        }
        #region
        /// <summary>
        /// 获取数据库列对应的字段相关信息(ColumnName,FieldName,FieldType)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> GetPropertyDicList(Type type,bool bContainList = false)
        {
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                //必须可读可写
                if (!property.CanWrite || !property.CanRead)
                    continue;

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.SetValue("CtrlType", "");
                //非自定义类型
                var pType = property.PropertyType;
                if (!pType.IsPrimitive
                    && pType != typeof(Decimal)
                     && pType != typeof(Decimal?)
                    && pType != typeof(String)
                    && pType != typeof(DateTime)
                    && pType != typeof(DateTime?))
                {
                    if (bContainList && pType.Name == "List`1" && property.GetAccessors().Any(a => a.IsVirtual))
                    {
                        var v = property.GetCustomAttributes(typeof(JsonIgnoreAttribute), false);
                        //如果带有JsonIgnore则不应该作为表单子表
                        if (v.Length > 0)
                        {
                            continue;
                        }
                        //public virtual List<xxx>  XXXX;
                        dic.SetValue("CtrlType", ControlType.SubDataGrid.ToString());
                    }
                    else
                    {
                        continue;
                    }                    
                }
                //外键字段排除
                else if (pType == typeof(String))
                {
                    //获取子表的主外键字段名
                    var v = (ForeignKeyAttribute[])property.GetCustomAttributes(typeof(ForeignKeyAttribute), false);
                    if (v != null && v.Count() > 0)
                    {
                        continue;
                    }
                }

                //外键字段


                
                object[] arr = property.GetCustomAttributes(typeof(DescriptionAttribute), true);

                dic.SetValue("ColumnName", "");
                if (arr.Length > 0 && !string.IsNullOrEmpty(((DescriptionAttribute)arr[0]).Description))
                {
                    dic.SetValue("ColumnName",((DescriptionAttribute)arr[0]).Description);
                }

                dic.SetValue("FieldName", property.Name);
                dic.SetValue("FieldType", property.PropertyType.ToString());
                res.Add(dic);
            }
            return res;
        }
        #endregion
    }
}
