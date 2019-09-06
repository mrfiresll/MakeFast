using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Remoting;
using MFTool;
using Newtonsoft.Json;
using BaseConfig;

namespace WebBase.Areas.MVCTemplate.Controllers
{
    [Export]
    public class FormController : Controller
    {
        [Import]
        IFormConfigRepository _repository;

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Add(string formBaseId, string data)
        {
            formBaseId.CheckNotNullOrEmpty("formBaseId");
            var formBase = _repository.R_Get(formBaseId);
            formBase.CheckNotNull("formBase");

            data.CheckNotNullOrEmpty("data");

            data = Server.UrlDecode(data);

            string entityNameSpace = formBase.EntityNameSpace;
            string entityFullName = formBase.EntityFullName;
            Type type = ReflectionHelper.GetTypeBy(entityNameSpace, entityFullName);
            var entity = JsonConvert.DeserializeObject(data, type, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
            object entityHelper = ReflectionHelper.CreateGeneric(typeof(BaseRepository<>), type);
            Type entityType = entityHelper.GetType();
            BeforeAdd(entityType.GetMethod("GetContext").Invoke(entityHelper, new object[] { }));
            var newObj = entityType.GetMethod("R_Add").Invoke(entityHelper, new[] { entity });
            var bSuccess = entityType.GetMethod("Commit").Invoke(entityHelper, new object[] { });
            if ((bool)bSuccess)
            {
                return Json(newObj);
            }
            return Json("");
        }

        public virtual void BeforeAdd(object context)
        {

        }

        public JsonResult Delete(string formBaseId, string dataId)
        {
            formBaseId.CheckNotNullOrEmpty("formBaseId");
            var formBase = _repository.R_Get(formBaseId);
            formBase.CheckNotNull("formBase");

            dataId.CheckNotNullOrEmpty("guid");

            string entityNameSpace = formBase.EntityNameSpace;
            string entityFullName = formBase.EntityFullName;
            object entityHelper = ReflectionHelper.CreateGeneric(typeof(BaseRepository<>), entityNameSpace, entityFullName);
            Type entityType = entityHelper.GetType();
            BeforeDelete(entityType.GetMethod("GetContext").Invoke(entityHelper, new object[] { }));
            entityType.GetMethod("R_Delete").Invoke(entityHelper, new object[] { dataId });
            var bSuccess = entityType.GetMethod("Commit").Invoke(entityHelper, new object[] { });
            return Json((bool)bSuccess);
        }

        public virtual void BeforeDelete(object context)
        {

        }

        public JsonResult Update(string formBaseId, string data)
        {
            formBaseId.CheckNotNullOrEmpty("formBaseId");
            var formBase = _repository.R_Get(formBaseId);
            formBase.CheckNotNull("formBase");
            
            data.CheckNotNullOrEmpty("data");

            string entityNameSpace = formBase.EntityNameSpace;
            string entityFullName = formBase.EntityFullName;
            Type type = ReflectionHelper.GetTypeBy(entityNameSpace, entityFullName);
            var entity = JsonConvert.DeserializeObject(data, type, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore });
            object entityHelper = ReflectionHelper.CreateGeneric(typeof(BaseRepository<>), type);
            Type entityType = entityHelper.GetType();
            BeforeUpdate(entityType.GetMethod("GetContext").Invoke(entityHelper, new object[] { }));
            entityType.GetMethod("R_Update").Invoke(entityHelper, new[] { entity });
            var bSuccess = entityType.GetMethod("Commit").Invoke(entityHelper, new object[] { });
            return Json((bool)bSuccess);
        }

        public virtual void BeforeUpdate(object context)
        {
 
        }
    }
}
