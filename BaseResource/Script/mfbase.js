//基本参数配置
var baseParamSettings = {
    key: "",                     //配置的key,用户从配置数组中查找
    formId: "mf_form",
    gridId: "mf_grid",         //执行关联的gridId
    //treeId: "dataTree",         //执行关联的TreeId
    //queryFormId: "queryForm",
    //queryWindowId: "queryWindow",
    //queryBoxId: "key",
    //queryTreeBoxId: "treeKey",
    //refresh: true,                  //是否刷新  
    //paramFrom: "",          //action或url从哪个控件获取参数值
    //validateForm: true,     //验证表单输入
};

//窗口参数配置，继承自基本参数配置和操作单数配置
var windowParamSettings = {
    url: "",
    allowResize: true,
    onDestroy: null,           //销毁时调用
    onLoad: null,              //加载完成时调用页面方法参数为contentWindow
    data: null,
    title: null,                 //窗口标题
    width: '80%',                //窗口宽度
    height: '80%',               //窗口高度
    addQueryString: true,
    showMaxButton: true,
    funcType: "",              //地址栏FuncType
};

var gridLinkParamSettings = {
    onFilter: null,     //过滤条件函数
    refresh: false,     //关闭后是否刷新grid   
    linkText: "",       //连接文本，默认为当前字段值
    isButton: false,    //是否显示为按钮
    paramField: "Id",   //执行参数字段 
    url: "",             //窗口Url
    enumSourceVarName: null       //枚举数据源变量名
};

gridLinkParamSettings = jQuery.extend(true, {}, windowParamSettings, gridLinkParamSettings);

//Grid按钮参数配置，继承自执行参数
var gridButtonParamSettings = {
    onButtonClick: null,      //点击按钮执行方法
    linkText: "",             //连接文本，默认为当前字段值
    isButton: false,          //是否显示为按钮
    mustConfirm: true,         //是否需要用户确认

    //confirmBoxTitle
};

gridButtonParamSettings = jQuery.extend(true, {}, gridLinkParamSettings, gridButtonParamSettings);

var noticeBoxParamSettings = {
    maxmin: false,
    title: "无标题",
    width: "340px",
    height: "215px",
    time: null
}

var delRowParamSettings = {
    url: 'Delete',
    params: {},
    KeyField: 'Id',//行的唯一识别字段
    KeyParamName: 'ids'//:传递到后台对应的键值对名

}

delRowParamSettings = jQuery.extend(true, {}, baseParamSettings, delRowParamSettings);
var gridEnumDatas = {};//枚举
var keyValueArr = {};

function pageInit() {

    //easyui 文本框在table内也自适应
    window.onload = function () {
        $('.textbox-f').each(function () {
            $(this).next().css({ width: 1 });
        }).textbox('resize')

        $('.datagrid-f').each(function () {
            $(this).parent().parent().parent().css({ width: 1 });
        }).datagrid('resize')
    }
    window.onresize = function () {
        $('.textbox-f').each(function () {
            $(this).next().css({ width: 1 });
        }).textbox('resize')

        $('.datagrid-f').each(function () {
            $(this).parent().parent().parent().css({ width: 1 });
        }).datagrid('resize')

        $('.multiFileBox').each(function () {
            $(this).parent().parent().parent().css({ width: 1 });
        });
    }

    if (typeof (pageLoad) != "undefined")
        pageLoad();

    var preView = queryString("PreView");
    if (preView) {
        disableInputCtrl();
        hideBtnCtrl();
    }
}


function addAjaxParam(key, value) {
    if (!value) value = '';

    if (typeof (value) == "object" || typeof (value) == "array" || value.constructor == Array || value.constructor == Object)
        value = JSON.stringify(value);

    var newPair = {};
    newPair[key] = value;
    keyValueArr = $.extend(true, {}, keyValueArr, newPair);

}

function commitAjax(url, setting) {
    var tmpKeyValueArr = {};
    //不传递postData
    if (!setting.notUsePostData)
    {
        tmpKeyValueArr = $.extend(true, {}, keyValueArr);
        keyValueArr = {};
    }    
    $.post(url,
           tmpKeyValueArr,
           function (data) {
               if (data) {
                   for (var a in data) {
                       //在post后得到数据会将斜杠加引号变为引号，因此要加回去
                       if (Object.prototype.toString.call(data[a]) === '[object String]') {
                           if (data[a].indexOf('"') != -1) {
                               var reg = new RegExp('"', "g");//g,表示全部替换。
                               data[a] = data[a].replace(reg, '\\"')
                           }
                       }
                   }
               }

               if (setting.CallBack) {
                   setting.CallBack(data);
               }
           }).error(function (request, status, error) {
               var tmp = JSON.parse(request.responseText);
               if (tmp && tmp.IsNoticeBox)
               {
                   noticeBox(tmp.Msg, { title: '异常', maxmin: true });
               }
               else
               {
                   msgBox(tmp.Msg);
               }               
           });
}

