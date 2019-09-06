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
    [Description("ö��")]
    public class MF_Enum : Entity
	{
        /// <summary>
        /// ģ��(���ݿ�)
        /// </summary>
        public string DBName { get; set; }
        /// <summary>
        /// ö�ٱ��
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// ö������
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        public string Remark { get; set; }
        [ForeignKey("MF_EnumId")]
        public List<MF_EnumDetail> MF_EnumDetail { get; set; }
	}
}
