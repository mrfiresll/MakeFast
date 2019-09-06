using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIBase;
using MFTool;
using MF_Base.Model;
using System.ComponentModel.Composition;
using System.Linq.Expressions;
using AutoUI.Areas.ConfigUI.Controllers;

namespace AutoUI.Areas.Sys.Controllers
{
    [Export]
    public class SQLDataSourceController : EntityFormController<SQLDataSource>
    {
        public JsonResult GetFields(string sqlDataSourceId)
        {
            var sqlDataSource = UnitOfWork.GetByKey<SQLDataSource>(sqlDataSourceId);
            sqlDataSource.CheckNotNull("Id 为 {0} 的数据源为空".ReplaceArg(sqlDataSourceId));
            return Json(sqlDataSource.GetFields().Select(a => new { text = a, value = a }));
        }

        public JsonResult GetFieldValueList(string sqlDataSourceId, string field) 
        {
            var sqlDataSource = UnitOfWork.GetByKey<SQLDataSource>(sqlDataSourceId);
            sqlDataSource.CheckNotNull("Id 为 {0} 的数据源为空".ReplaceArg(sqlDataSourceId));
            return Json(sqlDataSource.GetFieldValueList(field));
        }
    }
}