function addGridLink(gridId, fieldName, url, gridLinkSettings) {
    var setting = jQuery.extend(true, {}, gridLinkParamSettings, { gridId: gridId }, gridLinkSettings);

    setting["gridId"] = gridId;
    setting["url"] = url;
    if (!setting["title"] && setting["linkText"])
        setting["title"] = setting["linkText"];
    pushOrUpdateSettings(setting);

    jQuery("#" + gridId + " th[field='" + fieldName + "']").each(function (index) {
        var jQueryitem = jQuery(this);
        jQueryitem.attr("formatter", "gridLinkFormatter");
        if (setting["linkText"] != "")
            jQueryitem.attr("align", "center");

        var colId = jQueryitem.attr("id");
        if (!colId | colId == '') {
            colId = "th_" + gridId + "_" + fieldName;
            jQueryitem.attr("id", colId);
            setting["key"] = colId;
        }

        if (setting.enumSourceVarName) {
            gridEnumDatas[colId] = setting.enumSourceVarName;
        }
    });
}

function gridLinkFormatter(value, row, index) {
    var key = this.id;
    var settings = getSettings(key);
    if (settings == undefined)
        return value;

    var text = settings.title || settings.linkText;
    //优先取枚举
    if (settings.enumSourceVarName) {
        var datas = eval(gridEnumDatas[key]);
        if (datas) {
            if (value != 0) {
                for (var i = 0; i < datas.length; i++)
                { if (datas[i].value == value) { text = datas[i].text; break; } }
            }
        }
    }
    else {
        if (text == '') {
            text = value;
        }
    }

    if (settings.onFilter) {
        if (!settings.onFilter(row))
            return text;
    }

    var style = 'color:blue';
    var res = "<a style='" + style + "' href='javascript:void(0)' onclick=gridLinkClick('" + index + "','" + key + "')>" + text + "</a>";
    return res;
}

function gridLinkClick(index, key) {
    var settings = getSettings(key);

    var rows = $('#' + settings["gridId"]).datagrid('getRows');//获得所有行
    var row = rows[index];//根据index获得其中一行。

    var url = settings.url;
    if (settings.paramField && row[settings.paramField]) {
        url = addSearch(url, settings.paramField, row[settings.paramField]);
    }
    openWindow(url, settings);
}

function addGridBtn(gridId, fieldName, gridBtnSettings) {
    var setting = jQuery.extend(true, {}, gridButtonParamSettings, { gridId: gridId }, gridBtnSettings);

    setting["gridId"] = gridId;
    if (!setting["title"] && setting["linkText"])
        setting["title"] = setting["linkText"];
    pushOrUpdateSettings(setting);

    jQuery("#" + gridId + " th[field='" + fieldName + "']").each(function (index) {
        var jQueryitem = jQuery(this);
        jQueryitem.attr("formatter", "gridBtnFormatter");
        if (setting["linkText"] != "")
            jQueryitem.attr("align", "center");

        var colId = jQueryitem.attr("id");
        if (!colId | colId == '') {
            colId = "th_" + gridId + "_" + fieldName;
            jQueryitem.attr("id", colId);
            setting["key"] = colId;
        }

        if (setting.enumSourceVarName) {
            gridEnumDatas[colId] = setting.enumSourceVarName;
        }
    });
}

function gridBtnFormatter(value, row, index) {
    var key = this.id;
    var settings = getSettings(key);
    if (settings == undefined)
        return value;

    var text = settings.title || settings.linkText;
    //优先取枚举
    if (settings.enumSourceVarName) {
        var datas = eval(gridEnumDatas[key]);
        if (datas) {
            if (value != 0) {
                for (var i = 0; i < datas.length; i++)
                { if (datas[i].value == value) { text = datas[i].text; break; } }
            }
        }
    }
    else {
        if (text == '') {
            text = value;
        }
    }

    if (settings.onFilter) {
        if (!settings.onFilter(row))
            return text;
    }

    var gridId = settings["gridId"];
    var opts = $('#' + gridId).datagrid('options');
    //如果有定义idfield index 取idfield的值
    if (opts.idField) {
        index = row[opts.idField];
    }

    var style = 'color:blue';

    var rowJson = JSON.stringify(row);
    var res = "<a style='" + style + "' href='javascript:void(0)' onclick=gridBtnClick('" + index + "','" + key + "')>" + text + "</a>";
    return res;
}

function gridBtnClick(index, key) {
    var settings = getSettings(key);
    var gridId = settings["gridId"];

    var row = {};
    var opts = $('#' + gridId).datagrid('options');
    //如果有定义idfield index 取idfield的值
    if (opts.idField) {
        var rows = $('#' + gridId).datagrid('getRows');
        $.each(rows, function (i, item) {
            if (item[opts.idField] == index) {
                row = item;
                index = $('#' + gridId).datagrid('getRowIndex', row);//index 重新定义为索引
                return false;
            }
        })
    }
        //否则index仍旧是行索引
    else {
        row = $('#' + gridId).datagrid('getRows')[index];
    }

    if (settings.onButtonClick) {
        settings.onButtonClick(row, index);
    }
}

