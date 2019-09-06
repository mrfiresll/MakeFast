using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MFTool;

namespace AutoUI.Areas.ConfigUI.EasyUICtrl
{
    public class Button : CtrlBase
    {
        private string _title;
        public Button(string id, EasyUICtrlPrepareData pData, string title)
            : base(id, pData)
        {
            _title = title;
        }

        public override void Prepare()
        {
            DomStr = "a";
            _attr.Add("plain", "true");
            _attr.SetValue("iconcls", "iconfont " + _attr.GetValue("iconcls"));
            _attr.Add("href", "#");

            InnerHtm = _title;

            _class.Add("easyui-linkbutton");
        }
    }
}