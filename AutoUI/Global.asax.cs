using NCode.Composition.DisposableParts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MF_Base;
using UIBase;
using System.Configuration;
using MFTool;
using EFBase;

namespace AutoUI
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ControllerBuilder.Current.SetControllerFactory(new MFControllerFactory());

            AggregateCatalog aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new DirectoryCatalog(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath));
            MefDependencyResolver res = new MefDependencyResolver(new DisposableWrapperCatalog(aggregateCatalog, true));
            DependencyResolver.SetResolver(res);

            //dbconfig DatabaseInitializer Initialize
            var dbNameList = WebConfigHelper.GetDBNames();
            foreach (var dbName in dbNameList)
            {
                string initializerName = "{0}.DatabaseInitializer".ReplaceArg(dbName);
                Type type = ReflectionHelper.GetTypeBy(dbName, initializerName);
                if (type == null) continue;
                type.InvokeMember("Initialize", System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Public, null, null,
                new object[] { });
            }
            
            //MiniProfilerEF6.Initialize();
            //MF_Base.DatabaseInitializer.Initialize();            
        }

        void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest()
        {
            //if (Request.IsLocal)
            //{
            //    MiniProfiler.Start();
            //}
        }
        protected void Application_EndRequest()
        {
            //MiniProfiler.Stop();
        }
    }
}