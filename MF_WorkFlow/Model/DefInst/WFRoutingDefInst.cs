using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MF_WorkFlow.Model
{
    [Description("路由定义实例")]
    public class WFRoutingDefInst : Entity
    {
        [MaxLength(100)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override string Id { get; set; }
        public string WFDefInstId { get; set; }
        public string SNodeDefInstId { get; set; }
        public string SNodeAnchor { get; set; }//锚点
        public string ENodeDefInstId { get; set; }
        public string ENodeAnchor { get; set; }//锚点
        public string Name { get; set; }
        #region 路由通行权限
        public string PassUserId { get; set; }//通行人Id
        public string PassUserName { get; set; }//通行人名称
        public string PassRoleId { get; set; }//通行角色Id
        public string PassRoleName { get; set; }//通行角色名称
        //通行的判别表达式如：{FormData1}-{FormData2}*{FormData3} 其中FormData为表单字段
        public string PassExpression { get; set; }
        #endregion

        #region 接收人(下一环节执行人)
        public string NextExcuteUserId { get; set; }//下一环节执行人Id
        public string NextExcuteUserName { get; set; }//下一环节执行人名称
        public string NextExcuteUserRoleId { get; set; }//下一环节执行人角色Id
        public string NextExcuteUserRoleName { get; set; }//下一环节执行人角色名
        public string NextExcuteUserSQLSourceId { get; set; }//下一环节执行人数据源Id
        public string NextExcuteUserSQLSourceName { get; set; }//下一环节执行人数据源名称
        public string NextExcuteUserSQLSourceResKeyField { get; set; }//下一环节数据原返回结果要取的字段Key
        public string NextExcuteUserSQLSourceResValueField { get; set; }//下一环节数据原返回结果要取的字段Value
        #endregion

        [JsonIgnore]
        public virtual WFDefInst WFDefInst { get; set; }
        [JsonIgnore]
        public virtual WFNodeDefInst S_WFNodeDefInst { get; set; }
        [JsonIgnore]
        public virtual WFNodeDefInst E_WFNodeDefInst { get; set; }

        public bool IsDefineNextExcuteUsers
        {
            get
            {
                return !(string.IsNullOrEmpty(NextExcuteUserId)
                    && string.IsNullOrEmpty(NextExcuteUserRoleId)
                     && string.IsNullOrEmpty(NextExcuteUserSQLSourceId));
            }
        }
    }
}

namespace MF_WorkFlow
{
    public partial class WFRoutingDefInstConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFRoutingDefInst>
    {
        public WFRoutingDefInstConfiguration()
        {
            this.HasRequired(a => a.WFDefInst).WithMany(a => a.WFRoutingDefInst).HasForeignKey(a => a.WFDefInstId);
            this.HasRequired(a => a.S_WFNodeDefInst).WithMany().HasForeignKey(a => a.SNodeDefInstId).WillCascadeOnDelete(false);
            this.HasRequired(a => a.E_WFNodeDefInst).WithMany().HasForeignKey(a => a.ENodeDefInstId).WillCascadeOnDelete(false);
        }
    }
}
