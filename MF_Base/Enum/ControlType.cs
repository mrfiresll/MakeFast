using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF_Base
{
    public enum ControlType
    {
        [Description("文本框")]
        TextBox = 1,
        [Description("多行文本框")]
        MultiTextBox,
        [Description("下拉列表框")]
        ComboBox,
        [Description("弹出选择框")]
        PopupSelector,
        [Description("日期选择框")]
        DateBox,
        [Description("附件上传框")]
        FileBox,
        [Description("多附件上传框")]
        MultiFileBox,
        [Description("子表")]
        SubDataGrid
    }

    public enum GridControlType
    {
        [Description("文本框")]
        TextBox = 1,
        [Description("多行文本框")]
        MultiTextBox,
        [Description("下拉列表框")]
        ComboBox,
        //[Description("弹出选择框")]
        //PopupSelector,
        [Description("日期选择框")]
        DateBox,
        [Description("复选框")]
        CheckBox
    }
}
