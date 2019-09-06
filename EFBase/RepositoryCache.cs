using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MFTool;
using LinqKit;
using EntityFramework.Extensions;
using EntityFramework.Caching;
using System.Configuration;

namespace EFBase
{
    public class RepositoryCache<T> : Repository<T>, IRepository<T> where T : class
    {
        public RepositoryCache()
        {
            //this.context = new EFDbContext();
        }

        public RepositoryCache(DbContext cont)
        {
            context = cont;
        }

        private double SecondSpans()
        {
            object obj = ConfigurationManager.AppSettings["secondaryCacheSecondsSpan"];
            double res = 100;
            if(double.TryParse(obj.ToString(),out res))
            {
                return res;
            }
            return 5;            
        }

        public override T GetSingle(Expression<Func<T, bool>> conditions, params string[] includes)
        {
            IQueryable<T> queryList = context.Set<T>();//.AsNoTracking();//单一对象不需要关闭
            foreach (string path in includes)
            {
                queryList = queryList.Include(path);
            }

            try
            {
                if (conditions != null)
                {
                    queryList = queryList.AsExpandable().Where(conditions);
                }
                T t = queryList.Take(1).FromCacheFirstOrDefault();
                return t;
            }
            catch (Exception ex)
            {
                Logger.GetCurMethodLog().Error("操作失败", ex);
                throw ex;
            } 
        } 

        public override IEnumerable<T> Get<S>(Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes)
        {
            IQueryable<T> queryList = context.Set<T>().AsNoTracking();
            foreach (string path in includes)
            {
                queryList = queryList.Include(path);
            }

            if (conditions != null)
            {
                queryList = queryList.AsExpandable().Where(conditions);
            }

            if (orderBy != null)
            {
                queryList = bUp ? queryList.OrderBy(orderBy) : queryList.OrderByDescending(orderBy);
            }

            return queryList.FromCache().ToList();
        }

        public override IEnumerable<T> Get(Expression<Func<T, bool>> conditions = null, params string[] includes)
        {
            IQueryable<T> queryList = context.Set<T>().AsNoTracking();
            foreach (string path in includes)
            {
                queryList = queryList.Include(path);
            }

            if (conditions != null)
            {
                queryList = queryList.AsExpandable().Where(conditions);
            }
            return queryList.FromCache().ToList();
        }

        public override IEnumerable<T> Get<S1, S2>(Expression<Func<T, S1>> orderBy, bool bUp, Expression<Func<T, S2>> thenBy, bool bThenUp, Expression<Func<T, bool>> conditions = null, params string[] includes)
        {
            IQueryable<T> queryList = context.Set<T>().AsNoTracking();
            foreach (string path in includes)
            {
                queryList = queryList.Include(path);
            }

            if (conditions != null)
            {
                queryList = queryList.AsExpandable().Where(conditions);
            }

            var tmps = bUp ? queryList.OrderBy(orderBy) : queryList.OrderByDescending(orderBy);
            var result = bThenUp ? tmps.ThenBy(thenBy) : tmps.ThenByDescending(thenBy);
            return tmps.FromCache().ToList();
        }
    }
}
