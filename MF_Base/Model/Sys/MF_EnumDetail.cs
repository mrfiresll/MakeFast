using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel;

namespace MF_Base.Model
{
    [Description("枚举明细")]
    public class MF_EnumDetail : Entity
	{
        [Required]
        public string MF_EnumId { get; set; }
        /// <summary>
        /// 模块(数据库)
        /// </summary>
        public string DBName { get; set; }
        /// <summary>
        /// 枚举编号
        /// </summary>
        public string CatCode { get; set; }
        /// <summary>
        /// 枚举名称
        /// </summary>
        public string CatName { get; set; }

        /// <summary>
        /// 枚举描述
        /// </summary>
        public String Text { get; set; }
        /// <summary>
        /// 枚举值
        /// </summary>
        public string Value { get; set; }
	}
}
