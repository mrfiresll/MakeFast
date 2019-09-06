using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.Composition;
using System.ComponentModel;
using Newtonsoft.Json;

namespace MF_Base.Model
{
    [Description("用户TT")]
    public class MF_UserTT : Entity
    {
        public String LoginName { get; set; }
        public String PassWord { get; set; }
        public String Code { get; set; }
        public String RealName { get; set; }
        public String Sex { get; set; }
        public Int32 Age { get; set; }
        public DateTime? Birthday { get; set; }       
        public Boolean Enabled { get; set; }        
        [ForeignKey("MF_UserTTId")]
        public virtual List<MF_UserForTest1> MF_UserForTest1 { get; set; }
        [JsonIgnore]
        [ForeignKey("MF_UserTTId")]
        public virtual List<MF_UserForTest2> MF_UserForTest2 { get; set; }
    }
}