var gridLinkSettingss = new Array();
//从设置数组中获取设置
function getSettings(key) {

    var settings;
    var settingss = gridLinkSettingss;
    for (var i = 0; i < settingss.length; i++) {
        if (settingss[i]["key"] == key) {
            settings = settingss[i];
            break;
        }
    }

    if (!settings) {
        return;
    }
    return settings;
}

function pushOrUpdateSettings(setting, key) {
    var settingss = gridLinkSettingss;
    if (key)
        setting["key"] = key;

    for (var i = 0; i < settingss.length; i++) {
        if (settingss[i]["key"] == setting["key"]) {
            settingss[i] = setting;
            break;
        }
    }
    if (i == settingss.length || settingss.length == 0)
        settingss.push(setting);
}


function addGridEnum(gridId, fieldName, enumSourceName) {
    jQuery("#" + gridId + " th[field='" + fieldName + "']").each(function (index) {
        var jQueryitem = jQuery(this);
        jQueryitem.attr("formatter", "gridEnumFormatter");
        var colId = jQueryitem.attr("id");
        if (!colId | colId == '') {
            colId = "th_" + gridId + "_" + fieldName;
            jQueryitem.attr("id", colId);
        }
        gridEnumDatas[colId] = enumSourceName;
    });
}

function gridEnumFormatter(value, rowData, rowIndex) {
    var key = this.id;
    var datas = eval(gridEnumDatas[key]);
    if (!datas)
        return value;

    if (value == 0) { return; } for (var i = 0; i < datas.length; i++)
    { if (datas[i].value == value) { return datas[i].text; } }

    return value;
};

var dateFormatDatas = {};//日期格式化
function addGridDate(gridId, fieldName, dateFormat) {
    jQuery("#" + gridId + " th[field='" + fieldName + "']").each(function (index) {
        var jQueryitem = jQuery(this);
        jQueryitem.attr("formatter", "gridDateFormatter");
        var colId = jQueryitem.attr("id");
        if (!colId | colId == '') {
            colId = "th_" + gridId + "_" + fieldName;
            jQueryitem.attr("id", colId);
        }
        dateFormatDatas[colId] = dateFormat;
    });
}

function gridDateFormatter(value, rec, index) {
    if (value == undefined) {
        return "";
    }

    var dateValue = new Date(value)

    //if (dateValue.getFullYear() < 1900) {
    //    return "";
    //}

    var key = this.id;
    return dateValue.format(dateFormatDatas[key]);
}
//---------------------------id=mf_form------------------------------start
function validateFormData() {
    var selector = $("form[id='mf_form']");
    if (selector.length == 0) {
        alert('页面未找到id为mf_form的form,无法完成saveFormData');
        return false;
    }

    var isValid = selector.form('validate');
    if (!isValid) {
        msgBox('请检查所填数据!');
        return false;
    }

    //子表validate
    selector.find("table[class*='datagrid-f']").each(function () {
        if (!$(this).datagrid('mfValidate')) {
            isValid = false;
            return false;
        }
    })

    return isValid;
}

function saveFormData() {
    if (!validateFormData())
        return;

    var selector = $("form[id='mf_form']");
    if (selector.length == 0) {
        alert('页面未找到id为mf_form的form,无法完成saveFormData');
        return false;
    }

    closeWindow(getFormData(selector));
}

function getFormData(form) {
    if (!form || form.length == 0)
        return msgBox('页面未找到id为mf_form的form,无法完成getFormData');

    var formData = {};
    var t = form.serializeArray();
    $.each(t, function () {
        formData[this.name] = this.value;
    });

    //针对mfbuttonedit要特殊赋值处理
    form.find("input[class*='mfbuttonedit']").each(function () {
        var opts = $(this).mfbuttonedit('options');
        if (opts.textName)
            formData[opts.textName] = $(this).mfbuttonedit('getText');
    })

    //多附件上传控件
    form.find("ul[class*='multiFileBox']").each(function () {
        var txtValArr = [];
        var rows = $('#' + this.id).datagrid("getRows");
        $.each(rows, function (index, item) {
            if (item.value != 'ctrlRow')
                txtValArr.push(item.value + "__________" + item.text);
        })
        formData[this.id] = txtValArr.join(',');
    })

    //针对datagrid特殊处理
    form.find("table[class*='datagrid-f']").each(function () {
        $(this).datagrid('endEditingCell');
        $(this).datagrid('getRows');
        var rows = $(this).datagrid('getRows');
        var rowsStr = JSON.stringify(rows);
        formData[this.id] = rowsStr;
    })

    return formData;
}

