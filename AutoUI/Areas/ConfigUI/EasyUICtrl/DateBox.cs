using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MFTool;

namespace AutoUI.Areas.ConfigUI.EasyUICtrl
{
    public class DateBox : CtrlBase
    {
        public DateBox(string id, EasyUICtrlPrepareData pData)
            : base(id, pData)
        {

        }
        public override void Prepare()
        {
            DomStr = "input";
            _class.Add("easyui-datebox");
        }
    }
}