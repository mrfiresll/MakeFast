using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel;
using System.Data;

namespace MF_Base.Model
{
    [Description("表单列表主分类")]
    public class MF_MainType : Entity
    {
        public MF_MainType()
        {
        }
        public string DBName { get; set; }
        public string ParentId { get; set; }
        public string FullId { get; set; }
        /// <summary>
        /// 文本描述
        /// </summary>
        public String Text { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public String IconCls { get; set; }
        public string Remark { get; set; }
        [JsonIgnore]
        public MF_MainType Parent { get; set; }
        [JsonIgnore]
        [ForeignKey("ParentId")]
        public virtual List<MF_MainType> Children { get; set; }
    }
}
