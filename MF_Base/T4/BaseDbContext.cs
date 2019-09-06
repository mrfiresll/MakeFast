//-----------------------------------------------------------------
//由T4模板Repository.tt自动生成
//-----------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using EFBase;
using MF_Base.Model;


namespace MF_Base
{
    #region BaseDbContext
    public partial class BaseDbContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //禁用自动生成数据表末尾加s或者es
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            MF_MyLogConfiguration MF_MyLogConfiguration = new MF_MyLogConfiguration();
            modelBuilder.Configurations.Add(MF_MyLogConfiguration);


            MF_EnumDetailConfiguration MF_EnumDetailConfiguration = new MF_EnumDetailConfiguration();
            modelBuilder.Configurations.Add(MF_EnumDetailConfiguration);


            MF_EnumConfiguration MF_EnumConfiguration = new MF_EnumConfiguration();
            modelBuilder.Configurations.Add(MF_EnumConfiguration);


            MF_MainTypeConfiguration MF_MainTypeConfiguration = new MF_MainTypeConfiguration();
            modelBuilder.Configurations.Add(MF_MainTypeConfiguration);


            MF_DepartmentConfiguration MF_DepartmentConfiguration = new MF_DepartmentConfiguration();
            modelBuilder.Configurations.Add(MF_DepartmentConfiguration);


            MF_RoleUserConfiguration MF_RoleUserConfiguration = new MF_RoleUserConfiguration();
            modelBuilder.Configurations.Add(MF_RoleUserConfiguration);


            MF_UserForTest2Configuration MF_UserForTest2Configuration = new MF_UserForTest2Configuration();
            modelBuilder.Configurations.Add(MF_UserForTest2Configuration);


            MF_UserTTConfiguration MF_UserTTConfiguration = new MF_UserTTConfiguration();
            modelBuilder.Configurations.Add(MF_UserTTConfiguration);


            MF_UserForTest1Configuration MF_UserForTest1Configuration = new MF_UserForTest1Configuration();
            modelBuilder.Configurations.Add(MF_UserForTest1Configuration);


            MF_UserForTestConfiguration MF_UserForTestConfiguration = new MF_UserForTestConfiguration();
            modelBuilder.Configurations.Add(MF_UserForTestConfiguration);


            SQLDataSourceConfiguration SQLDataSourceConfiguration = new SQLDataSourceConfiguration();
            modelBuilder.Configurations.Add(SQLDataSourceConfiguration);


            MF_FuncConfiguration MF_FuncConfiguration = new MF_FuncConfiguration();
            modelBuilder.Configurations.Add(MF_FuncConfiguration);


            MF_FuncRoleConfiguration MF_FuncRoleConfiguration = new MF_FuncRoleConfiguration();
            modelBuilder.Configurations.Add(MF_FuncRoleConfiguration);


            MF_RoleConfiguration MF_RoleConfiguration = new MF_RoleConfiguration();
            modelBuilder.Configurations.Add(MF_RoleConfiguration);


            FormConfigConfiguration FormConfigConfiguration = new FormConfigConfiguration();
            modelBuilder.Configurations.Add(FormConfigConfiguration);


            ListConfigConfiguration ListConfigConfiguration = new ListConfigConfiguration();
            modelBuilder.Configurations.Add(ListConfigConfiguration);


            MF_UserConfiguration MF_UserConfiguration = new MF_UserConfiguration();
            modelBuilder.Configurations.Add(MF_UserConfiguration);


            base.OnModelCreating(modelBuilder);
        }
    }
    #endregion
    #region EntityTypeConfiguration

    public partial class MF_MyLogConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_MyLog>
    {
        public MF_MyLogConfiguration()
        {

        }
    }

    public partial class MF_EnumDetailConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_EnumDetail>
    {
        public MF_EnumDetailConfiguration()
        {

        }
    }

    public partial class MF_EnumConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_Enum>
    {
        public MF_EnumConfiguration()
        {

        }
    }

    public partial class MF_MainTypeConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_MainType>
    {
        public MF_MainTypeConfiguration()
        {

        }
    }

    public partial class MF_DepartmentConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_Department>
    {
        public MF_DepartmentConfiguration()
        {

        }
    }

    public partial class MF_RoleUserConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_RoleUser>
    {
        public MF_RoleUserConfiguration()
        {

        }
    }

    public partial class MF_UserForTest2Configuration : EntityTypeConfiguration<MF_Base.Model.MF_UserForTest2>
    {
        public MF_UserForTest2Configuration()
        {

        }
    }

    public partial class MF_UserTTConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_UserTT>
    {
        public MF_UserTTConfiguration()
        {

        }
    }

    public partial class MF_UserForTest1Configuration : EntityTypeConfiguration<MF_Base.Model.MF_UserForTest1>
    {
        public MF_UserForTest1Configuration()
        {

        }
    }

    public partial class MF_UserForTestConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_UserForTest>
    {
        public MF_UserForTestConfiguration()
        {

        }
    }

    public partial class SQLDataSourceConfiguration : EntityTypeConfiguration<MF_Base.Model.SQLDataSource>
    {
        public SQLDataSourceConfiguration()
        {

        }
    }

    public partial class MF_FuncConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_Func>
    {
        public MF_FuncConfiguration()
        {

        }
    }

    public partial class MF_FuncRoleConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_FuncRole>
    {
        public MF_FuncRoleConfiguration()
        {

        }
    }

    public partial class MF_RoleConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_Role>
    {
        public MF_RoleConfiguration()
        {

        }
    }

    public partial class FormConfigConfiguration : EntityTypeConfiguration<MF_Base.Model.FormConfig>
    {
        public FormConfigConfiguration()
        {

        }
    }

    public partial class ListConfigConfiguration : EntityTypeConfiguration<MF_Base.Model.ListConfig>
    {
        public ListConfigConfiguration()
        {

        }
    }

    public partial class MF_UserConfiguration : EntityTypeConfiguration<MF_Base.Model.MF_User>
    {
        public MF_UserConfiguration()
        {

        }
    }

    #endregion
}