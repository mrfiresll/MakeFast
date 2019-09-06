using EFBase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF_Project
{
    /// <summary>
    /// 要启用数据库迁移，在global调用DatabaseInitializer.Initialize()
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// 数据库初始化引用Configuration
        /// </summary>
        public static void Initialize()
        {
            bool enable = Convert.ToBoolean(ConfigurationManager.AppSettings["CodeFirstEnabled"]);
            if (enable)
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<BaseDbContext, Configuration>());
            }
            else
            {
                //关闭数据迁移
                Database.SetInitializer<BaseDbContext>(null);
            } 
        }
    }
}
