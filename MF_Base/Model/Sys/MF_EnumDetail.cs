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
    [Description("ö����ϸ")]
    public class MF_EnumDetail : Entity
	{
        [Required]
        public string MF_EnumId { get; set; }
        /// <summary>
        /// ģ��(���ݿ�)
        /// </summary>
        public string DBName { get; set; }
        /// <summary>
        /// ö�ٱ��
        /// </summary>
        public string CatCode { get; set; }
        /// <summary>
        /// ö������
        /// </summary>
        public string CatName { get; set; }

        /// <summary>
        /// ö������
        /// </summary>
        public String Text { get; set; }
        /// <summary>
        /// ö��ֵ
        /// </summary>
        public string Value { get; set; }
	}
}
