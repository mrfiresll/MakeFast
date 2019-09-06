using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace MF_Base.Model
{
    [Description("用户用于测试2")]
    public class MF_UserForTest2 : Entity
    {
        public string MF_UserTTId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Attr { get; set; }
        public int Version { get; set; }
    }
}
