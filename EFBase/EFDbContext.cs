using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFBase
{
    //[Export(typeof(DbContext))]
    public class EFDbContext : DbContext
    {
        public EFDbContext()
            : base("MyEF")
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EFDbContext>());
            //Database.SetInitializer<EFDbContext>(null);
            //Configuration.LazyLoadingEnabled = false;
        }

        public EFDbContext(DbConnection conn, bool contextOwnsConnection)
            : base(conn, contextOwnsConnection)
        {
            
        }

        ///// <summary>
        ///// 属性容器注入Configurations文件夹下的
        ///// [Export]
        ///// XXXXConfiguration
        ///// </summary>
        //[ImportMany(typeof(IEntityMapper))]
        //public IEnumerable<IEntityMapper> EntityMappers { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    //默认全局禁用级联删除
        //    modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

        //    //默认全局禁用级联删除
        //    //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        //    //禁用自动生成数据表末尾加s或者es
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //    //移除不必要的契约
        //    //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();

        //    if (EntityMappers == null)
        //    {
        //        return;
        //    }

        //    foreach (var mapper in EntityMappers)
        //    {
        //        mapper.RegistTo(modelBuilder.Configurations);
        //    }
        //}
    }
}
