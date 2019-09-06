using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    public static class StringExtension
    {
        public static string ReplaceArg(this string source, params object[] arg)
        {
            if (string.IsNullOrEmpty(source)) return source;
            return string.Format(source, arg);
        }
    }
}
