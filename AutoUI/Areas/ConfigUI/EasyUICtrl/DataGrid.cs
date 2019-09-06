using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MFTool;
using UIBase;

namespace AutoUI.Areas.ConfigUI.EasyUICtrl
{
    public class DataGrid : CtrlBase
    {
        private List<Dictionary<string, object>> _colList = new List<Dictionary<string, object>>();
        public DataGrid(string id, EasyUICtrlPrepareData pData,List<Dictionary<string, object>> collist)
            : base(id, pData)
        {
            _colList = collist;
        }
        public override void Prepare()
        {
            DomStr = "table";
            //Attr.Add()
            _class.Add("easyui-datagrid");

            InnerHtm = "<thead><tr>";
            //多选
            if (!string.IsNullOrEmpty(_attr.GetValue("checkbox")) && _attr.GetValue("checkbox") == "true")
            {

                InnerHtm += "<th data-options=\"field:'ck',checkbox:true\"></th>";
                _attr.Remove("checkbox");
            }
            
            foreach (var colDic in _colList)
            {
                var th = "<th ";
                foreach (var p in colDic)
                {
                    if (p.Key == "title" || p.Key == "state" || p.Key == "mfid" || p.Key.ToLower() == "detail")
                        continue;

                    if(p.Key == "hidden" && p.Value.ToString() == "false")
                    {
                        continue;
                    }

                    th += string.Format("{0}='{1}' ", p.Key, p.Value);
                }
                th += " halign='center' ";
                th += string.Format(">{0}</th>", colDic.GetValue("title"));
                InnerHtm += th;
            }
            InnerHtm += "</tr></thead>";
        }

        public override string GetScript()
        {
            string res = "";
            foreach (var col in _colList)
            {
                string detail = col.GetValue("Detail");
                if (!string.IsNullOrEmpty(detail))
                {
                    string colScript = "";
                    var detailDic = detail.JsonToDictionary();
                    string dataType = detailDic.GetValue("SourceType");//SourceType与SourceText,参考ColumnDetail.cshtml
                    string dataSource = detailDic.GetValue("SourceValue");
                    if (dataType == "MF_Enum")//来自于库枚举
                    {
                        string catCode = detailDic.GetValue("SourceValue");
                        var enumDic = CommonHelper.GetDBEnum(catCode);
                        if (enumDic != null)
                            dataSource = enumDic.ToJson();
                    }

                    string dateFormat = detailDic.GetValue("dateFormat");//日期格式
                    if (!string.IsNullOrEmpty(dateFormat))//日期格式优先
                    {
                        colScript += string.Format("    addGridDate('{0}','{1}','{2}')", "mf_grid", col.GetValue("field"), dateFormat);
                        res += colScript;
                        res += "\r\n";
                    }
                    else if (!string.IsNullOrEmpty(dataSource))//有数据源则必须生成formatter,名称固定(DataGrid.cs)
                    {
                        string dataName = col.GetValue("field") + "Source";
                        colScript = string.Format("var {0} = {1};", dataName, dataSource);
                        colScript += "\r\n";
                        colScript += string.Format("    addGridEnum('{0}','{1}',{2})", "mf_grid", col.GetValue("field"), dataName);
                        res += colScript;
                        res += "\r\n";
                    }

                }
            }

            return res;
        }
    }
}