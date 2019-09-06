using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJTZZXDB.Model
{
    [Description("合同")]
    /// <summary>
    /// 合同
    /// </summary>
    public class ContractInfo: Entity
    {
        public ContractInfo()
        {
             
        }
        /// <summary>
        /// 合同编号格式1234-12345
        /// </summary>
        public String ContractNum { get; set; }
        /// <summary>
        /// 合同名称
        /// </summary>
        public String ContractName { get; set; }
        /// <summary>
        /// 咨询类型名称
        /// </summary>
        public String ConsultTypeName { get; set; }
        /// <summary>
        /// 项目类型名称
        /// </summary>
        public String ContractTypeName { get; set; }
        /// <summary>
        /// 项目经理
        /// </summary>
        public Int32? ProjManagerId { get; set; }
        /// <summary>
        /// 作业部门
        /// </summary>
        public Int32? WorkDepartId { get; set; }
        /// <summary>
        /// 部门经理
        /// </summary>
        public Int32? DepartManagerId { get; set; }
        /// <summary>
        /// 案值(元)
        /// </summary>
        public Decimal? Value { get; set; }
        /// <summary>
        /// 建筑面积(m2)
        /// </summary>
        public Decimal? Area { get; set; }
        /// <summary>
        /// 估计收费金额(元)
        /// </summary>
        public Decimal? ExpectedCharge { get; set; }
        /// <summary>
        /// 委托单位
        /// </summary>
        public String ClientName { get; set; }
        /// <summary>
        /// 委托单位联系人
        /// </summary>
        public String ClientPeopleName { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public String ClientPeopleTels { get; set; }
        /// <summary>
        /// 合同签订时间
        /// </summary>
        public DateTime? SignDate { get; set; }
        /// <summary>
        /// 合同预领时间
        /// </summary>
        public DateTime? AdvanceDate { get; set; }
        /// <summary>
        /// 预领人
        /// </summary>
        public String Advancer { get; set; }
        /// <summary>
        /// 合同返回时间
        /// </summary>
        public DateTime? BackDate { get; set; }
        /// <summary>
        /// 合同价(元)
        /// </summary>
        public Decimal? SurePay { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String RegistrantName { get; set; }
        /// <summary>
        /// 合同状态
        /// </summary>
        public string ContractState { get; set; }
        /// <summary>
        /// 咨询类型Id
        /// </summary>
        public Int32? ConsultTypeId { get; set; }

        ////public bool ContractReiview

        //public TabContractDescrib TabContractDescrib { get; set; }
        //public TabContractClientReq TabContractClientReq { get; set; }
        //public TabContractReview TabContractReview { get; set; }
        //public List<P_CConfirmCharge> P_CConfirmCharges { get; set; }
        //public StaffInfo ProjManager { get; set; }
        //public ItemDepartInfo WorkDepart { get; set; }
        //public StaffInfo DepartManager { get; set; }
        //public StaffInfo Creator { get; set; }
        //public StaffInfo Modifier { get; set; }
        //public List<ProjInfo> ProjInfos { get; set; }
        //public ItemType ItemType { get; set; }
        //public ContractInfoExtend ContractInfoExtend { get; set; }
        //public bool IsChongMingDepart 
        //{
        //    get 
        //    {
        //        return WorkDepart != null ? WorkDepart.IsChongMingDepart : false;
        //    }
        //}

        //public bool IsZhaoBiaoDaiLi
        //{
        //    get
        //    {
        //        return this.ConsultTypeId == 217;
        //    }
        //}

        //public bool IsZhengFuCaiGou
        //{
        //    get
        //    {
        //        return this.ConsultTypeId == 215;
        //    }
        //}
        //[NotMapped]
        //public string ProjManagerName
        //{
        //    get
        //    {
        //        return ProjManager != null ? ProjManager.RealName : "";
        //    }
        //}
        //[NotMapped]
        //public string WorkDepartName
        //{
        //    get
        //    {
        //        return WorkDepart != null ? WorkDepart.DepartName : "";
        //    }
        //}
        //[NotMapped]
        //public string DepartManagerName
        //{
        //    get
        //    {
        //        return WorkDepart != null ? WorkDepart.DepartManagerName : "";
        //    }
        //}
        ////最近修改时间
        //[NotMapped]
        //public string ModifyTimeStr
        //{
        //    get
        //    {
        //        return ModifyTime == null ? "" : ModifyTime.Value.ToString("yyyy-MM-dd");
        //    }
        //}
        ////创建时间
        //[NotMapped]
        //public string CreateTimeStr
        //{
        //    get
        //    {
        //        return CreateTime == null ? "" : CreateTime.ToString("yyyy-MM-dd");
        //    }
        //}
        ////合同返回时间
        //[NotMapped]
        //public string BackDateStr
        //{
        //    get
        //    {
        //        return BackDate == null ? "" : BackDate.Value.ToString("yyyy-MM-dd");
        //    }
        //}
        ///// <summary>
        ///// 合同预领时间
        ///// </summary>
        //[NotMapped]
        //public string AdvanceDateStr
        //{
        //    get
        //    {
        //        return AdvanceDate == null ? "" : AdvanceDate.Value.ToString("yyyy-MM-dd");
        //    }
        //}
        ///// <summary>
        ///// 合同签订时间
        ///// </summary>
        //[NotMapped]
        //public string SignDateDateStr
        //{
        //    get
        //    {
        //        return SignDate == null ? "" : SignDate.Value.ToString("yyyy-MM-dd");
        //    }
        //}
        ///// <summary>
        ///// 查看功能最近修改时间显示值
        ///// </summary>
        ///// [NotMapped]
        //public String ModifyTimeState
        //{
        //    get
        //    {
        //        if (ModifyTime == null)
        //        {
        //            return "无";
        //        }

        //        else
        //        {
        //            return ModifyTimeStr;
        //        }
        //    }
        //}
        //[NotMapped]
        //public String CreatorRealName
        //{
        //    get
        //    {
        //        return Creator != null ? Creator.RealName : "读取数据出错";

        //    }
        //}
        //[NotMapped]
        //public String AreaStr
        //{
        //    get
        //    {
        //        return Area != null ? Area.ToString() : "";

        //    }
        //}
        ///// <summary>
        ///// 查看功能最近修改人显示值
        ///// </summary>
        //[NotMapped]
        //public String ModifyerState
        //{
        //    get
        //    {
        //        if (ModifierId == null)
        //        {
        //            return "无";
        //        }

        //        else
        //        {
        //            return Modifier != null ? Modifier.RealName : "读取数据出错";
        //        }
        //    }
        //}

        //[NotMapped]
        //public String StateName
        //{
        //    get
        //    {
        //        if (Convert.ToInt32(ContractState) == 1)
        //        {
        //            return "未开始";
        //        }

        //        else if (Convert.ToInt32(ContractState) == 2)
        //        {
        //            return "签订完成";
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //}
    }
}
