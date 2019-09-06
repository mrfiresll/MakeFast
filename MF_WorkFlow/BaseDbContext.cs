using EFBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MF_WorkFlow
{
    [Export(typeof(DbContext))]
    public partial class BaseDbContext : DbContext
    {
        public BaseDbContext()
            : base("WorkFlow")
        {
		    //true的时候改变查询对象的属性时,其EntityState会自动变成modified,此时savechange会进行数据更新
            //false则关闭该功能
            this.Configuration.AutoDetectChangesEnabled = true;
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }        
    }
}