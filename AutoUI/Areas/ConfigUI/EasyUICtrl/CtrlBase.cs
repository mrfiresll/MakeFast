using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MFTool;
using System.Text.RegularExpressions;
using MF_Base;

namespace AutoUI.Areas.ConfigUI.EasyUICtrl
{
    #region 控件定义的位置    

    public class EasyUICtrlPrepareData
    {
        public Dictionary<string, object> Style = new Dictionary<string,object>();
        public Dictionary<string, object> DataOptions  =new Dictionary<string,object>();
        public List<string> ClassNames = new List<string>();
        public Dictionary<string, object> Attr = new Dictionary<string, object>();
    }

    public class EasyUICtrlFactory
    {
        public static CtrlBase GetCtrl(string controlTypeName, string id, EasyUICtrlPrepareData prepareData)
        {
            CtrlBase ctrl = null;
            if (controlTypeName == ControlType.TextBox.ToString())
            {
                ctrl = new TextBox(id, prepareData);
            }
            else if (controlTypeName == ControlType.MultiTextBox.ToString())
            {
                ctrl = new MultiTextBox(id, prepareData);
            }
            else if (controlTypeName == ControlType.ComboBox.ToString())
            {
                ctrl = new ComboBox(id, prepareData);
            }
            else if (controlTypeName == ControlType.PopupSelector.ToString())
            {
                ctrl = new PopupSelector(id, prepareData);
            }
            else if (controlTypeName == ControlType.DateBox.ToString())
            {
                ctrl = new DateBox(id, prepareData);
            }
            else if (controlTypeName == ControlType.FileBox.ToString())
            {
                ctrl = new FileBox(id, prepareData);
            }
            else if (controlTypeName == ControlType.MultiFileBox.ToString())
            {
                ctrl = new MultiFileBox(id, prepareData);
            }
            else if (controlTypeName == ControlType.SubDataGrid.ToString())
            {
                ctrl = new SubDataGrid(id, prepareData);
            }
            else
            {
                return new CtrlBase(id, prepareData);
            }

            ctrl.Prepare();
            return ctrl;
        }
    }

    #endregion

    public class CtrlBase
    {
        public string Id { get; set; }
        protected List<string> _class = new List<string>();
        protected Dictionary<string, object> _dataOption = new Dictionary<string, object>();
        protected Dictionary<string, object> _style = new Dictionary<string, object>();
        protected Dictionary<string, object> _attr = new Dictionary<string, object>();

        protected string DomStr = "";
        protected string InnerHtm = "";

        public CtrlBase(string id,EasyUICtrlPrepareData pData)
        {
            Id = id;
            _dataOption = pData.DataOptions;
            _style = pData.Style;
            _class = pData.ClassNames;
            _attr = pData.Attr;
        }

        public virtual void Prepare()
        {
             
        }

        protected virtual void AfterDataOptionToJson(ref string dataOption)
        {

        }

        protected virtual void AfterGeneralHtml(ref string html)
        {
 
        }

        public virtual string GetScript()
        {
            return "";
        }

        public string GetCtrlHtm()
        {
            string classStr = "";
            foreach (var tmp in _class)
            {
                classStr += tmp + " ";
            }

            string dataOption = _dataOption.ToJson();
            AfterDataOptionToJson(ref dataOption);
            dataOption = JsonHelper.JsonRegex(dataOption)
                            .Replace("\"", "'")
                            .Replace("'true'", "true")
                            .Replace("'false'", "false").Trim();//除去bool类型的引号，否则easyui不识别
            if (!string.IsNullOrEmpty(dataOption))
            {
                if (dataOption[0] == '{') dataOption = dataOption.Substring(1, dataOption.Length - 1);
                if (dataOption[dataOption.Length - 1] == '}') dataOption = dataOption.Substring(0, dataOption.Length - 1);
            }

            string style = "";
            foreach (var item in _style)
            {
                style += string.Format("{0}:{1};", item.Key, item.Value);
            }
            string attr = "";
            foreach (var item in _attr)
            {
                if (item.Key.ToLower() == "id" 
                    || item.Key.ToLower() == "name" 
                    || item.Key.ToLower() == "class"
                    || item.Key.ToLower() == "data-options" 
                    || item.Key.ToLower() == "style"
                    || item.Key.ToLower() == "mfid"
                    || item.Key.ToLower() == "state"
                    || item.Key.ToLower() == "detail")
                    continue;

                attr += string.Format("{0}='{1}' ", item.Key, item.Value);
            }

            string ctrlHtml = string.Format("<{0} id='{1}' name='{1}' class='{2}' data-options=\"{3}\" style='{4}' {5}>{6}</{0}>",
                DomStr,
                Id,
                classStr,
                dataOption,
                style,
                attr,
                InnerHtm
                );
            ctrlHtml = ctrlHtml.Replace("\"true\"", "true").Replace("\"false\"", "false");//除去bool类型的双引号，否则easyui不识别
            if (_dataOption.GetValue("visible") == "false")
            {
                ctrlHtml = "<div style='height:26px'><div style='display:none;'>{0}</div></div>".ReplaceArg(ctrlHtml);
                _dataOption.Remove("visible");
            }
            AfterGeneralHtml(ref ctrlHtml);
            return ctrlHtml;
        }
    }
}