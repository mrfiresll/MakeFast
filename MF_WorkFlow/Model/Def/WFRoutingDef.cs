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
    [Description("路由定义")]
    public class WFRoutingDef : Entity
	{
        public WFRoutingDef()
        {

        }

        [MaxLength(100)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override string Id { get; set; }
        public string WFDefId { get; set; }
        public string SNodeDefId { get; set; }
        public string SNodeAnchor { get; set; }//锚点
        public string ENodeDefId { get; set; }
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

        public WFDef WFDef { get; set; }

        [JsonIgnore]
        public virtual WFNodeDef S_WFNodeDef { get; set; }
        [JsonIgnore]
        public virtual WFNodeDef E_WFNodeDef { get; set; }
	}
}

namespace MF_WorkFlow
{
    public partial class WFRoutingDefConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFRoutingDef>
    {
        public WFRoutingDefConfiguration()
        {
            this.HasRequired(a => a.WFDef).WithMany(a => a.WFRoutingDef).HasForeignKey(a => a.WFDefId);
            this.HasRequired(a => a.S_WFNodeDef).WithMany().HasForeignKey(a => a.SNodeDefId).WillCascadeOnDelete(false);
            this.HasRequired(a => a.E_WFNodeDef).WithMany().HasForeignKey(a => a.ENodeDefId).WillCascadeOnDelete(false);
        }
    }
}
