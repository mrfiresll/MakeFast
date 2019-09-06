using MFTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace UIBase
{
    public class CommonHelper
    {
        /// <summary>
        /// 获取类的fullName，以及DescriptionAttribute的内容
        /// 如果DescriptionAttribute未指定，则取fullName
        /// FullName,Description,ClassName
        /// </summary>
        /// <param name="modelPath">模型dll路径</param>
        /// <param name="baseEntityName">基类(用于过滤)</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> GetClassFullNames(string projName, string baseEntityName)
        {
            projName.CheckNotNullOrEmpty("projName");
            baseEntityName.CheckNotNullOrEmpty("baseEntityName");
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
            var types = ReflectionHelper.GetTypes(projName);
            foreach (Type type in types)
            {
                if (type.BaseType != null && type.BaseType.Name == baseEntityName)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    var attrResult = type.GetAttributes<DescriptionAttribute>(false);
                    string attrDescription = type.FullName;
                    if (attrResult.Length > 0)
                    {
                        attrDescription = attrResult[0].Description;
                    }

                    var res = new Dictionary<string, object>();
                    res.SetValue("FullName", type.FullName);
                    res.SetValue("Description", attrDescription);
                    res.SetValue("ClassName", type.Name);
                    results.Add(res);
                }
            }
            return results;
        }

        public static IEnumerable<dynamic> GetDBEnum(string catCode)
        {
            if (string.IsNullOrEmpty(catCode))
                return new List<dynamic>();

            SqlHelper sqlHelper = new SqlHelper("Base");
            string sql = "select MF_EnumDetail.* from MF_Enum inner join MF_EnumDetail on MF_EnumDetail.MF_EnumId = MF_Enum.Id where MF_Enum.Code = '" + catCode + "'";
            var dt = sqlHelper.ExcuteTable(sql);
            return dt.ToDicList().Select(a => new { text = a.GetValue("Text"), value = a.GetValue("Value") });
        }

        public static IEnumerable<Dictionary<string, object>> GetMainTypeTree()
        {
            var dbNames = WebConfigHelper.GetDBNames();
            SqlHelper sqlHelper = new SqlHelper("Base");
            string sql = "";
            string topId = CommonStr.MainTypeTreeRootID;
            string topName = ConfigurationManager.AppSettings["AppTitle"];
            for (int i = 0; i < dbNames.Count(); i++)
            {
                string tmpSql = @"select  '{0}' as Id, '' as ParentId, '{1}' Text, '' as IconCls, 0 as OrderIndex, '' as DBName, '{0}' as FullId
                      union
                      select  '{2}' as Id, '{0}' as ParentId, '{2}' Text, '' as IconCls, 0 as OrderIndex, '{2}' as DBName, '{0}.{2}' as FullId
                      union 
                      select Id, case when ParentId is null then '{2}' else ParentId end ParentId,Text,IconCls,OrderIndex, DBName,FullId FROM MF_MainType where DBName = '{2}' ";
                sql += string.Format(tmpSql, topId, topName, dbNames[i]);

                if (i != dbNames.Count() - 1)
                {
                    sql += " union ";
                }
            }

            var dt = sqlHelper.ExcuteTable(sql);
            return dt.ToDicList();
        }

        public static IEnumerable<Dictionary<string, object>> GetMainTypeTopTree()
        {
            string topId = CommonStr.MainTypeTreeRootID;
            string topName = ConfigurationManager.AppSettings["AppTitle"];
            var topMainType = new { Id = topId, ParentId = "", Text = topName, DBName = "", OrderIndex = 0 };
            var resList = WebConfigHelper.GetDBNames().Select(a => new { Id = a, ParentId = topId, Text = a, DBName = a, OrderIndex = 0 }).ToList();
            resList.Insert(0, topMainType);
            return resList.Select(a => a.ToDictionary());
        }
    }
}
