using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MFTool;

namespace AutoUI.Areas.ConfigUI.EasyUICtrl
{
    public class PopupSelector : CtrlBase
    {
        public PopupSelector(string id, EasyUICtrlPrepareData pData)
            : base(id, pData)
        {

        }
        public override void Prepare()
        {
            DomStr = "input";
            _class.Add("easyui-mfbuttonedit");
            string url = _dataOption.GetValue("url");
            string width =  _dataOption.GetValue("width");
            string height =  _dataOption.GetValue("height");
            string text = _dataOption.GetValue("text");
            string value = _dataOption.GetValue("value");
            string returnVal = _dataOption.GetValue("returnVal");

            string setting = "{ width:'" + width + 
                "',height:'" + height +
                "',text:'" + text + 
                "',value:'" + value +
                "',returnVal:'" + returnVal + 
                "' }";

            string openWindow = "!-function(e) {  popupSelectorOpenWindow(e.id,'" + url + "'," + setting + ")  }-!";
            _dataOption.Add("onButtonClick", openWindow);
            _dataOption.Remove("url");
            _dataOption.Remove("width");
            _dataOption.Remove("height");
            _dataOption.Remove("text");
            _dataOption.Remove("value");
            _dataOption.Remove("returnVal");
        }

        protected override void AfterDataOptionToJson(ref string dataOption)
        {
            dataOption = dataOption.Replace("\"!-", "").Replace("-!\"", "");
        }
    }
}