using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoUI
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method,AllowMultiple=false,Inherited = false)]
    public class NoCheckLoginAttr:Attribute
    {
        public NoCheckLoginAttr() { }
    }
}