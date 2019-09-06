using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJTZZXDB.Model
{
    [Description("招投标信息")]
    public class ProjTenderBid : Entity
    {
        /// <summary>
        /// 合同情况 正本
        /// </summary>
        public Int32 ContractDescribZ { get; set; }
        /// <summary>
        /// 投标人数(家)
        /// </summary>
        public Int32 BidderCount { get; set; }
        /// <summary>
        /// 合同额
        /// </summary>
        public Decimal ContractPrice { get; set; }
        /// <summary>
        /// 合同情况 副本
        /// </summary>
        public Int32 ContractDescribF { get; set; }
        /// <summary>
        /// 招标师Id
        /// </summary>
        public Int32? BidStaffId { get; set; }
        /// <summary>
        /// 招标代理内容
        /// </summary>
        public String BidContent { get; set; }
        /// <summary>
        /// 招标方式
        /// </summary>
        public string BidStyle { get; set; }
        /// <summary>
        /// 代理开始日期
        /// </summary>
        public DateTime? BeginDate { get; set; }
        /// <summary>
        /// 代理结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 中标金额
        /// </summary>
        public Decimal BidPrice { get; set; }
        /// <summary>
        /// 报建编号/采购编号
        /// </summary>
        public String BuildNum { get; set; }
        /// <summary>
        /// 中标通知书编号
        /// </summary>
        public String NoticeNum { get; set; }
        /// <summary>
        /// 中标通知书发出时间
        /// </summary>
        public DateTime? GetNoticeDate { get; set; }
        /// <summary>
        /// 中标单位
        /// </summary>
        public String GetCompany { get; set; }
        /// <summary>
        /// 评价意见
        /// </summary>
        public string Appraise { get; set; }
        /// <summary>
        /// 监管部门
        /// </summary>
        public String RegulatoryDepart { get; set; }
        /// <summary>
        /// 合同备案显示
        /// </summary>
        public string ContractCopy { get; set; }      
        /// <summary>
        /// 删除
        /// </summary>
        public Boolean Deleted { get; set; }
        /// <summary>
        /// 投资规模
        /// </summary>
        public Decimal TotalCharge { get; set; }
        public String AttachFileName { get; set; }
        public String AttachFilePath { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public String Describ { get; set; }
        //public StaffInfo BidStaff { get; set; }
        //public StaffInfo Creator { get; set; }
        //public StaffInfo Modifier { get; set; }
        //public ProjInfo ProjInfo { get; set; }
        ////public  ItemType BidType { get; set; }
        ////public ItemType AppraiseType { get; set; }
        ////public ItemType ContractCopyType { get; set; }
        ///// <summary>
        ///// 代理起止日期
        ///// </summary>
        //[NotMapped]
        //public String StartEndDate
        //{
        //    get
        //    {
        //        string val1 = "";
        //        string val2 = "";
        //        if (BeginDate != null)
        //        {
        //            val1 = BeginDate.Value.ToString("yyyy-MM-dd");// +EndDate.Value.ToString("yyyy-MM-dd");
        //        }

        //        if (EndDate != null)
        //        {
        //            val2 = EndDate.Value.ToString("yyyy-MM-dd");
        //        }


        //        return val1 + "~" + val2;
        //    }
        //}
        ///// <summary>
        ///// 招标方式名称
        ///// </summary>
        //[NotMapped]
        //public String BidTypeName
        //{
        //    get
        //    {
        //        return BidStyle.ToDescription();
        //    }
        //}
        ///// <summary>
        ///// 已提供合同情况
        ///// </summary>
        //[NotMapped]
        //public String BackState
        //{
        //    get
        //    {
        //        if (ProjInfo != null && ProjInfo.ContractInfo != null && ProjInfo.ContractInfo.BackDate != null)
        //        {
        //            return "已提供";
        //        }
        //        return "未提供";
        //    }
        //}
        ///// <summary>
        ///// 合同情况
        ///// </summary>
        //[NotMapped]
        //public String ContractState
        //{
        //    get
        //    {
        //        return "正本" + ContractDescribZ + "份，副本" + ContractDescribF + "份";
        //    }
        //}
        ///// <summary>
        ///// 委托单位联系人电话
        ///// </summary>
        //[NotMapped]
        //public String NameTel
        //{
        //    get
        //    {
        //        if (ProjInfo != null && ProjInfo.ContractInfo != null)
        //        {
        //            return ProjInfo.ContractInfo.ClientPeopleName + " " +
        //                ProjInfo.ContractInfo.ClientPeopleTels;
        //        }
        //        return "";
        //    }
        //}
        ///// <summary>
        ///// 意见评价显示值
        ///// </summary>
        //[NotMapped]
        //public String AppraiseStr
        //{
        //    get
        //    {
        //        return Appraise.ToDescription();

        //    }
        //}
        //[NotMapped]
        //public String ContractCopyStr
        //{
        //    get
        //    {
        //        EnumContractCopy CopyValue = ContractCopy;
        //        if (ContractCopy == 0)
        //        {
        //            CopyValue = EnumContractCopy.No;
        //            return CopyValue.ToDescription();
        //        }
        //        else
        //        {
        //            return CopyValue.ToDescription();
        //        }

        //    }
        //}
        //[NotMapped]
        //public int ContractCopyValue  //将数据库中的0转化一下，保持新旧平台数据兼容
        //{
        //    get
        //    {
        //        if (ContractCopy == 0)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            return Convert.ToInt32(ContractCopy);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 中标通知书发出时间
        ///// </summary>
        //[NotMapped]
        //public string GetNoticeDateStr
        //{
        //    get
        //    {
        //        return GetNoticeDate == null ? "" : GetNoticeDate.Value.ToString("yyyy-MM-dd");
        //    }
        //}
        ///// <summary>
        ///// 最近修改时间
        ///// </summary>
        //[NotMapped]
        //public string ModifyTimeStr
        //{
        //    get
        //    {
        //        return ModifyTime == null ? "无" : ModifyTime.Value.ToString("yyyy-MM-dd");
        //    }
        //}
        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //[NotMapped]
        //public string CreateTimeStr
        //{
        //    get
        //    {
        //        return CreateTime == null ? "无" : CreateTime.ToString("yyyy-MM-dd");
        //    }
        //}

        ///// <summary>
        ///// 附件是否存在判断
        ///// </summary>
        //[NotMapped]
        //public string FileState
        //{
        //    get
        //    {
        //        if (!String.IsNullOrEmpty(AttachFileName) && !String.IsNullOrEmpty(AttachFilePath))
        //        {
        //            return "已上传附件";
        //        }
        //        else
        //        {
        //            return "无附件";
        //        }
        //    }
        //}
        ///// <summary>
        ///// 附件名显示
        ///// </summary>
        //[NotMapped]
        //public string FileName
        //{
        //    get
        //    {
        //        if (!String.IsNullOrEmpty(AttachFileName) && !String.IsNullOrEmpty(AttachFilePath))
        //        {
        //            return AttachFileName.ToString();
        //        }
        //        else
        //        {
        //            return "（空）";
        //        }
        //    }
        //}
        //[NotMapped]
        //public string BidStaffName
        //{
        //    get
        //    {
        //        return BidStaff != null ? BidStaff.RealName : "无";
        //    }
        //}

        //[NotMapped]
        //public string CreatorName
        //{
        //    get
        //    {
        //        return Creator != null ? Creator.RealName : "无";
        //    }
        //}
        //[NotMapped]
        //public String BidStyleStr
        //{
        //    get
        //    {
        //        return BidStyle.ToDescription();
        //    }
        //}
        //[NotMapped]
        //public String ModifierStr
        //{
        //    get
        //    {
        //        return Modifier == null ? "无" : Modifier.RealName;
        //    }
        //}

    }
}
