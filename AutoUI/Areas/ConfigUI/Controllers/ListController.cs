using MF_Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoUI.Controllers;
using MFTool;
using MF_Base.Model;
using AutoUI.Areas.ConfigUI.EasyUICtrl;
using UIBase;
using System.Reflection;
using System.Data;

namespace AutoUI.Areas.ConfigUI.Controllers
{
    [Export]
    public class ListController : BaseController
    {
        public ActionResult PageView()
        {
            string code = QueryString("UICode");
            var list = UnitOfWork.GetSingle<ListConfig>(a => a.UICode == code);
            list.CheckNotNull("未找到列表");
            DataGrid dg = GetDataGrid(list);
            List<Button> btnList = GetButtonList(list);

            ViewBag.ListHtml = dg.GetCtrlHtm();
            ViewBag.Script = dg.GetScript() + "\n\r" + list.Script;
            ViewBag.ButtonHtml = string.Join("\n\r", btnList.Select(a => a.GetCtrlHtm()));
            ViewBag.QuickSearchColumTitle = string.Join("或", list.GetQuickSearchColumnTitleList());
            ViewBag.ListId = list.Id;
            return View();
        }

        public JsonResult GetList()
        {
            string code = QueryString("UICode");
            var list = UnitOfWork.GetSingle<ListConfig>(a => a.UICode == code);
            list.CheckNotNull("未找到列表");
            string sql = list.SQL;
            //快查过滤
            string quickSearchValue = QueryString("QuickSearchValue");
            if (!string.IsNullOrEmpty(quickSearchValue))
            {
                var quickSearchColumnFieldList = list.GetQuickSearchColumnFieldList();
                string tmpWhereStr = "";
                foreach (var field in quickSearchColumnFieldList)
                {
                    tmpWhereStr += string.Format(" or {0} like '%{1}%'", field, quickSearchValue);
                }

                if (!string.IsNullOrEmpty(tmpWhereStr))
                    sql = string.Format("select * from ({0} where 1=0 {1}) original", sql, tmpWhereStr);
            }

            //分页
            if (!string.IsNullOrEmpty(QueryString("page")) && !string.IsNullOrEmpty(QueryString("rows")))
            {
                sql = "select newid() as _id ,_tmpTable.* from ({0}) _tmpTable".ReplaceArg(sql);
                int pageId = Convert.ToInt32(QueryString("page"));
                int pageSize = Convert.ToInt32(QueryString("rows"));
                int startRowNumber = (pageId - 1) * pageSize;
                int endRowNumber = pageId * pageSize;
                string pageSql = string.Format("select * from (select  ROW_NUMBER() over(order by son._id) as rows, son.*  from ({0}) son) tmp where rows between {1} and {2}", sql, startRowNumber, endRowNumber);
                if (!string.IsNullOrEmpty(list.OrderBy))
                {
                    if (!list.OrderBy.ToLower().Contains("order") && !list.OrderBy.ToLower().Contains("by"))
                    {
                        pageSql += (" order by " + list.OrderBy);
                    }
                    else
                    {
                        pageSql += (" " + list.OrderBy);
                    }
                }
                else
                {
                    pageSql += " order by Id desc";
                }
                var res = UnitOfWork.DynamicListFromSql(list.DBName, pageSql);
                var totalCount = UnitOfWork.DynamicFromSql(list.DBName, string.Format("select count(*) FROM ({0})tmp", sql));
                return Json(new { rows = res, total = totalCount });
            }
            //不分页
            else
            {
                if (!string.IsNullOrEmpty(list.OrderBy))
                {
                    if (!list.OrderBy.ToLower().Contains("order") && !list.OrderBy.ToLower().Contains("by"))
                    {
                        sql += (" order by " + list.OrderBy);
                    }
                    else
                    {
                        sql += (" " + list.OrderBy);
                    }
                }

                var res = UnitOfWork.DynamicListFromSql(list.DBName, sql);
                return Json(res);
            }
        }

        public JsonResult Delete(string ids)
        {
            string code = QueryString("UICode");
            code.CheckNotNullOrEmpty("UICode");
            var listConfig = UnitOfWork.GetSingle<ListConfig>(a => a.UICode == code);
            listConfig.CheckNotNull("listConfig");
            ids.CheckNotNullOrEmpty("ids");
            var idArr = ids.JsonToObject<IEnumerable<string>>();

            string entityFullName = listConfig.EntityFullName;
            entityFullName.CheckNotNull("编号为{0}的列表定义未绑定实体类全名EntityFullName,无法执行删除".ReplaceArg(code));
            string projName = listConfig.DBName;
            Type entityType = ReflectionHelper.GetTypeBy(projName, entityFullName);

            var delMethod = typeof(EFBase.UnitOfWork).GetMethod("DeleteByKey", BindingFlags.Instance | BindingFlags.Public);
            delMethod.CheckNotNull("UnitOfWork中的DeleteByKey方法未找到或者不可访问");
            delMethod = delMethod.MakeGenericMethod(entityType);
            BeforeDelete(idArr);//
            foreach (var id in idArr)
            {
                delMethod.Invoke(UnitOfWork, new object[] { id });
            }
            return Json(UnitOfWork.Commit());
        }

