//-----------------------------------------------------------------
//由T4模板Repository.tt自动生成
//-----------------------------------------------------------------







using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using EFBase;


namespace JJTZZXDB
{
   #region BaseDbContext
    public partial class BaseDbContext : DbContext
    {
			protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {	
		    //禁用自动生成数据表末尾加s或者es
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
	
       ProjTenderBidConfiguration ProjTenderBidConfiguration = new ProjTenderBidConfiguration();
           modelBuilder.Configurations.Add(ProjTenderBidConfiguration);


       ContractInfoConfiguration ContractInfoConfiguration = new ContractInfoConfiguration();
           modelBuilder.Configurations.Add(ContractInfoConfiguration);


           base.OnModelCreating(modelBuilder);
 }
    }
	#endregion
	#region EntityTypeConfiguration
	  
       public partial class ProjTenderBidConfiguration : EntityTypeConfiguration<JJTZZXDB.Model.ProjTenderBid>{}

       public partial class ContractInfoConfiguration : EntityTypeConfiguration<JJTZZXDB.Model.ContractInfo>{}

	#endregion
}