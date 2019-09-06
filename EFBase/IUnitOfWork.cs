using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFBase
{
    public interface IUnitOfWork : IDisposable
    {
        T Add<T>(T entity) where T : class;
        void AddRange<T>(IEnumerable<T> entities) where T : class;
        T GetByKey<T>(string key) where T : class;
        T GetSingle<T>(Expression<Func<T, bool>> conditions, params string[] includes) where T : class;
        T GetSingle<T, S>(Expression<Func<T, S>> orderBy, bool firstOrLast, Expression<Func<T, bool>> conditions = null) where T : class;
        IEnumerable<T> Get<T>(Expression<Func<T, bool>> conditions = null, params string[] includes) where T : class;
        IEnumerable<T> Get<T, S>(Expression<Func<T, S>> orderBy, bool firstOrLast, int count, Expression<Func<T, bool>> conditions = null) where T : class;
        IEnumerable<T> Get<T, S>(Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes) where T : class;
        IEnumerable<T> Get<T, S1, S2>(Expression<Func<T, S1>> orderBy, bool bUp, Expression<Func<T, S2>> thenBy, bool bThenUp, Expression<Func<T, bool>> conditions = null, params string[] includes) where T : class;
        IEnumerable<T> GetByPage<T, S>(out int totalCount, int pageSize, int pageIndex, Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes) where T : class;
        void DeleteByKey<T>(string key) where T : class;
        void Delete<T>(T entity) where T : class;
        void Delete<T>(IEnumerable<T> entities) where T : class;
        void Delete<T>(Expression<Func<T, bool>> predicate) where T : class;
        T UpdateEntity<T>(T entity) where T : class;
        void Update<T>(Expression<Func<T, T>> paramToChange, Expression<Func<T, bool>> conditions = null) where T : class;
        void Update<T>(T entity, params string[] nameOfPropertyToUpdate) where T : class;
        bool IsExist<T>(Expression<Func<T, bool>> conditions = null) where T : class;

        IQueryable<T> GetQuery<T>() where T : class;
        T SqlQuerySing<T>(string sql, params object[] paras) where T : class;
        List<T> SqlQuery<T>(string sql, params object[] paras) where T : class;
        IEnumerable<Dictionary<string, object>> DynamicListFromSql(string dbName, string Sql, Dictionary<string, object> Params = null);
        object DynamicFromSql(string dbName, string Sql, Dictionary<string, object> Params = null);

        bool Commit();
    }    
}
