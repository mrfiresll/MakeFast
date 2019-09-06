using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel;

namespace MF_Base.Model
{
    [Description("功能菜单")]
    public class MF_Func : Entity
    {
        public MF_Func()
        {
            //Id = DomainHelper.CreateTimeOrderID();
        }
        public string ParentId { get; set; }
        /// <summary>
        /// 文本描述
        /// </summary>
        public String Text { get; set; }
        /// <summary>
        /// 可作为url，亦可作为控件id
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public String IconCls { get; set; }
        /// <summary>
        /// 功能类型
        /// </summary>
        public string EnumFuncType { get; set; }

        public string Remark { get; set; }

        //public bool HasChildren { get { return Children.Count > 0; } }

        //public Int32 RoleId { get; set; }
        [JsonIgnore]
        public MF_Func Parent { get; set; }
        [JsonIgnore]
        [ForeignKey("ParentId")]
        public virtual List<MF_Func> Children { get; set; }
        [JsonIgnore]
        [ForeignKey("MF_FuncId")]
        public virtual List<MF_FuncRole> MF_FuncRole { get; set; }

        public bool IsBindToRole(string RoleId)
        {
            return MF_FuncRole != null && MF_FuncRole.Any(a => a.MF_RoleId == RoleId);
        }
    }
}
