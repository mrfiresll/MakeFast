using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MFTool
{
    public static class NameValueCollectionExtension 
    {
        public static Dictionary<string, object> NameValueCollectToDictionary(this NameValueCollection col, bool ignoreCase = true)
        {
            Dictionary<string, object> dict;
            if (ignoreCase)
            {
                dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                dict = new Dictionary<string, object>();
            }            

            foreach (string key in col.Keys)
            {
                dict.Add(key.ToString(), col[key]);
            }

            return dict;
        }
    }
}