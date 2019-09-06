//-----------------------------------------------------------------
//由T4模板Repository.tt自动生成
//-----------------------------------------------------------------







using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using EFBase;
using MF_WorkFlow.Model;


namespace MF_WorkFlow
{
   #region BaseDbContext
    public partial class BaseDbContext : DbContext
    {
			protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {	
		    //禁用自动生成数据表末尾加s或者es
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
	
       WFNodeDefInstConfiguration WFNodeDefInstConfiguration = new WFNodeDefInstConfiguration();
           modelBuilder.Configurations.Add(WFNodeDefInstConfiguration);


       WFDefInstConfiguration WFDefInstConfiguration = new WFDefInstConfiguration();
           modelBuilder.Configurations.Add(WFDefInstConfiguration);


       WFStepConfiguration WFStepConfiguration = new WFStepConfiguration();
           modelBuilder.Configurations.Add(WFStepConfiguration);


       WFRoutingDefInstConfiguration WFRoutingDefInstConfiguration = new WFRoutingDefInstConfiguration();
           modelBuilder.Configurations.Add(WFRoutingDefInstConfiguration);


       WFRoutingDefConfiguration WFRoutingDefConfiguration = new WFRoutingDefConfiguration();
           modelBuilder.Configurations.Add(WFRoutingDefConfiguration);


       WFNodeDefConfiguration WFNodeDefConfiguration = new WFNodeDefConfiguration();
           modelBuilder.Configurations.Add(WFNodeDefConfiguration);


       WFInstConfiguration WFInstConfiguration = new WFInstConfiguration();
           modelBuilder.Configurations.Add(WFInstConfiguration);


       WFDefConfiguration WFDefConfiguration = new WFDefConfiguration();
           modelBuilder.Configurations.Add(WFDefConfiguration);


           base.OnModelCreating(modelBuilder);
 }
    }
	#endregion
	#region EntityTypeConfiguration
	  
       public partial class WFNodeDefInstConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFNodeDefInst>{}

       public partial class WFDefInstConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFDefInst>{}

       public partial class WFStepConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFStep>{}

       public partial class WFRoutingDefInstConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFRoutingDefInst>{}

       public partial class WFRoutingDefConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFRoutingDef>{}

       public partial class WFNodeDefConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFNodeDef>{}

       public partial class WFInstConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFInst>{}

       public partial class WFDefConfiguration : EntityTypeConfiguration<MF_WorkFlow.Model.WFDef>{}

	#endregion
}