function setFormData(data, formId) {
    if (!data)
        return;

    formId = formId || 'mf_form';
    if ($("form[id='" + formId + "']").length == 0) {
        alert('页面未找到id为mf_form的form,无法完成setFormData');
        return;
    }

    if (typeof (data) == 'string')
        var data = eval('(' + data + ')');

    fillFormCtrl(data, formId);

    if (typeof (afterSetFormData) != "undefined")
        afterSetFormData(data);
}

//此时已经解析成页面html
function fillFormCtrl(data, formId) {
    for (var key in data) {
        if (fillTextBox(formId, data, key)) { }
        else if (fillComboBox(formId, data, key)) { }
        else if (fillDataGrid(formId, data, key)) { }
        else if (fillDataList(formId, data, key)) { }
        else if ($('#' + formId + ' input[Id=' + key + ']').length != 0) {
            $('#' + formId + ' input[Id=' + key + ']').val(data[key]);
        }
        else if ($('#' + formId + ' textarea[Id=' + key + ']').length != 0) {
            $('#' + formId + ' input[Id=' + key + ']').val(data[key]);
        }
    }
}

function fillTextBox(formId, data, key) {
    var ctrl = $('#' + formId + ' input[textboxname=' + key + ']');
    if (ctrl.length == 0)
        return;

    var classNames = ctrl.attr('class');
    //如果是下拉列表框不按textbox方式处理
    if (classNames.indexOf('combobox') != -1)
        return;

    if (classNames.indexOf('datebox') != -1)    
    {
        ctrl.datebox('setValue', data[key]);
    }
    else
    {
        ctrl.textbox('setValue', removeEscapeStr(data[key]));
        //扩展至textbox弹出选择控件要SetText    
        if (classNames.indexOf('mfbuttonedit') != -1) {
            var opts = ctrl.mfbuttonedit('options');
            if (opts.textName) {
                if (opts.foreignTableFieldName) {
                    //参：AutoUI.Areas.ConfigUI.Controllers.FormController的Get方法
                    ctrl.mfbuttonedit('setText', data[opts.textName + '__________' + opts.foreignTableFieldName]);
                }
                else {
                    ctrl.mfbuttonedit('setText', data[opts.textName]);
                }
            }
        }
    }
    
    return true;
}

function fillComboBox(formId, data, key) {
    var ctrl = $('#' + formId + ' input[textboxname=' + key + ']');
    if (ctrl.length == 0)
        ctrl = $('#' + formId + ' select[textboxname=' + key + ']');
    if (ctrl.length == 0)
        return;

    ctrl.combobox('setValue', data[key]);
    return true;
}

function fillDataGrid(formId, data, key) {
    var ctrl = $('#' + formId + ' table[id=' + key + ']');
    if (ctrl.length == 0)
        return;

    var rows = data[key];
    if (typeof (rows) == 'string')
        rows = eval('(' + rows + ')');

    $.each(rows, function (index, item) {
        ctrl.datagrid('mfInsertRow', { row: item })
    })
    return true;
}

function fillDataList(formId, data, key) {
    var ctrl = $('#' + formId + ' ul[name=' + key + ']');
    if (ctrl.length == 0)
        return;

    //扩展至datalist的多附件上传控件
    var classNames = ctrl.attr('class');
    if (classNames.indexOf('multiFileBox') != -1) {
        var tmpListStr = data[key];
        if (tmpListStr) {
            var tmpListArr = tmpListStr.split(',');
            $.each(tmpListArr, function (index, item) {
                if (item) {
                    var textValArr = item.split('__________');
                    if (textValArr.length == 2)
                        ctrl.datalist('insertRow', { index: 0, row: { text: textValArr[1], value: textValArr[0] } });
                }
            })
        }
    }
    return true;
}

function disableInputCtrl() {
    $('input:not(:button,:hidden)').prop('disabled', true);
    $('textarea').prop('disabled', true);

    $('select').prop('disabled', true);

    $(".easyui-datalist").each(function () {
        var dataSet = $(this)[0].dataset;
        if (dataSet.options && dataSet.options != '')
            dataSet.options += ',disabled:true';
        else
            dataSet.options = 'disabled:true';
        $(this)[0].removeChild($(this)[0].lastChild)
    })
    //$('input:radio').prop('disabled', true);
    //$('input:checkbox').prop('disabled', true);
}

function hideBtnCtrl() {
    $('a[class*=button]').css('display', 'none');
}
//---------------------------id=mf_form------------------------------end

//---------------------------id=mf_grid------------------------------start