        public virtual void BeforeDelete(IEnumerable<string> ids)
        {

        }

        public JsonResult ExportExcel()
        {
            string code = QueryString("UICode");
            code.CheckNotNullOrEmpty("UICode");
            var listConfig = UnitOfWork.GetSingle<ListConfig>(a => a.UICode == code);
            listConfig.CheckNotNull("listConfig");
            var colList = listConfig.GetColumnList();//列设置
            #region 数据库数据
            string sql = listConfig.SQL;
            if (!string.IsNullOrEmpty(listConfig.OrderBy))
            {
                if (!listConfig.OrderBy.ToLower().Contains("order") && !listConfig.OrderBy.ToLower().Contains("by"))
                {
                    sql += (" order by " + listConfig.OrderBy);
                }
                else
                {
                    sql += (" " + listConfig.OrderBy);
                }
            }
            var res = UnitOfWork.DynamicListFromSql(listConfig.DBName, sql);
            #endregion
            //fieldName替换为标题
            var dt = DictionaryExtend.ConvertDicToTable(res.ToList());
            foreach (DataColumn dataCol in dt.Columns)
            {
                var colItem = colList.FirstOrDefault(a => a.GetValue("field") == dataCol.ColumnName);
                if (colItem != null)
                {
                    dataCol.ColumnName = colItem.GetValue("title");
                }
            }
            NPOIExcelHelper.ExportByWeb(dt, listConfig.Name, listConfig.Name);
            return Json("");
        }

        private DataGrid GetDataGrid(ListConfig list)
        {
            var propertyDic = list.PropertySetting.JsonToDictionary();
            var style = new Dictionary<string, object>();
            var dataOptions = new Dictionary<string, object>();

            EasyUICtrlPrepareData pData = new EasyUICtrlPrepareData()
            {
                Style = style,
                DataOptions = dataOptions,
                Attr = propertyDic
            };

            List<Dictionary<string, object>> collist = list.ColumnSetting.JsonToDictionaryList();
            DataGrid dg = new DataGrid("mf_grid", pData, collist);
            dg.Prepare();
            return dg;
        }

        private List<Button> GetButtonList(ListConfig list)
        {
            List<Button> buttonList = new List<Button>();
            var buttonDicList = list.GetButtonList();
            foreach (var buttonDic in buttonDicList)
            {
                var detailDic = buttonDic.GetValue("Detail").JsonToDictionary();
                var attr = new Dictionary<string, object>();

                string title = buttonDic.GetValue("title");
                buttonDic.Remove("title");
                if (!string.IsNullOrEmpty(detailDic.GetValue("onclick")))
                {
                    if (!string.IsNullOrEmpty(detailDic.GetValue("mustSelect")))
                    {
                        string action = string.Format("checkSelection(\"{0}\",\"{1}\")", detailDic.GetValue("mustSelect"), detailDic.GetValue("onclick"));
                        buttonDic.SetValue("onclick", action);
                    }
                    else
                    {
                        buttonDic.SetValue("onclick", detailDic.GetValue("onclick"));
                    }
                }
                else if (!string.IsNullOrEmpty(buttonDic.GetValue("url")))
                {
                    string url = buttonDic.GetValue("url");
                    //高宽
                    string width = detailDic.GetValue("width");
                    string height = detailDic.GetValue("height");

                    if (!string.IsNullOrEmpty(detailDic.GetValue("mustSelect")))
                    {
                        string clickCmd = "\"openWindowWithUrl(\\\"" + url + "\\\",\\\"" + width + "\\\",\\\"" + height + "\\\")\"";
                        string action = string.Format("checkSelection(\"{0}\",{1})", detailDic.GetValue("mustSelect"), clickCmd);
                        buttonDic.SetValue("onclick", action);
                    }
                    else
                    {
                        string clickCmd = "openWindowWithUrl(\"" + url + "\")";
                        buttonDic.SetValue("onclick", clickCmd);
                    }
                    buttonDic.Remove("url");
                }

                var classNames = new List<string>();
                if (buttonDic.GetValue("hidden").ToLower() == "true")
                {
                    classNames.Add("euiCtrlHidden");
                }

                EasyUICtrlPrepareData pData = new EasyUICtrlPrepareData()
                {
                    Attr = buttonDic,
                    ClassNames = classNames
                };
                Button dg = new Button(buttonDic.GetValue("id"), pData, title);
                dg.Prepare();
                buttonList.Add(dg);
            }

            return buttonList;
        }
    }
}