using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false,Inherited=true)]
    public class CascadeRelationsAttribute: Attribute
    {
        public CascadeRelationsAttribute() { }
        //public Type Type { get; set; }
        //public string Property { get; set; }

        //public CascadeRelationsAttribute(Type type,string property)
        //{
        //    this.Type = type;
        //    this.Property = property;
        //}
    }
}
