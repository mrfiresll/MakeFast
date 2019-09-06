using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MF_Base.Model
{
    [Description("用户用于测试1")]
    public class MF_UserForTest1 : Entity
    {
        [Required]
        public string MF_UserTTId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Attr { get; set; }
        public int Version { get; set; }
    }
}
