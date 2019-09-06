using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MFTool;
using UIBase;

namespace AutoUI.Areas.ConfigUI.EasyUICtrl
{
    public class ComboBox : CtrlBase
    {
        public ComboBox(string id, EasyUICtrlPrepareData pData)
            : base(id, pData)
        {

        }

        public override void Prepare()
        {
            DomStr = "select";
            _class.Add("easyui-combobox");
            _attr.Add("panelheight", "auto");
            string sourceType = _dataOption.GetValue("SourceType");
            if (sourceType == "MF_Enum")
            {
                string dataJson = CommonHelper.GetDBEnum(_dataOption.GetValue("SourceValue")).ToJson();
                _dataOption.SetValue("data", dataJson);
            }
            else
            {
                _dataOption.SetValue("data", _dataOption.GetValue("SourceValue"));
            }
            _dataOption.Remove("SourceType");
            _dataOption.Remove("SourceValue");
        }

        protected override void AfterDataOptionToJson(ref string dataOption)
        {
            dataOption = dataOption.Replace("\"[", "[").Replace("]\"", "]").Replace("\\", "");
        }

        public override string GetScript()
        {
            return "";
        }
    }
}