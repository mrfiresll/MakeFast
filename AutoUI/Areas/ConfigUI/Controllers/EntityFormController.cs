using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoUI.Areas.ConfigUI.Controllers
{
    /// <summary>
    /// 子视图需自定义视图
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Export]
    public class EntityFormController<T> : FormController where T: class
    {
        protected override Type EntityType
        {
            get
            {
                return typeof(T);
            }
        }

        public override ActionResult PageView(string UICode)
        {
            return View();
        }
    }
}
