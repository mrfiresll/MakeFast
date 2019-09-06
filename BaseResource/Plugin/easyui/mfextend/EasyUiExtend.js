/**
* jQuery EasyUI 1.4.3
* Copyright (c) 2009-2015 www.jeasyui.com. All rights reserved.
*
* Licensed under the GPL license: http://www.gnu.org/licenses/gpl.txt
* To use it on other terms please contact us at info@jeasyui.com
* http://www.jeasyui.com/license_commercial.php
*
* jQuery EasyUI datagrid 组件扩展
* jeasyui.extensions.datagrid.appendRow.js
*/
(function () {

    //$.util.namespace("$.fn.datagrid.extensions");

    var _originalMethod = {};
    $.extend(_originalMethod, {
        appendRow: $.fn.datagrid.methods.appendRow
    });

    var appendRow = function (target, row) {
        var t = $(target),
            state = $.data(target, "datagrid"),
            opts = state.options;

        if ($.isFunction(opts.onBeforeAppendRow) && opts.onBeforeAppendRow.call(target, row) == false) { return; }
        _originalMethod.appendRow.call(target, t, row);
        if ($.isFunction(opts.onAppendRow)) { opts.onAppendRow.call(target, row); }
    };

    var appendRows = function (target, rows) {
        var t = $(target),
            state = $.data(target, "datagrid"),
            opts = state.options;
        if ($.array.likeArrayNotString(rows)) {
            $.each(rows, function (index, val) { appendRow(target, val); });
        }
    };

    var methods = {

        //  重写 easyui-datagrid 的 appendRow 方法；参数 row 表示要追加的数据行；
        //  返回值：返回表示当前 easyui-datagrid 组件的 jQuery 链式对象。
        appendRow: function (jq, row) { return jq.each(function () { appendRow(this, row); }); },

        //  扩展 easyui-datagrid 的自定义方法；参数 rows 表示要追加的数据行数组；该参数是以下类型：
        //      Array 类型，数组中的每一项目均表示要追加的行数据对象
        //  返回值：返回表示当前 easyui-datagrid 组件的 jQuery 链式对象。
        appendRows: function (jq, rows) { return jq.each(function () { appendRows(this, rows); }); }
    };

    var events = {
        onBeforeCheck: function (index, row) {
            var opts = $(this).datagrid('options');
            if (!opts.singleSelect) {
                var state = $.data(this, 'datagrid');
                if (opts.idField) {
                    $.easyui.addArrayItem(state.selectedRows, opts.idField, row);
                    $.easyui.addArrayItem(state.checkedRows, opts.idField, row);
                }
                opts.finder.getTr(this, index).addClass('datagrid-row-selected');
                var tr = opts.finder.getTr(this, index).addClass('datagrid-row-checked');
                tr.find('div.datagrid-cell-check input[type=checkbox]')._propAttr('checked', true);
                tr = opts.finder.getTr(this, '', 'checked', 2);
                if (tr.length == opts.finder.getRows(this).length) {
                    var dc = state.dc;
                    dc.header1.add(dc.header2).find('input[type=checkbox]')._propAttr('checked', true);
                }
                return false;
            }

        },
        onBeforeUncheck: function (index, row) {
            var opts = $(this).datagrid('options');
            if (!opts.singleSelect) {
                var state = $.data(this, 'datagrid');
                if (opts.idField) {
                    $.easyui.removeArrayItem(state.selectedRows, opts.idField, row[opts.idField]);
                    $.easyui.removeArrayItem(state.checkedRows, opts.idField, row[opts.idField]);
                }
                opts.finder.getTr(this, index).removeClass('datagrid-row-selected');
                var tr = opts.finder.getTr(this, index).removeClass('datagrid-row-checked');
                tr.find('div.datagrid-cell-check input[type=checkbox]')._propAttr('checked', false);
                tr = opts.finder.getTr(this, '', 'checked', 2);
                if (tr.length == opts.finder.getRows(this).length) {
                    var dc = state.dc;
                    dc.header1.add(dc.header2).find('input[type=checkbox]')._propAttr('checked', false);
                }
                return false;
            }
        },
        onBeforeSelect: function (index, row) {
            var opts = $(this).datagrid('options');
            var selectedRows = $(this).datagrid('getSelections');
            var inRows = $.grep(selectedRows, function (item, index) { return item == row; });

            //不是选中已选择行,则清除其他行的选中状态
            if (inRows.length == 0) {
                $(this).datagrid('clearSelections');
                $(this).datagrid('clearChecked');

                var state = $.data(this, 'datagrid');
                if (opts.idField) {
                    $.easyui.addArrayItem(state.selectedRows, opts.idField, row);
                }
                opts.finder.getTr(this, index).addClass('datagrid-row-selected');

                var tr = opts.finder.getTr(this, index).addClass('datagrid-row-checked');
                tr.find('div.datagrid-cell-check input[type=checkbox]')._propAttr('checked', true);
                tr = opts.finder.getTr(this, '', 'checked', 2);
                if (tr.length == opts.finder.getRows(this).length) {
                    var dc = state.dc;
                    dc.header1.add(dc.header2).find('input[type=checkbox]')._propAttr('checked', true);
                }
            }
            return false;
        },
        onBeforeUnselect: function (index, row) {
            return false;
        },
        onLoadError: function (request, status, error) {
            var tmp = JSON.parse(request.responseText);
            if (tmp && tmp.IsNoticeBox) {
                noticeBox(tmp.Msg, { title: '异常', maxmin: true });
            }
            else {
                msgBox(tmp.Msg);
            }
        }
    }


    var defaults = {

        //  扩展 easyui-datagrid 的自定义事件；该事件表示执行 appendRow 方法前所触发的动作；该事件回调函数提供如下参数：
        //      row:    表示要进行 appendRow 操作的行数据对象；
        //  该事件函数中的 this 指向当前 easyui-datarid 的 DOM 对象(非 jQuery 对象)；
        //  备注：如果该事件回调函数返回 false，则立即取消即将要执行的 appendRow 操作。
        onBeforeAppendRow: function (row) { },

        //  扩展 easyui-datagrid 的自定义事件；该事件表示执行 appendRow 方法后所触发的动作；该事件回调函数提供如下参数：
        //      row:    表示要进行 appendRow 操作的行数据对象；
        //  该事件函数中的 this 指向当前 easyui-datarid 的 DOM 对象(非 jQuery 对象)；
        onAppendRow: function (row) { }
    };



    //if ($.fn.datagrid.extensions.defaults) {
    //    $.extend($.fn.datagrid.extensions.defaults, defaults);
    //} else {
    //    $.fn.datagrid.extensions.defaults = defaults;
    //}

    //if ($.fn.datagrid.extensions.methods) {
    //    $.extend($.fn.datagrid.extensions.methods, methods);
    //} else {
    //    $.fn.datagrid.extensions.methods = methods;
    //}

    //onLoadError

    $.extend($.fn.datagrid.defaults, defaults);
    $.extend($.fn.datagrid.defaults, events);
    $.extend($.fn.datagrid.methods, methods);

})(jQuery);