//mustSelect(OneRow/MultiRow)
function checkSelection(mustSelect, actionStr) {
    var dataToWindow = null;
    if (mustSelect == 'OneRow')//参考ButtonDetail.cshtml的mustSelect
    {
        var rows = $('#mf_grid').datagrid('getSelections');
        if (rows.length != 1) {
            msgBox('请选择一行');
            return;
        }
        dataToWindow = rows[0];
    }
    else if (mustSelect == 'MultiRow')//参考ButtonDetail.cshtml的mustSelect
    {
        var dataToWindow = $('#mf_grid').datagrid('getSelections');
        if (dataToWindow.length == 0) {
            msgBox('请选择行');
            return;
        }
    }

    if (actionStr) {
        eval(actionStr);
    }
}

function delRow(setting) {
    setting = jQuery.extend(true, {}, delRowParamSettings, setting);

    var selector = $("table[id='" + setting.gridId + "']");
    if (selector.length == 0) {
        alert('页面未找到id为mf_grid的table,无法完成delRow');
        return;
    }

    var selections = selector.datagrid('getSelections');
    if (selections.length == 0) {
        return msgBox('请选择要删除的行');
    }

    confirmBox('是否删除?', function (r) {
        if (r) {
            //var className = selector.attr("class");
            //if (className.indexOf('datagrid') > 0) {
            //    $.each(selections, function (index, item) {
            //        var index = selector.datagrid('getRowIndex', item);
            //        selector.datagrid('deleteRow', index);
            //    })
            //}
            //else
            {
                if (!setting.url) {
                    msgBox('未指定执行删除的url');
                    return;
                }

                var ids = [];
                $.each(selections, function (index, item) {
                    ids.push(item[setting.KeyField]);
                })
                addAjaxParam(setting.KeyParamName, ids);

                for (var item in setting.params) {
                    addAjaxParam(item, setting.params[item]);
                }

                //转化为全路径
                url = changeToFullUrl(setting.url);

                commitAjax(url, {
                    CallBack: function (data) {
                        if (data) {
                            selector.datagrid('reload');
                            selector.datagrid('clearSelections');
                        }
                    }
                })
            }
        }
    })
}

function selectRow() {
    var selector = $("table[id='mf_grid']");
    if (selector.length == 0) {
        alert('页面未找到id为mf_grid的table,无法完成selectRow');
        return;
    }

    var selections = selector.datagrid('getSelections');
    if (selector.attr('singleSelect')
        && selector.attr('singleSelect') == 'true'
        && selections.length > 0) {
        closeWindow(selections[0]);
    }
    else {
        closeWindow(selections);
    }
}

function exportExcel(gridId, title) {
    gridId = gridId || "mf_grid";
    var selector = $("table[id='" + gridId + "']");
    if (selector.length == 0) {
        alert('页面未找到id为mf_grid的table,无法完成delRow');
        return;
    }

    var opts = selector.datagrid('options');
    var gridUrl = changeToFullUrl(opts.url);
    var columns = selector.datagrid("getColumnFields");
    var columnsWithTitle = [];
    $.each(columns, function (index, item) {
        if (item == 'ck') return true;
        var colOpt = selector.datagrid('getColumnOption', item);
        columnsWithTitle.push({ field: item, title: colOpt.title });
    })

    // 提交下载表单（利用iframe模拟Ajax）
    var $excelForm = $("#exportExcelForm");

    if ($excelForm.length == 0) {
        alert('请确保ID为excelForm的表单存在！');
    }

    if (!title)
        title = document.title;

    var formData = {
        gridUrl: gridUrl,
        referUrl: window.location.href,
        pageNumber: opts.pageNumber || '',
        pageSize: opts.pageSize || '',
        queryParams: JSON.stringify(opts.queryParams) || '',
        title: title,
        sortName: opts.sortName,
        sortOrder: opts.sortOrder,
        columns: JSON.stringify(columnsWithTitle)
    };

    for (var p in formData) {
        $excelForm.find("input[name='" + p + "']").val(formData[p]);
    }
    $excelForm.submit();
}
//---------------------------id=mf_grid------------------------------end

//function runAjax(setting)
//{
//    setting = jQuery.extend(true, {}, ajaxParamSettings, setting);
//    if (setting.params)
//    {
//        for (var item in setting.params) {
//            addAjaxParam(item, settings.params[item]);
//        }
//    }

//}

//---------------------------layer<-layer------------------------------start
function getlayer() {
    var thisLayer;
    var top = window.top;
    if (top && top.layer) {
        thisLayer = top.layer;
    }
    else if (this.layer) {
        thisLayer = this.layer;
    }
    else {
        alert('找不到窗体控件layer对象！');
        return;
    }
    return thisLayer;
}

function msgBox(info, callBack) {
    var thisLayer = getlayer();
    if (callBack) {
        thisLayer.confirm(info, {
            skin: 'layui-layer-lan',
            btn: ['确定'] //按钮
        }, function (index) {
            callBack(true);
            thisLayer.close(index);
        });
    }
    else {
        thisLayer.msg(info);
    }
}

