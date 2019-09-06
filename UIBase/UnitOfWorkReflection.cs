using MFTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UIBase
{
    //无用，仅做反射参考
    public class UnitOfWorkReflection
    {
        private object _unitOfWorkInst;

        public UnitOfWorkReflection()
        {
            //创建unitofwork实例
            _unitOfWorkInst = Activator.CreateInstance(typeof(EFBase.UnitOfWork));            
        }

        public object Add(object entity)
        {
            CheckAndAddRepository(entity.GetType());
            var addMethod = typeof(EFBase.UnitOfWork).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            addMethod.CheckNotNull("UnitOfWork中的Add方法未找到或者不可访问");
            addMethod = addMethod.MakeGenericMethod(entity.GetType());
            var newObj = addMethod.Invoke(_unitOfWorkInst, new object[] { entity });
            return newObj;
        }

        public object Get(Type type, string key)
        {
            var getMethod = typeof(EFBase.UnitOfWork).GetMethod("GetByKey", BindingFlags.Instance | BindingFlags.Public);
            getMethod.CheckNotNull("UnitOfWork中的GetByKey方法未找到或者不可访问");
            getMethod = getMethod.MakeGenericMethod(type.GetType());
            var newObj = getMethod.Invoke(_unitOfWorkInst, new object[] { key });
            return newObj;
        }

        public object Delete(Type type, string key)
        {
            var delMethod = typeof(EFBase.UnitOfWork).GetMethod("Delete", BindingFlags.Instance | BindingFlags.Public);
            delMethod.CheckNotNull("UnitOfWork中的Delete方法未找到或者不可访问");
            delMethod = delMethod.MakeGenericMethod(type.GetType());
            var newObj = delMethod.Invoke(_unitOfWorkInst, new object[] { key });
            return newObj;
        }

        public object Update(object entity)
        {
            var addMethod = typeof(EFBase.UnitOfWork).GetMethod("UpdateEntity", BindingFlags.Instance | BindingFlags.Public);
            addMethod.CheckNotNull("UnitOfWork中的Add方法未找到或者不可访问");
            addMethod = addMethod.MakeGenericMethod(entity.GetType());
            var newObj = addMethod.Invoke(_unitOfWorkInst, new object[] { entity });
            return newObj;
        }

        public bool Commit()
        {
            var commitMethod = typeof(EFBase.UnitOfWork).GetMethod("Commit", BindingFlags.Instance | BindingFlags.Public);
            commitMethod.CheckNotNull("UnitOfWork中的Commit方法未找到或者不可访问");
            var bSuccess = commitMethod.Invoke(_unitOfWorkInst, new object[] { });
            return (bool)bSuccess;
        }

        private void CheckAndAddRepository(Type entityType)
        {
            var getRepositoryMethod = typeof(EFBase.UnitOfWork).GetMethod("GetRepository", BindingFlags.Instance | BindingFlags.Public);
            getRepositoryMethod.CheckNotNull("UnitOfWork中的GetRepository方法未找到或者不可访问");
            getRepositoryMethod = getRepositoryMethod.MakeGenericMethod(entityType);
            var repository = getRepositoryMethod.Invoke(_unitOfWorkInst, new object[] { });
            //如果为空则add
            if(repository == null)
            {
                string projName = entityType.Assembly.GetName().Name;
                //repository实例
                Type repositoryType = ReflectionHelper.GetTypeBy(projName, projName + ".Repository`1");
                repositoryType.CheckNotNull(string.Format("工程名{0},类全名{1},无法找到类型。", projName, projName + ".Repository`1"));
                repositoryType = repositoryType.MakeGenericType(entityType);
                object repositoryInst = Activator.CreateInstance(repositoryType);
                //在unitofwork中加入repository
                var addRepositoryMethod = typeof(EFBase.UnitOfWork).GetMethod("AddRepository", BindingFlags.Instance | BindingFlags.Public);
                addRepositoryMethod.CheckNotNull("UnitOfWork中的AddRepository方法未找到或者不可访问");
                addRepositoryMethod = addRepositoryMethod.MakeGenericMethod(entityType);
                addRepositoryMethod.Invoke(_unitOfWorkInst, new object[] { repositoryInst });
            }
        }
    }
}
