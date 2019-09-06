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
    [Description("·�ɶ���ʵ��")]
    public class WFRoutingDefInst : Entity
    {
        [MaxLength(100)]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override string Id { get; set; }
        public string WFDefInstId { get; set; }
        public string SNodeDefInstId { get; set; }
        public string SNodeAnchor { get; set; }//ê��
        public string ENodeDefInstId { get; set; }
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
