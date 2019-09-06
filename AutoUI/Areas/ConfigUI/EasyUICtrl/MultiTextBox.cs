using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MFTool;

namespace AutoUI.Areas.ConfigUI.EasyUICtrl
{
    public class MultiTextBox : CtrlBase
    {
        public MultiTextBox(string id, EasyUICtrlPrepareData pData)
            : base(id, pData)
        {
            if (_dataOption.Count == 0)
            {
                _dataOption = DefaultValue.MultiTextBoxJson().JsonToDictionary();
            }
        }

        public override void Prepare()
        {
            DomStr = "input";
            _dataOption.Add("multiline", "true");
            _class.Add("easyui-textbox");
        }
    }
}