function confirmBox(info, callBack) {
    var thisLayer = getlayer();
    thisLayer.confirm(info, {
        skin: 'layui-layer-lan',
        btn: ['确定', '取消'] //按钮
    }, function (index) {
        callBack(true);
        thisLayer.close(index);
    }, function () {
        callBack(false);
    });
}

function noticeBox(html, settings) {
    var params = jQuery.extend(true, {}, noticeBoxParamSettings, settings);
    var thisLayer = getlayer();
    thisLayer.open({
        type: 1,
        title: params.title,
        skin: 'layui-layer-lan',
        area: [params.width, params.height], //宽高
        offset: 'rb', //右下角弹出
        time: params.time,
        anim: 2,
        maxmin: params.maxmin,
        content: html
    });
}

function oneTextBoxBox(callBack, title, value) {
    var thisLayer = getlayer();
    thisLayer.prompt({
        value: value,
        title: title || '对话框',
        btnAlign: 'c',
        yes: function (index, layero) {
            // 获取文本框输入的值
            var value = layero.find(".layui-layer-input").val();
            if (callBack) {
                callBack(value);
            }
            thisLayer.close(index);
        }
    });
}

function openWindow(url, windowSettings) {
    if (typeof (url) == "undefined") {
        msgBox('当前url不能为空，请检查！');
        return;
    }

    if (windowSettings && typeof (windowSettings) == 'string' && windowSettings.constructor == String) {
        windowSettings = JSON.parse(windowSettings);
    }

    var settings = jQuery.extend(true, {}, windowParamSettings, windowSettings);

    var width = settings.width;
    if (settings.width && settings.width.toString().indexOf('%') != -1) {
        width = settings.width;
    }
    else {
        width = settings.width ? (settings.width.toString() + 'px') : '100px';
    }

    var height = settings.height;
    if (settings.height && settings.height.toString().indexOf('%') != -1) {
        height = settings.height;
    }
    else {
        height = settings.height ? (settings.height.toString() + 'px') : '100px';
    }

    //转化为全路径
    url = changeToFullUrl(url);
    if (settings.addQueryString)
    {
        url = addUrlSearch(url);
    }

    var thisLayer = getlayer();
    thisLayer.open({
        skin: 'layui-layer-lan',
        type: 2,
        title: settings.title || '&nbsp',
        maxmin: settings.showMaxButton,
        area: [width, height],
        content: [url],
        scrollbar: true,
        end: function () {
            if (settings.onDestroy) {
                if (thisLayer.windowResult)
                settings.onDestroy(thisLayer.windowResult);
                thisLayer.windowResult = null;//避免重复用
            }
        },
        success: function (layero, index) {
            var obj = $(layero).find('iframe')[0].contentWindow;
            if (obj.setFormData && settings.data) {
                obj.setFormData(settings.data);
            }
        }
    });
}

function resizeLayer(layerIndex, layerInitWidth, layerInitHeight) {
    var docWidth = $(document).width();
    var docHeight = $(document).height();
    var minWidth = layerInitWidth > docWidth ? docWidth : layerInitWidth;
    var minHeight = layerInitHeight > docHeight ? docHeight : layerInitHeight;
    console.log("doc:", docWidth, docHeight);
    console.log("lay:", layerInitWidth, layerInitHeight);
    console.log("min:", minWidth, minHeight);
    layer.style(layerIndex, {
        top: 0,
        width: minWidth,
        height: minHeight
    });
}

function closeWindow(windowResult) {
    var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
    parent.layer.windowResult = windowResult;
    parent.layer.close(index); //再执行关闭
}

//将当前地址栏参数加入到url
function addUrlSearch(url, execParams) {
    var newParams = [];

    var paramKeys = window.location.search.replace('?', '').split('&');
    for (var i = 0; i < paramKeys.length; i++) {
        var key = paramKeys[i].split('=')[0];
        if (key == "" || key == "_t" || key == "_winid")
            continue;
        if (typeof (execParams) == "undefined") {
            if (!hasQueryString(key, url))
                newParams.push(paramKeys[i]);
        }
        else {
            if (!hasQueryString(key, url) && execParams[key] == undefined)
                newParams.push(paramKeys[i]);
        }
    }

    if (url.indexOf('?') >= 0)
        return url + "&" + newParams.join('&');
    else
        return url + "?" + newParams.join('&');
}

//url增加参数
function addSearch(url, key, value) {
    if (!hasQueryString(key, url)) {
        if (url.indexOf('?') >= 0)
            return url + "&" + key + "=" + value;
        else
            return url + "?" + key + "=" + value;
    }
    else
        return url;
}

function hasQueryString(key, url) {
    if (typeof (url) == "undefined")
        url = window.location.search;

    var re = new RegExp("[?&]" + key + "=([^\\&]*)", "i");
    var a = re.exec(url);
    if (a == null) return false;
    return true;
}

