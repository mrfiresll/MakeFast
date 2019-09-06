using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel;

namespace MF_WorkFlow.Model
{
    [Description("�ڵ�ʵ��")]
    public class WFStep : Entity
    {
        public WFStep()
        {

        }

        public string WFNodeDefInstId { get; set; }
        public string WFInstId { get; set; }
        public string PreStepId { get; set; }
        public string NextStepId { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// ��ǰ���������
        /// </summary>
        public string StepUserId { get; set; }
        public string StepUserName { get; set; }
        /// <summary>
        /// ��ǰ���账����
        /// </summary>
        public string OperateUserId { get; set; }
        public string OperateUserName { get; set; }
        public DateTime? OperateTime { get; set; }
        [JsonIgnore]
        public virtual WFNodeDefInst WFNodeDefInst { get; set; }
        public virtual WFInst WFInst { get; set; }
        public virtual WFStep PreStep { get; set; }
        //public WFStep NextStep { get; set; }
    }
}

namespace MF_WorkFlow
{
    public partial class WFStepConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFStep>
    {
        public WFStepConfiguration()
        {
            this.HasOptional(a => a.PreStep).WithMany().HasForeignKey(a => a.PreStepId);
            //this.HasOptional(a => a.NextStep).WithMany().HasForeignKey(a => a.NextStepId); ���ϻ���ѭ������
            this.HasRequired(a => a.WFInst).WithMany(a => a.WFStep).HasForeignKey(a => a.WFInstId);
            this.HasRequired(a => a.WFNodeDefInst).WithMany().HasForeignKey(a => a.WFNodeDefInstId).WillCascadeOnDelete(false);
        }
    }
}
