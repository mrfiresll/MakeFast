using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

[assembly: WebActivator.PreApplicationStartMethod(typeof(UIBase.AppStart), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(UIBase.AppStart), "Stop")]

namespace UIBase
{
    /// <summary>
    /// 在应用程序启动时需要注册的服务
    /// </summary>
    public class AppStart
    {
        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(ScriptModule));
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            // TODO 是否需要关闭的服务，或者发送邮件
        }
    }
}