//转化为全路径
function changeToFullUrl(url, currentUrlPathName) {
    if (url.indexOf('/') == 0 || url.indexOf("http://") == 0 || url.indexOf('?') == 0 || url == "")
        return url;


    if (typeof (currentUrlPathName) == "undefined" || currentUrlPathName == "")
        currentUrlPathName = window.location.pathname;

    var currentPathNameParts = currentUrlPathName.split('/');
    var pathNameParts = url.split('?')[0].split('/');
    if (currentPathNameParts[currentPathNameParts.length - 1] == "")
        currentPathNameParts.pop(); //去掉一个反斜线
    if (pathNameParts[pathNameParts.length - 1] == "")
        pathNameParts.pop(); //去掉一个反斜线


    var index = currentPathNameParts.length - 1;

    for (var i = 0; i < pathNameParts.length; i++) {
        if (pathNameParts[i] == "..") {
            index = index - 1;
            if (index <= 0) {
                msgUI("Url错误：" + url + "！", 4);
                return;
            }
            continue;
        }

        if (index < currentPathNameParts.length)
            currentPathNameParts[index] = pathNameParts[i];
        else
            currentPathNameParts.push(pathNameParts[i]);
        index = index + 1;
    }
    var length = currentPathNameParts.length;
    for (var i = index; i < length; i++) {
        currentPathNameParts.pop();
    }

    var result = currentPathNameParts.join('/');

    if (url.indexOf('?') > 0)
        result += url.substring(url.indexOf('?'));

    return result;
}
//---------------------------dialog<-layer------------------------------end
//----------------------------mf_file--------------------------------------start
function mfUpLoadFile(fileSizeRequired, extRequired, ctrlId) {
    var fileCtrlId = "file_" + ctrlId;
    var extArr = extRequired.split(',');
    var mimeArr = [];
    $.each(extArr, function (index, item) {
        mimeArr.push(getFileMIME(item));
    })
    var mimeStr = mimeArr.join(',');
    var inputHtm = "<input id='" + fileCtrlId + "' type='file' accept='" + mimeStr + "'  onchange='mfRealUpLoadFile(" + fileSizeRequired + ",\"" + fileCtrlId + "\",\"" + ctrlId + "\")' style='display:none' />"

    if ($("#" + fileCtrlId).length == 0) {
        $('body').append(inputHtm);
    }

    //处理只能响应一次mfRealUpLoadFile的问题
    $("#" + fileCtrlId).on('change', function () {
        $("#" + fileCtrlId).replaceWith(inputHtm);
    })

    $("#" + fileCtrlId).click();
}

function mfRealUpLoadFile(fileSizeRequired, filCtrlId, dataListId) {
    var fileCtrl = $("#" + filCtrlId);

    if (fileCtrl.val().length > 0) {
        //大小判断
        //html5
        if (fileCtrl[0].files) {
            if (fileCtrl[0].files[0].size / 1024 > fileSizeRequired) {
                alert("附件大小不能超过" + fileSizeRequired + "KB");
                return;
            }
        }
        else {
            return msgBox('浏览器不支持h5上传文件');
        }

        var formData = new FormData()
        formData.append('fileData', fileCtrl[0].files[0]);
        $.ajax({
            url: "/AutoUI/ConfigUI/File/UpLoadFile",
            type: "post",
            data: formData,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.IsNoPass) {//同ScriptModule ex.GetType() == typeof(BusinessException)
                    msgBox(data.Msg);
                }
                else {
                    msgBox("上传成功!");
                    var dataList = $("#" + dataListId);
                    dataList.datagrid('insertRow', { index: 0, row: { text: data.name, value: data.id } });
                }

                //window.clearInterval(timer);
                //console.log("over..");
            },
            error: function (e) {
                noticeBox(e.responseText, { title: '异常', maxmin: true });
                //window.clearInterval(timer);
            }
        });
        //get();//此处为上传文件的进度条
    }
}

function upLoadDataListRowRender(text, row, index) {
    var dataListId = this.id;
    var res;
    if (row.value == 'ctrlRow') {
        var opts = $('#' + dataListId).datagrid('options');
        var fileMaxSize = $('#' + dataListId).attr('fileMaxSize');
        var fileExt = opts.fileextaccept || "";
        if (!opts.disabled)
            res = "<a href= '#' class='dataListAddRow' onclick=\"mfUpLoadFile(" + fileMaxSize + ", '" + fileExt + "', '" + dataListId + "')\"> " + row.text + "</a>";
    }
    else {
        var url = '#';
        var fileDownLoadUrl = $('#' + dataListId).attr('downLoadUrl');
        var opts = $('#' + dataListId).datagrid('options');
        if (fileDownLoadUrl) {
            url = HOST_URL + fileDownLoadUrl + row.value + '__________' + row.text;
        }

        res = "<a href='" + url + "'>" + row.text + "</a>";
        if (!opts.disabled)
            res += "<a class='dataListDeleteRow' onclick=\"msgBox('是否移除?',function(){ mfUpLoadDataListDeleteRow('" + dataListId + "','" + row.value + "')})\" style=\"float:right;cursor:pointer\">X</a>";
    }
    return res;
}

