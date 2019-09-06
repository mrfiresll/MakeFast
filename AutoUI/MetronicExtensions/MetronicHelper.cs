using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MFire.Web.MetronicExtensions
{
    public class MetronicHelper : HtmlHelper
    {
        /// <summary>
        /// 使用指定的视图上下文和视图数据容器来初始化 MetronicHelper 类的新实例。
        /// </summary>
        /// <param name="viewContext">视图上下文</param>
        /// <param name="viewDataContainer">视图数据容器</param>
        public MetronicHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
            : base(viewContext, viewDataContainer)
        { }

        /// <summary>
        /// 使用指定的视图上下文、视图数据容器和路由集合来初始化 MetronicHelper 类的新实例。
        /// </summary>
        /// <param name="viewContext">视图上下文</param>
        /// <param name="viewDataContainer">视图数据容器</param>
        /// <param name="routeCollection">路由集合</param>
        public MetronicHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection)
            : base(viewContext, viewDataContainer, routeCollection)
        { }
    }
}