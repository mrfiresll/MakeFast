using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIBase;
using MFTool;
using System.ComponentModel.Composition;
using System.Linq.Expressions;
using DocumentIndex;
using System.Configuration;
using MF_Project.Model;
using AutoUI.Areas.ConfigUI.Controllers;

namespace Project.Areas.Document.Controllers
{
    [Export]
    public class MFDocumentController : FormController
    {
        protected override void BeforeAdd(Dictionary<string, object> dic)
        {
            string browsFile = dic.GetValue("BrowsFile");
            if (!string.IsNullOrEmpty(browsFile))
            {
                string fileStorePath = ConfigurationManager.AppSettings["FileStorePath"];
                var fileNameArr = browsFile.Split(',');
                foreach (var name in fileNameArr)
                {
                    string tmpPath = fileStorePath + name;
                    string physicalPath = Server.MapPath("/" + tmpPath);
                    IndexOperation opera = IndexOperation.GetAddOpera(physicalPath);
                    opera.Id = dic.GetValue("Id");
                    opera.Title = dic.GetValue("Name");
                    SearchIndexManager.GetInstance().AddOpreation(opera);
                }
            }
        }

        protected override void BeforeUpdate(Dictionary<string, object> dic)
        {
             
        }

        public JsonResult SearchByContent(string content)
        {
            int totalCount = 0;
            int pageId = Convert.ToInt32(QueryString("page"));
            int pageSize = Convert.ToInt32(QueryString("rows"));
            var resList = SearchIndexManager.GetInstance().SearchAll(content);
            var resIds = resList.Select(a => a.Id);
            var finalRes = UnitOfWork.GetByPage<MFDocument, DateTime>(out totalCount, pageSize, pageId, a => a.CreateTime, false, a => resIds.Contains(a.Id));
            return Json(finalRes);
        }
    }
}