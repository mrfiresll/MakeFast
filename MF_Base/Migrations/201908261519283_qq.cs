namespace MF_Base
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class qq : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.MF_Role", "EnumRoleType", c => c.String(nullable: false));
            //AlterColumn("dbo.MF_MyLog", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_EnumDetail", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_Enum", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_MainType", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_Department", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_RoleUser", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_Role", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_User", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_UserForTest", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_UserForTest2", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_UserTT", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_UserForTest1", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.SQLDataSource", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_Func", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.MF_FuncRole", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.FormConfig", "OrderIndex", c => c.Double(nullable: false));
            AlterColumn("dbo.ListConfig", "OrderIndex", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ListConfig", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.FormConfig", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_FuncRole", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_Func", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.SQLDataSource", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_UserForTest1", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_UserTT", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_UserForTest2", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_UserForTest", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_User", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_Role", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_RoleUser", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_Department", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_MainType", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_Enum", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_EnumDetail", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("dbo.MF_MyLog", "OrderIndex", c => c.Int(nullable: false));
            DropColumn("dbo.MF_Role", "EnumRoleType");
        }
    }
}
