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
    [Description("流程实例")]
    public class WFInst : Entity
    {
        public WFInst()
        {

        }

        public string FormInstId { get; set; }
        public string WFDefInstId { get; set; }
        public string FlowState { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual WFDefInst WDefInst { get; set; }

        [JsonIgnore]
        public virtual List<WFStep> WFStep { get; set; }

        public WFStep GetCurrentStep()
        {
            var steps = GetCurrentSteps();
            if (steps.Count > 0)
                return steps[0];
            else
                return null;
        }
        public WFStep GetCurrentStep(string stepUserId)
        {
            //TODO:暂不考虑处理人
            if(string.IsNullOrEmpty(stepUserId))
            {
                return WFStep.FirstOrDefault(a => string.IsNullOrEmpty(a.NextStepId));
            }

            return WFStep.FirstOrDefault(a => string.IsNullOrEmpty(a.NextStepId) && a.StepUserId.Contains(stepUserId));
        }

        public List<WFStep> GetCurrentSteps()
        {
            return WFStep.Where(a => string.IsNullOrEmpty(a.NextStepId)).ToList();
        }
    }
}

namespace MF_WorkFlow
{
    public partial class WFInstConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFInst>
    {
        public WFInstConfiguration()
        {
            this.HasRequired(a => a.WDefInst).WithMany().HasForeignKey(a => a.WFDefInstId);
        }
    }
}
