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
    [Description("·�ɶ���")]
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
        public string SNodeAnchor { get; set; }//ê��
        public string ENodeDefId { get; set; }
        public string ENodeAnchor { get; set; }//ê��        
        public string Name { get; set; }

        #region ·��ͨ��Ȩ��
        public string PassUserId { get; set; }//ͨ����Id
        public string PassUserName { get; set; }//ͨ��������
        public string PassRoleId { get; set; }//ͨ�н�ɫId
        public string PassRoleName { get; set; }//ͨ�н�ɫ����
        //ͨ�е��б���ʽ�磺{FormData1}-{FormData2}*{FormData3} ����FormDataΪ���ֶ�
        public string PassExpression { get; set; }
        #endregion

        #region ������(��һ����ִ����)
        public string NextExcuteUserId { get; set; }//��һ����ִ����Id
        public string NextExcuteUserName { get; set; }//��һ����ִ��������
        public string NextExcuteUserRoleId { get; set; }//��һ����ִ���˽�ɫId
        public string NextExcuteUserRoleName { get; set; }//��һ����ִ���˽�ɫ��
        public string NextExcuteUserSQLSourceId { get; set; }//��һ����ִ��������ԴId
        public string NextExcuteUserSQLSourceName { get; set; }//��һ����ִ��������Դ����
        public string NextExcuteUserSQLSourceResKeyField { get; set; }//��һ��������ԭ���ؽ��Ҫȡ���ֶ�Key
        public string NextExcuteUserSQLSourceResValueField { get; set; }//��һ��������ԭ���ؽ��Ҫȡ���ֶ�Value
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
