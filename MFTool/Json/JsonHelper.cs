using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MFTool
{
    public class JsonHelper
    {
        /// <summary>
        /// 去除json key双引号
        /// </summary>
        /// <param name="jsonInput">json</param>
        /// <returns>去除key引号</returns>
        public static string JsonRegex(string jsonInput)
        {
            string result = string.Empty;
            try
            {
                string pattern = "\"(\\w+)\"(\\s*:\\s*)";
                string replacement = "$1$2";
                System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(pattern);
                result = rgx.Replace(jsonInput, replacement);
            }
            catch 
            {
                result = jsonInput;
            }
            return result;
        }
    }
}