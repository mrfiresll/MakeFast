using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    public class ReflectionHelper
    {
        public static IEnumerable<Type> GetTypes(string projName)
        {
            projName.CheckNotNullOrEmpty("projName");
            string modelDllPath = GetModelDllPath(projName);
            Assembly ab = Assembly.Load(File.ReadAllBytes(modelDllPath));
            var types = ab.GetLoadableTypes();
            return types;
        }

        public static Type GetTypeBy(string projName, string classFullName)
        {
            projName.CheckNotNullOrEmpty("projName");
            classFullName.CheckNotNullOrEmpty("classFullName");
            //string modelDllPath = GetModelDllPath(projName);
            //Assembly ab = Assembly.Load(File.ReadAllBytes(modelDllPath));
            //var types = ab.GetLoadableTypes();
            //return types.FirstOrDefault(a => a.FullName == classFullName);
            Type type = Type.GetType(classFullName + "," + projName);
            return type;
        }

        public static object CreateGeneric(Type generic, Type innerType)
        {
            Type specificType = generic.MakeGenericType(new System.Type[] { innerType });
            return Activator.CreateInstance(specificType, new object[] { });
        }

        public static object CreateGeneric(Type generic, string projName, string innerTypeFullName)
        {
            Type type = GetTypeBy(projName, innerTypeFullName);
            return CreateGeneric(generic, type);
        }

        private static string GetModelDllPath(string dbName)
        {
            string baseDirectory = System.Web.HttpContext.Current.Server.MapPath("/{0}/bin/Debug".ReplaceArg(dbName));
            string modelDllPath = baseDirectory + "\\" + dbName + ".dll";
            return modelDllPath;
        }
    }
}
