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
    [Description("用户")]
    public class MF_User : Entity
    {
        [ForeignKey("MF_UserForTest")]
        public string MF_UserForTestId { get; set; }
        [ForeignKey("MF_Department")]
        public string MF_DepartmentId { get; set; }
        public String LoginName { get; set; }
        public String PassWord { get; set; }
        public String Code { get; set; }
        public String RealName { get; set; }
        public String Sex { get; set; }
        public Int32 Age { get; set; }
        public DateTime? Birthday { get; set; }       
        public Boolean Enabled { get; set; }
        public virtual MF_Department MF_Department { get; set; }

        [JsonIgnore]        
        public virtual MF_UserForTest MF_UserForTest { get; set; }


        #region get
        public string DepartName { get { return MF_Department == null ? "" : MF_Department.Name; } }
        #endregion
    }
}