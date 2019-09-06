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
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;

namespace EFBase
{
    /// <summary>
    /// 返回list的全是AsNoTracking,返回单一对象的是AutoDetectChange
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T>, IDisposable where T : class
    {
        [Import]
        protected DbContext context;

        public Repository()
        {

        }

        public DbContext GetContext()
        {
            return context;
        }

        public Repository(DbContext cont)
        {
            context = cont;
        }

        public T GetByKey(string key)
        {
            DbSet<T> dbSet = context.Set<T>();
            string str = context.Database.Connection.ConnectionString;
            T t = dbSet.Find(key);
            return t;
        }
        public virtual T GetSingle(Expression<Func<T, bool>> conditions,params string[] includes)
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
                T t = queryList.Take(1).FirstOrDefault();
                return t;
            }
            catch (Exception ex)
            {
                Logger.GetCurMethodLog().Error("操作失败", ex);
                throw ex;
            } 
        }
        public T GetSingle<S>(Expression<Func<T, S>> orderBy, bool firstOrLast, Expression<Func<T, bool>> conditions = null)
        {
            orderBy.CheckNotNull("orderBy");
            IQueryable<T> queryList = context.Set<T>();//.AsNoTracking();//单一对象不需要关闭     

            try
            {
                if (conditions != null)
                {
                    queryList = queryList.AsExpandable().Where(conditions);
                }
                T t = firstOrLast ? queryList.OrderBy(orderBy).FirstOrDefault() : queryList.OrderByDescending(orderBy).FirstOrDefault();
                return t;
            }
            catch(Exception ex)
            {
                Logger.GetCurMethodLog().Error("操作失败", ex);
                throw ex;
            }            
        }
        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> conditions = null, params string[] includes)
        {
            IQueryable<T> queryList = context.Set<T>().AsNoTracking();
            foreach (string path in includes)
            {
                queryList = queryList.Include(path);
            }

            try
            {
                if (conditions != null)
                {
                    queryList = queryList.AsExpandable().Where(conditions);//.OrderBy(a => a.OrderIndex);
                }

                return queryList.ToList();
            }
            catch (Exception ex)
            {
                Logger.GetCurMethodLog().Error("操作失败", ex);
                throw ex;
            }
        }

        public IEnumerable<T> Get<S>(Expression<Func<T, S>> orderBy, bool firstOrLast, int count, Expression<Func<T, bool>> conditions = null)
        {
            orderBy.CheckNotNull("orderBy");
            IQueryable<T> queryList = context.Set<T>().AsNoTracking();
            
            try
            {
                if (conditions != null)
                {
                    queryList = queryList.AsExpandable().Where(conditions);
                }

                return firstOrLast ? queryList.OrderBy(orderBy).Take(count).ToList() : queryList.OrderByDescending(orderBy).Take(count).ToList();
            }
            catch (Exception ex)
            {
                Logger.GetCurMethodLog().Error("操作失败", ex);
                throw ex;
            }
        }

        public virtual IEnumerable<T> Get<S>(Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes)
        {
            orderBy.CheckNotNull("orderBy");
            IQueryable<T> queryList = context.Set<T>().AsNoTracking();
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

                if (orderBy != null)
                {
                    queryList = bUp ? queryList.OrderBy(orderBy) : queryList.OrderByDescending(orderBy);
                }

                return queryList.ToList();
            }
            catch (Exception ex)
            {
                Logger.GetCurMethodLog().Error("操作失败", ex);
                throw ex;
            }            
        }

        public virtual IEnumerable<T> Get<S1, S2>(Expression<Func<T, S1>> orderBy, bool bUp, Expression<Func<T, S2>> thenBy, bool bThenUp, Expression<Func<T, bool>> conditions = null, params string[] includes)
        {
            orderBy.CheckNotNull("orderBy");
            IQueryable<T> queryList = context.Set<T>().AsNoTracking();
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

                var tmps = bUp ? queryList.OrderBy(orderBy) : queryList.OrderByDescending(orderBy);
                var result = bThenUp ? tmps.ThenBy(thenBy) : tmps.ThenByDescending(thenBy);

                return tmps.ToList();
            }
            catch (Exception ex)
            {
                Logger.GetCurMethodLog().Error("操作失败", ex);
                throw ex;
            }
        }

        public IEnumerable<T> GetByPage<S>(out int totalCount, int pageSize, int pageIndex, Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes)
        {
            IQueryable<T> queryList = context.Set<T>().AsNoTracking();
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

                //错误写法
                //foreach (string path in includes)
                //{
                //    queryList.Include(path);
                //}
                PagedList<T> tmps = queryList.ToPagedList<T, S>(pageIndex, pageSize, orderBy, bUp);
                totalCount = tmps.TotalItemCount;
                return tmps.Data;
            }
            catch (Exception ex)
            {
                Logger.GetCurMethodLog().Error("操作失败", ex);
                throw ex;
            }
        }

        public T Add(T entity)
        {
            entity.CheckNotNull("entity");
            var createTimeProperty = context.Entry<T>(entity).Property("CreateTime");
            if (createTimeProperty != null)
                createTimeProperty.CurrentValue = DateTime.Now;
            var modifyTimeProperty = context.Entry<T>(entity).Property("ModifyTime");
            if (modifyTimeProperty != null)
                modifyTimeProperty.CurrentValue = DateTime.Now;

            var orderIndexProperty = context.Entry<T>(entity).Property("OrderIndex");
            if (orderIndexProperty != null)
            {
                double maxOrderIndex = GetMaxOrderIndex();
                orderIndexProperty.CurrentValue = maxOrderIndex + 1;
            }
           
            //context.Entry<T>(entity).State = EntityState.Added;
            context.Set<T>().Add(entity);
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
             entities.CheckNotNull("entities");
             double maxOrderIndex = GetMaxOrderIndex() + 1;
             foreach (var entity in entities)
             {
                 var orderIndexProperty = context.Entry<T>(entity).Property("OrderIndex");
                 if (orderIndexProperty != null)
                 {
                     orderIndexProperty.CurrentValue = maxOrderIndex + 1;
                 }
                 maxOrderIndex++;
             }
             return context.Set<T>().AddRange(entities);            
        }
        public void DeleteByKey(string key)
        {            
            Delete(GetByKey(key));
        }

        public void Delete(T entity)
        {
            if (entity == null)
                return;

            DbSet<T> dbSet = context.Set<T>();
            
            //无法通过ef判断context already has the same primary key value,因此直接用try catch来判断
            try
            {
                if (context.Entry<T>(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }
            }
            catch
            {
                //context already has the same primary key value
                //强制移除原有的entity
                RemoveHoldingEntityInContext(entity);
                dbSet.Attach(entity);
            }

            context.Entry<T>(entity).State = EntityState.Deleted;
        }

        private Boolean RemoveHoldingEntityInContext(T entity)
        {
            var objContext = ((IObjectContextAdapter)context).ObjectContext;
            var objSet = objContext.CreateObjectSet<T>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);

            if (exists)
            {
                objContext.Detach(foundEntity);
            }

            return (exists);
        }

        public void Delete(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                Delete(entity);
            }
        }

        public void Delete(Expression<Func<T, bool>> predicate)
        {
            Delete(Get(predicate));
        }

        public T UpdateEntity(T entity)
        {
            entity.CheckNotNull("entity");
            context.Set<T>().Attach(entity);
            var createTimeProperty = context.Entry<T>(entity).Property("CreateTime");
            if (createTimeProperty != null)
                createTimeProperty.IsModified = false;
            var createUserIDProperty = context.Entry<T>(entity).Property("CreateUserID");
            if (createUserIDProperty != null)
                createUserIDProperty.IsModified = false;
            var createUserNameProperty = context.Entry<T>(entity).Property("CreateUserName");
            if (createUserNameProperty != null)
                createUserNameProperty.IsModified = false;

            var modifyProperty = context.Entry<T>(entity).Property("ModifyTime");
            if (modifyProperty != null)
                modifyProperty.CurrentValue = DateTime.Now;
            context.Entry<T>(entity).State = EntityState.Modified;
            return entity;
        }

        ///// <summary>
        ///// 修改实体对象
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public int UpdateModel(TEntity model, List<string> listRemoveField = null)
        //{

        //    // 排除不需更新的字段            
        //    foreach (string field in listRemoveField)
        //    {
        //        if (field != "Id")
        //            unDbContext.Entry(model).Property(field).IsModified = false;
        //    }

        //    if (unDbContext.Entry<TEntity>(model).State == System.Data.Entity.EntityState.Detached)
        //    {
        //        //将model追加到EF容器                
        //        unDbContext.Entry(model).State = EntityState.Modified;
        //    }
        //    return unDbContext.SaveChanges();
        //}

        public void Update(Expression<Func<T, T>> paramToChange, Expression<Func<T, bool>> conditions = null)
        {
            paramToChange.CheckNotNull("paramToChange");
            DbSet<T> dbSet = context.Set<T>();
            
            if (conditions != null)
            {
                dbSet.Where(conditions).Update(paramToChange);
            }
            else
            {
                dbSet.Update(paramToChange);
            }
        }

        public void Update(T entity, params string[] nameOfPropertyToUpdate)
        {
            entity.CheckNotNull("entity");
            if (context.Entry<T>(entity).State == EntityState.Detached)
            {
                context.Set<T>().Attach(entity);
            }

            var createTimeProperty = context.Entry<T>(entity).Property("CreateTime");
            if (createTimeProperty != null)
                createTimeProperty.IsModified = false;
            var createUserIDProperty = context.Entry<T>(entity).Property("CreateUserID");
            if (createUserIDProperty != null)
                createUserIDProperty.IsModified = false;
            var createUserNameProperty = context.Entry<T>(entity).Property("CreateUserName");
            if (createUserNameProperty != null)
                createUserNameProperty.IsModified = false;

            var modifyProperty = context.Entry<T>(entity).Property("ModifyTime");
            if (modifyProperty != null)
                modifyProperty.CurrentValue = DateTime.Now;

            IObjectContextAdapter objectContextAdapter = context;
            ObjectContext objectContext = objectContextAdapter.ObjectContext;
            ObjectStateEntry ose = objectContext.ObjectStateManager.GetObjectStateEntry(entity);
            foreach(var p in nameOfPropertyToUpdate)
            {
                ose.SetModifiedProperty(p);
            }
            
            if(context.Entry<T>(entity).State == EntityState.Unchanged)
            {
                context.Entry<T>(entity).State = EntityState.Modified;
            }
        }

        public bool IsExist(Expression<Func<T, bool>> conditions = null)
        {
            return Get(conditions).Any();
        }

        public IQueryable<T> R_GetQuery<S>(Expression<Func<T, S>> orderBy, bool bUp, Expression<Func<T, bool>> conditions = null, params string[] includes)
        {
            orderBy.CheckNotNull("orderBy");
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
            return queryList;
        }

        public IQueryable<T> GetQuery()
        {
            return context.Set<T>();
        }

        public T SqlQuerySing(string sql, params object[] paras)
        {
            T res = null;
            try
            {
                res = context.Database.SqlQuery<T>(sql).SingleOrDefault();
            }
            catch(Exception ex)
            {
                Logger.GetCurMethodLog().Write(EnumLogLevel.Error, "SqlQuerySing失败,sql:" + sql, ex);
                throw new Exception("SqlQuerySing失败,sql:" + sql, ex);
            }

            return res;
        }

        public List<T> SqlQuery(string sql, params object[] paras)
        {
            return context.Database.SqlQuery<T>(sql).ToList();
        }

        public IEnumerable<Dictionary<string, object>> DynamicListFromSql(string Sql, Dictionary<string, object> Params = null)
        {
            return context.DynamicListFromSql(Sql, Params);
        }

        public object DynamicFromSql(string Sql, Dictionary<string, object> Params = null)
        {
            return context.DynamicFromSql(Sql, Params);
        }

        public void Dispose()
        {
            if (context != null)
                context.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool Commit()
        {
            bool bExit = context.Database.Exists();
            Debug.Assert(bExit);
            int res = -1;
            try
            {
                res = context.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.GetCurMethodLog().Write(EnumLogLevel.Error, "SaveChanges失败", ex);
                throw new Exception("SaveChanges失败", ex);
            }

            return res >= 0;
        }

        private double GetMaxOrderIndex()
        {
            string tabName = typeof(T).Name;
            string sql = string.Format("select max({0}) from [{1}]", "OrderIndex", tabName);
            double maxOrderIndex = -1;
            try
            {
                maxOrderIndex = Convert.ToDouble(DynamicFromSql(sql));
            }
            catch (Exception ex) { Logger.GetCurMethodLog().Write(EnumLogLevel.Error, "GetMaxOrderIndex失败+sql:" + sql, ex); }
            return maxOrderIndex;
        }
    }
}
