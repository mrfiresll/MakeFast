using EFBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoUI.Controllers
{
    [Export]
    public class TestController : Controller
    {
        public ActionResult Index()
        {
         
            return View();
        }
    }
}
