using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MFTool;

namespace AutoUI.Areas.ConfigUI.EasyUICtrl
{
    public class FileBox : CtrlBase
    {
        public FileBox(string id, EasyUICtrlPrepareData pData)
            : base(id, pData)
        {

        }
        public override void Prepare()
        {
            DomStr = "input";
            _class.Add("easyui-filebox");
            _dataOption.Add("buttonText", "选择文件");
            //_dataOption.Add("buttonIcon", "icon icon-upload");
        }
    }
}