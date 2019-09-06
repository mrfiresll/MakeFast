//-----------------------------------------------------------------
//由T4模板Repository.tt自动生成
//-----------------------------------------------------------------







using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using EFBase;


namespace MF_Project
{
   #region BaseDbContext
    public partial class BaseDbContext : DbContext
    {
			protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {	
		    //禁用自动生成数据表末尾加s或者es
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
	
       MFDocumentConfiguration MFDocumentConfiguration = new MFDocumentConfiguration();
           modelBuilder.Configurations.Add(MFDocumentConfiguration);


           base.OnModelCreating(modelBuilder);
 }
    }
	#endregion
	#region EntityTypeConfiguration
	  
       public partial class MFDocumentConfiguration : EntityTypeConfiguration<MF_Project.Model.MFDocument>{}

	#endregion
}