using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace UIBase
{
    public class MFControllerFactory : DefaultControllerFactory
    {
        protected override Type GetControllerType(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
           
            return base.GetControllerType(requestContext, controllerName);
        }
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            //Console.WriteLine(string.Format("{0}is create.", controllerType.Name));
            return base.GetControllerInstance(requestContext, controllerType);
        }
    }
}