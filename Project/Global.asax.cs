using DocumentIndex;
using NCode.Composition.DisposableParts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UIBase;

namespace Project
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
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ControllerBuilder.Current.SetControllerFactory(new MFControllerFactory());

            DirectoryCatalog catalog = new DirectoryCatalog(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath);
            MefDependencyResolver res = new MefDependencyResolver(new DisposableWrapperCatalog(catalog, true));
            DependencyResolver.SetResolver(res);

            MF_Project.DatabaseInitializer.Initialize();
            string fileIndexPath = ConfigurationManager.AppSettings["FileIndexPath"];
            string physicalPath = Server.MapPath("/" + fileIndexPath);
            SearchIndexManager.GetInstance().StartThread(physicalPath);
        }
    }
}