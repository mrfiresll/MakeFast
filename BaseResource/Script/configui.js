var formData = {};
function pageLoad() {
    if (typeof(id) == "undefined")
    {
        return;
    }

    var tmpId = id;
    //编辑
    if (tmpId) {
        $("#Id").val(tmpId);
        addAjaxParam("formId", formId);
        addAjaxParam("Id", tmpId);
        commitAjax("Get", {
            CallBack: function (data) {
                if (data) {
                    formData = data;
                    setFormData(data);
                }
            }
        });
    }
}

//弹出选择控件的通用打开窗口的方法
function popupSelectorOpenWindow(selectorId, url, windowSettings) {
    //大括号参数替换为值
    if (url) {
        var t = $('form').serializeArray();
        $.each(t, function () {
            url = url.replace('{' + this.name + '}', this.value);
        });
    }
    windowSettings.onDestroy = function (data) {
        if (data) {
            $('#' + selectorId).textbox('setValue', data[windowSettings['value'] || ''] || '');//先value
            $('#' + selectorId).textbox('setText', data[windowSettings['text'] || ''] || '');//后text 否则全为value
            if (windowSettings['returnVal']) {
                var keyValuePairs = windowSettings['returnVal'].split(',');
                var tmpFormData = {};
                $.each(keyValuePairs, function (index, item) {
                    var keyValuePair = item.split('=');
                    if (keyValuePair.length == 2) {
                        var tmp = eval('({' + keyValuePair[0] + ':\'' + data[keyValuePair[1]] + '\'})');
                        tmpFormData = $.extend(true, {}, tmpFormData, tmp);
                    }
                })
                setFormData(tmpFormData);
            }
        }
    }
    openWindow(url, windowSettings);
}

//保存界面数据
function save() {
    var isValid = $('form').form('validate');
    $('form').find("table[class*='easyui-datagrid']").each(function () {
        if (!$(this).datagrid('mfValidate'))
        {
            isValid = false;
            return false;
        }
    })

    if (!isValid) {
        msgBox('请检查所填数据!');
        return;
    }

    var fData = jQuery.extend(true, {}, formData, getFormData($('form')));

    addAjaxParam("formId", formId);
    addAjaxParam("formData", fData);
    commitAjax("Save", {
        CallBack: function (data) {
            if (data) {
                msgBox('操作成功', function () {
                    closeWindow(true);
                });
            }
            else {
                msgBox('操作失败');
            }
        }
    })
}