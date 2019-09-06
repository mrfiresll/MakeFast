using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel;

namespace MF_Base.Model
{
    [Description("枚举")]
    public class MF_Enum : Entity
	{
        /// <summary>
        /// 模块(数据库)
        /// </summary>
        public string DBName { get; set; }
        /// <summary>
        /// 枚举编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 枚举名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        [ForeignKey("MF_EnumId")]
        public List<MF_EnumDetail> MF_EnumDetail { get; set; }
	}
}
