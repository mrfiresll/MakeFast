using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MFTool
{
    public static class DataTableExtensions
    {
        public static List<Dictionary<string, object>> ToDicList(this DataTable dt)
        {
            try
            {
                var array = new List<Dictionary<string, object>>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];

                    Dictionary<string, object> record = new Dictionary<string, object>();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        object cellValue = row[j];
                        if (cellValue.GetType() == typeof(DBNull))
                        {
                            cellValue = null;
                        }
                        record[dt.Columns[j].ColumnName] = cellValue;
                    }
                    array.Add(record);
                }
                return array;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }

        }
    }
}