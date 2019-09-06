using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MFTool;

namespace AutoUI.Helper
{
    public class EasyUIHelper
    {
        public static IEnumerable<Dictionary<string, object>> GetTreeJson<T>(IEnumerable<T> list, string idField = "Id", string parentIdField = "ParentId")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            var topList = list.ToDicList().Where(a => a.GetValue(parentIdField) == "").OrderBy(a => Convert.ToInt32(a.GetValue("OrderIndex")));
            foreach(var top in topList)
            {
                MakeTree(top, idField, parentIdField, list.ToDicList());
            }
           
            return topList;
        }

        public static IEnumerable<Dictionary<string, object>> GetTreeJson(IEnumerable<Dictionary<string, object>> list, string idField = "Id", string parentIdField = "ParentId")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();           
            var topList = list.Where(a => a.GetValue(parentIdField) == "").OrderBy(a => Convert.ToInt32(a.GetValue("OrderIndex"))).ToList();
            foreach (var top in topList)
            {
                MakeTree(top, idField, parentIdField, list);
            }

            return topList;
        }

        private static void MakeTree(Dictionary<string,object> parent, string idField,string parentIdField, IEnumerable<Dictionary<string,object>> source)
        {
            var children = source.Where(a => a.GetValue(parentIdField) == parent.GetValue(idField)).OrderBy(a => Convert.ToDouble(a.GetValue("OrderIndex")));
            parent.SetValue("children", children);

            foreach (var child in children)
            {               
                MakeTree(child, idField, parentIdField, source);
            }
        }
    }
}