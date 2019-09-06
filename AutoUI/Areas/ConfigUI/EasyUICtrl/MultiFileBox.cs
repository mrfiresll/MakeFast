using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MFTool;
using System.Text;

namespace AutoUI.Areas.ConfigUI.EasyUICtrl
{
    public class MultiFileBox : CtrlBase
    {
        public MultiFileBox(string id, EasyUICtrlPrepareData pData)
            : base(id, pData)
        {

        }
        public override void Prepare()
        {
            string maxFileLength = System.Configuration.ConfigurationManager.AppSettings["maxFileLength_KB"];
            if (!string.IsNullOrEmpty(maxFileLength))
            {
                _attr.Add("filemaxsize", maxFileLength);
            }

            if (string.IsNullOrEmpty(_dataOption.GetValue("fileextaccept")))
            {
                string fileTypeAccept = System.Configuration.ConfigurationManager.AppSettings["FileTypeAccept"];
                if (!string.IsNullOrEmpty(fileTypeAccept))
                {
                    _dataOption.Add("fileextaccept", fileTypeAccept);
                }
            }
            
            string fileStorePath = System.Configuration.ConfigurationManager.AppSettings["FileStorePath"];
            if (!string.IsNullOrEmpty(fileStorePath))
            {
                _attr.Add("downLoadUrl", fileStorePath);
            }

            DomStr = "ul";
            //_style
            _class.Add("easyui-datalist multiFileBox");
            //_attr.Add("lines", "true");
            _attr.Add("rownumbers", "true");
            _dataOption.Add("editorHeight", "31px");
            _dataOption.Add("textFormatter", "!-upLoadDataListRowRender-!");
            _dataOption.Add("onLoadSuccess", "!-mfUpLoadDataListOnLoad-!");
            _dataOption.Add("onLoadError", "!-mfUpLoadDataListOnLoad-!");
            InnerHtm = "<li value=\"ctrlRow\">点击上传文件...</li>";
        }

        protected override void AfterDataOptionToJson(ref string dataOption)
        {
            dataOption = dataOption.Replace("\"!-", "").Replace("-!\"", "");
        }

        protected override void AfterGeneralHtml(ref string html)
        {
            
        }
    }
}