function mfUpLoadDataListOnLoad() {
    var dataListId = this.id;
    var rows = $('#' + dataListId).datagrid('getRows');
    var ctrlRow = $.grep(rows, function (item, index) {
        return item.value == 'ctrlRow';
    })
    var opts = $('#' + dataListId).datagrid('options');
    if (ctrlRow.length == 0 && !opts.disabled)
        $('#' + dataListId).datagrid('appendRow', { text: '点击上传文件...', value: 'ctrlRow' });//加载成功后最后一行作为操作行
}

function mfUpLoadDataListDeleteRow(dataListId, value) {
    var rows = $('#' + dataListId).datagrid('getRows');
    $.each(rows, function (index, item) {
        if (item.value && item.value == value) {
            $('#' + dataListId).datagrid('deleteRow', index);
            return false;
        }
    })
}

//----------------------------file--------------------------------------end
//-------------------------------------tool-----------------------------start
function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

function guid() {
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

function guidEmpty() {
    return "00000000-0000-0000-0000-000000000000";
}

function queryString(paramName) {
    var reg = new RegExp("[\?&]" + paramName + "=([^&]*)[&]?", "i");
    var paramVal = window.location.search.match(reg);
    return paramVal == null ? "" : paramVal[1];
}

function addLoadEvent(func) {
    //把现有的 window.onload 事件处理函数的值存入变量
    var oldOnload = window.onload;
    if (typeof window.onload != "function") {
        //如果这个处理函数还没有绑定任何函数，就像平时那样添加新函数
        window.onload = func;
    } else {
        //如果处理函数已经绑定了一些函数，就把新函数添加到末尾
        window.onload = function () {
            oldOnload();
            func();
        }
    }
}

function getFileMIME(fileExt) {
    if (fileExt == '.doc') {
        return 'application/msword'
    }
    else if (fileExt == '.docx') {
        return 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
    }
    else if (fileExt == '.xls') {
        return 'application/vnd.ms-excel'
    }
    else if (fileExt == '.xlsx') {
        return 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    }
    else if (fileExt == '.pdf') {
        return 'application/pdf'
    }
    else if (fileExt == '.jpg') {
        return 'image/jpeg'
    }
    else {
        return ''
    }
}

function removeEscapeStr(s) {
    // 去掉转义字符  
    s = s.replace(/\\/g, '');
    // 去掉特殊字符  
    //s = s.replace(/[\@\#\$\%\^\&\*\{\}\:\"\L\<\>\?]/);
    return s;
};

//-------------------------------------end-----------------------------end
//----------------------------------extension--------------------------start
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
    (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
        RegExp.$1.length == 1 ? o[k] :
        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}

String.prototype.toJSON = function () {
    return (new Function("return " + this))()
}

//首先我们需要一下工具方法
//为数组添加一些方法
//去重
Array.prototype.unique = function () {
    this.sort();
    var re = [this[0]];
    for (var i = 1; i < this.length; i++) {
        if (this[i] !== re[re.length - 1]) {
            re.push(this[i]);
        }
    }
    return re;
}

//删除
Array.prototype.remove = function (val) {
    var index = this.indexOf(val);
    if (index > -1) {
        this.splice(index, 1);
    }
}
//判断元素是否存在
Array.prototype.contains = function (obj) {
    var i = this.length;
    while (i--) {
        if (this[i] === obj) {
            return true;
        }
    }
    return false;
}

//判断元素字段是否存在
Array.prototype.containsField = function (obj, field) {
    var i = this.length;
    while (i--) {
        if (!this[i][field])
            return false;

        if (this[i][field] === obj) {
            return true;
        }
    }
    return false;
}

//对数组或集合的深拷贝
function objDeepCopy(source) {
    var sourceCopy = source instanceof Array ? [] : {};
    for (var item in source) {
        sourceCopy[item] = typeof source[item] === 'object' ? objDeepCopy(source[item])
                : source[item];
    }
    return sourceCopy;
}
//定位
Array.prototype.indexOf = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) return i;
    }
    return -1;
};
//判断两个对象指定属性的值是否相等(忽略类型)
function isObjectValueEqual(obj1, obj2, arr) {
    for (var i = 0; i < arr.length; i++) {
        var propName = arr[i];
        if (obj1[propName] != obj2[propName]) {
            return false;
        }
    }
    return true;
}

var HOST_URL = function () {
    var port = location.port;
    var hostUrl = "http://";

    if (port == 80) {
        hostUrl += location.hostname;
    } else {
        hostUrl += location.host;
    }

    return hostUrl;
}();