using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MFTool;
using MF_Base;

namespace AutoUI.Areas.ConfigUI.EasyUICtrl
{
    public class SubDataGrid : CtrlBase
    {
        private const string _toolBarHtm = @"<div id='toolBar{0}'>
            <table cellpadding='0' cellspacing='0' style='width: 100%'>
                <tr>
                    <td>
                        <a href='#' class='easyui-linkbutton' onclick=""javascript: $('#{0}').datagrid('mfInsertRow')"" iconcls='iconfont iconfont-jia1' plain='true'>增加</a>
                        <a href='#' class='easyui-linkbutton' onclick=""javascript: $('#{0}').datagrid('mfDeleteRow')"" iconcls='iconfont iconfont-shanchu' plain='true'>删除</a>
                        <a href='#' class='easyui-linkbutton' onclick=""javascript: $('#{0}').datagrid('mfMoveUp')"" iconcls='iconfont iconfont-xiangshang' plain='true'>上移</a>
                        <a href='#' class='easyui-linkbutton' onclick=""javascript: $('#{0}').datagrid('mfMoveDown')"" iconcls='iconfont iconfont-paixu' plain='true'>下移</a>
                    </td>
                </tr>
            </table>
        </div>";

        public SubDataGrid(string id, EasyUICtrlPrepareData pData)
            : base(id, pData)
        {
            //_colList = collist;
        }
        public override void Prepare()
        {
            DomStr = "table";
            //Attr.Add()
            _class.Add("easyui-datagrid");
            if (_dataOption.GetValue("showToolBar") == "true")
            {
                _attr.SetValue("toolbar", "#toolBar" + Id);
            }

            _dataOption.GetValue("showToolBar");//是否显示操作栏
            _attr.SetValue("width", "100%");

            InnerHtm = "<thead><tr>";
            if (!string.IsNullOrEmpty(_dataOption.GetValue("checkbox")) && _dataOption.GetValue("checkbox") == "true")
            {
                InnerHtm += "<th data-options=\"field:'ck',checkbox:true\"></th>";
                _attr.Remove("checkbox");
            }

            string columnSetting = _dataOption.GetValue("ColumnSetting");
            if (!string.IsNullOrEmpty(columnSetting))
            {
                List<Dictionary<string, object>> collist = columnSetting.JsonToDictionaryList();
                foreach (var colDic in collist)
                {
                    if (colDic.GetValue("IsVisible") == "false")
                        continue;
                    colDic.Remove("IsVisible");

                    var th = "<th ";

                    colDic.GetValue("FieldType");
                    colDic.Remove("FieldType");

                    if (colDic.GetValue("Enable") == "是")
                    {
                        string controlType = colDic.GetValue("CtrlType");
                        if (!string.IsNullOrEmpty(controlType))
                        {
                            colDic.GetValue("Detail");
                            th += string.Format("editor=\"{0}\" ", GetEditorHtm(controlType, colDic.GetValue("Detail")));
                        }
                    }
                    colDic.Remove("Enable");
                    colDic.Remove("Detail");
                    colDic.Remove("CtrlType");

                    colDic.GetValue("FieldName");
                    th += string.Format("field='{0}' ", colDic.GetValue("FieldName"));
                    colDic.Remove("FieldName");

                    foreach (var p in colDic)
                    {
                        th += string.Format("{0}='{1}' ", p.Key, p.Value);
                    }
                    th += " halign='center' ";
                    th += string.Format(">{0}</th>", colDic.GetValue("ColumnName"));
                    colDic.Remove("ColumnName");
                    InnerHtm += th;
                }
                InnerHtm += "</tr></thead>";
            }
            _dataOption.Remove("ColumnSetting");
        }

        protected override void AfterGeneralHtml(ref string html)
        {
            if (_dataOption.GetValue("showToolBar") == "true")
                html = _toolBarHtm.ReplaceArg(Id) + html;
        }

        public override string GetScript()
        {
            string script = "$('#{0}').datagrid().datagrid('enableCellEditing');".ReplaceArg(Id);
            return script;
        }

        private string GetEditorHtm(string controlType, string detail)
        {
            var dic = detail.JsonToDictionary();
            string ctrlType = "";
            if (controlType == GridControlType.TextBox.ToString())
            {
                ctrlType = "validatebox";
            }
            else if (controlType == GridControlType.DateBox.ToString())
            {
                ctrlType = "datebox";
            }
            else if (controlType == GridControlType.MultiTextBox.ToString())
            {
                ctrlType = "textarea";
            }
            else if (controlType == ControlType.ComboBox.ToString())
            {
                ctrlType = "combobox";
            }

            string opt = "";
            foreach (var item in dic)
            {
                if (item.Value != null
                    && !string.IsNullOrEmpty(item.Value.ToString())
                    && !item.Key.ToString().Contains("style"))
                {
                    opt += string.Format("{0}:'{1}',", item.Key, item.Value);
                }
            }
            opt = opt.Replace("'true'", "true")
                     .Replace("'false'", "false").Trim();//除去bool类型的引号，否则easyui不识别
            return "{{ type: '{0}',options:{{{1}}} }}".ReplaceArg(ctrlType, opt);
        }
    }
}