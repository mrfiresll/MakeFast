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
    [Description("平台备忘")]
    public class MF_MyLog : Entity
	{
        public string Title { get; set; }
        public string RemarkType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
	}
}
