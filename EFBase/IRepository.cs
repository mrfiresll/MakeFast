using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFBase
{
    public interface IRepository<T> where T : class
    {
        T Add(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        T GetByKey(string key);
        T GetSingle(Expression<Func<T, bool>> conditions, params string[] includes);
        T GetSingle<S>(Expression<Func<T, S>> orderBy, bool firstOrLast, Expression<Func<T, bool>> conditions = null);
        IEnumerable<T> Get(Expression<Func<T, bool>> conditions = null, params string[] includes);
        IEnumerable<T> Get<S>(Expression<Func<T, S>> orderBy, bool firstOrLast, int count, Expression<Func<T, bool>> conditions = null);
        IEnumerable<T> Get<S>(Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes);
        IEnumerable<T> Get<S1, S2>(Expression<Func<T, S1>> orderBy, bool bUp, Expression<Func<T, S2>> thenBy, bool bThenUp, Expression<Func<T, bool>> conditions = null, params string[] includes);
        IEnumerable<T> GetByPage<S>(out int totalCount, int pageSize, int pageIndex, Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes);
        void DeleteByKey(string key);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        void Delete(Expression<Func<T, bool>> predicate);
        T UpdateEntity(T entity);
        void Update(Expression<Func<T, T>> paramToChange, Expression<Func<T, bool>> conditions = null);
        void Update(T entity, params string[] nameOfPropertyToUpdate);
        IQueryable<T> R_GetQuery<S>(Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes);
        bool IsExist(Expression<Func<T, bool>> conditions = null);
        IQueryable<T> GetQuery();
        T SqlQuerySing(string sql, params object[] paras);
        List<T> SqlQuery(string sql, params object[] paras);
        IEnumerable<Dictionary<string, object>> DynamicListFromSql(string Sql, Dictionary<string, object> Params = null);
        object DynamicFromSql(string Sql, Dictionary<string, object> Params = null);
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        bool Commit();
    }
}
