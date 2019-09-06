using MFTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace EFBase
{
    /// <summary>
    /// UnitWork模式,不同模型的事物处理
    /// </summary>
    [Export(typeof(IUnitOfWork))]
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        [ImportMany(typeof(DbContext))]
        protected List<DbContext> _dbContextList;

        //构造通用的Repository
        private IDictionary<Type, object> repositoryTable = new Dictionary<Type, object>();

        public void AddRepository<T>(Repository<T> repository) where T : class
        {
            var key = typeof(T);
            if (_dbContextList == null)
            {
                _dbContextList = new List<DbContext>();
            }

            if (!repositoryTable.ContainsKey(key))
            {
                _dbContextList.Add(repository.GetContext());
                repositoryTable.Add(key, repository);
            }
        }

        public T Add<T>(T entity) where T : class
        {
            return GetRepository<T>().Add(entity);
        }
        public void AddRange<T>(IEnumerable<T> entities) where T : class
        {
            GetRepository<T>().AddRange(entities);
        }
        public T GetByKey<T>(string key) where T : class
        {
            return GetRepository<T>().GetByKey(key);
        }
        public T GetSingle<T>(Expression<Func<T, bool>> conditions, params string[] includes) where T : class
        {
            return GetRepository<T>().GetSingle(conditions, includes);
        }
        public T GetSingle<T, S>(Expression<Func<T, S>> orderBy, bool firstOrLast, Expression<Func<T, bool>> conditions = null) where T : class
        {
            return GetRepository<T>().GetSingle(orderBy, firstOrLast, conditions);
        }
        public IEnumerable<T> Get<T>(Expression<Func<T, bool>> conditions = null, params string[] includes) where T : class
        {
            return GetRepository<T>().Get(conditions, includes);
        }
        public IEnumerable<T> Get<T, S>(Expression<Func<T, S>> orderBy, bool firstOrLast, int count, Expression<Func<T, bool>> conditions = null) where T : class
        {
            return GetRepository<T>().Get(orderBy, firstOrLast, count, conditions);
        }
        public IEnumerable<T> Get<T, S>(Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes) where T : class
        {
            return GetRepository<T>().Get(orderBy, bUp, conditions, includes);
        }
        public IEnumerable<T> Get<T, S1, S2>(Expression<Func<T, S1>> orderBy, bool bUp, Expression<Func<T, S2>> thenBy, bool bThenUp, Expression<Func<T, bool>> conditions = null, params string[] includes) where T : class
        {
            return GetRepository<T>().Get(orderBy, bUp, thenBy, bThenUp, conditions, includes);
        }
        public IEnumerable<T> GetByPage<T, S>(out int totalCount, int pageSize, int pageIndex, Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes) where T : class
        {
            return GetRepository<T>().GetByPage<S>(out totalCount, pageSize, pageIndex, orderBy, bUp, conditions, includes);
        }
        public void DeleteByKey<T>(string key) where T : class
        {
            GetRepository<T>().DeleteByKey(key);
        }
        public void Delete<T>(T entity) where T : class
        {
            GetRepository<T>().Delete(entity);
        }
        public void Delete<T>(IEnumerable<T> entities) where T : class
        {
            GetRepository<T>().Delete(entities);
        }
        public void Delete<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            GetRepository<T>().Delete(predicate);
        }
        public T UpdateEntity<T>(T entity) where T : class
        {
            return GetRepository<T>().UpdateEntity(entity);
        }
        public void Update<T>(Expression<Func<T, T>> paramToChange, Expression<Func<T, bool>> conditions = null) where T : class
        {
            GetRepository<T>().Update(paramToChange, conditions);
        }
        public void Update<T>(T entity, params string[] nameOfPropertyToUpdate) where T : class
        {
            GetRepository<T>().Update(entity, nameOfPropertyToUpdate);
        }
        public bool IsExist<T>(Expression<Func<T, bool>> conditions = null) where T : class
        {
            return GetRepository<T>().IsExist(conditions);
        }
        public IQueryable<T> GetQuery<T>() where T : class
        {
            return GetRepository<T>().GetQuery();
        }
        public T SqlQuerySing<T>(string sql, params object[] paras) where T : class
        {
            return GetRepository<T>().SqlQuerySing(sql, paras);
        }
        public List<T> SqlQuery<T>(string sql, params object[] paras) where T : class
        {
            return GetRepository<T>().SqlQuery(sql, paras);
        }
        public IEnumerable<Dictionary<string, object>> DynamicListFromSql(string dbName, string Sql, Dictionary<string, object> Params)
        {
            var dbContext = GetContext(dbName);
            return dbContext.DynamicListFromSql(Sql, Params);
        }
        public object DynamicFromSql(string dbName, string Sql, Dictionary<string, object> Params = null)
        {
            var dbContext = GetContext(dbName);
            return dbContext.DynamicFromSql(Sql, Params);
        }
        private IRepository<T> GetRepository<T>() where T : class
        {
            IRepository<T> repository = null;
            var key = typeof(T);

            if (repositoryTable.ContainsKey(key))
                repository = (IRepository<T>)repositoryTable[key];
            else
            {
                repository = GenericRepository<T>();
                repositoryTable.Add(key, repository);
            }

            return repository;
        }

        protected virtual IRepository<T> GenericRepository<T>() where T : class
        {
            string assName = typeof(T).Assembly.GetName().Name;
            var dbContext = GetContext(assName);
            return new Repository<T>(dbContext);
        }

        protected DbContext GetContext(string dbName)
        {
            var dbContext = _dbContextList.SingleOrDefault(a => a.Database.Connection.Database == dbName);
            dbContext.CheckNotNull("程序集" + dbName + "未定义DBContext或者未Export,请检查");
            return dbContext;
        }

        public bool Commit()
        {
            int res = -1;
            Action action = () =>
            {
                foreach (var _dbContext in _dbContextList)
                {
                    bool bExit = _dbContext.Database.Exists();
                    Debug.Assert(bExit);

                    try
                    {
                        res = _dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Logger.GetCurMethodLog().Write(EnumLogLevel.Error, _dbContext.Database.Connection.Database + "的SaveChanges失败", ex);
                        throw new Exception(_dbContext.Database.Connection.Database + "的SaveChanges失败", ex);
                    }
                }
            };
            if (System.Configuration.ConfigurationManager.AppSettings["UseMsdtc"].ToLower() == "true")
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    try
                    {
                        action();
                    }
                    catch (Exception)
                    {
                        Transaction.Current.Rollback();
                    }
                }
            }
            else
            {
                action();
            }

            return res >= 0;
        }
        public void Dispose()
        {
            foreach (var context in _dbContextList)
            {
                if (context != null)
                    context.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
