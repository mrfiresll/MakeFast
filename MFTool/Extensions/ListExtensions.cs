using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MFTool
{
    public static class ListExtensions
    {
        static public DataTable ToDataTable<T>(this IEnumerable<T> varlist, bool bUseDescription = false)
        {
            DataTable dtReturn = new DataTable();

            // column names   
            PropertyInfo[] oProps = null;

            // Could add a check to verify that there is an element 0
            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow   
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;
                        string tmpName = bUseDescription ? pi.ToDescription() : pi.Name;
                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(tmpName, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow(); 
                foreach (PropertyInfo pi in oProps)
                {
                    string tmpName = bUseDescription ? pi.ToDescription() : pi.Name;
                    dr[tmpName] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                }

                dtReturn.Rows.Add(dr);
            }

            return (dtReturn);
        }

        static public IEnumerable<T> DistinctBy<T>(this IEnumerable<T> source, Func<T,Guid> keySelector)
        {
            return source.GroupBy(keySelector).Select(group => group.First());
        }

        static public IEnumerable<Dictionary<string,object>> ToDicList<T>(this IEnumerable<T> source)
        {
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            foreach(var tt in source)
            {
                res.Add(tt.ToDictionary());
            }
            return res;
        }
    }
}
