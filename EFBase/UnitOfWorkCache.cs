using MFTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFBase
{
    /// <summary>
    /// UnitWork模式,不同模型的事物处理
    /// </summary>
    [Export("CacheVersion", typeof(IUnitOfWork))]
    public class UnitOfWorkCache : UnitOfWork, IUnitOfWork, IDisposable
    {
        protected override IRepository<T> GenericRepository<T>()
        {
            string assName = typeof(T).Assembly.GetName().Name;
            var dbContext = GetContext(assName);
            return new RepositoryCache<T>(dbContext);
        }
    }
}
