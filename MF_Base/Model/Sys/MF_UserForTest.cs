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
    [Description("用户用于测试")]
    public class MF_UserForTest : Entity
    {
        public string Name { get; set; }
    }
}
