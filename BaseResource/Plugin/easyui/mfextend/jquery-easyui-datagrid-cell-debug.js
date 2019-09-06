function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

function guid() {
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

(function ($) {
    $.extend($.fn.datagrid.defaults, {
        editorHeight:31,//可编辑的控件高度为表格整体样式中行高
        saveUrl: null,	// return the added row
        saveParamName: 'rows',
        idField: 'mfid',//sll add
        oldDatas: [],//sll add
        updateRows: [],//sll add
        clickToEdit: true,
        dblclickToEdit: false,
        navHandler: {
            '37': function (e) {
                var opts = $(this).datagrid('options');
                return navHandler.call(this, e, opts.isRtl ? 'right' : 'left');
            },
            '39': function (e) {
                var opts = $(this).datagrid('options');
                return navHandler.call(this, e, opts.isRtl ? 'left' : 'right');
            },
            '38': function (e) {
                return navHandler.call(this, e, 'up');
            },
            '40': function (e) {
                return navHandler.call(this, e, 'down');
            },
            '13': function (e) {
                return enterHandler.call(this, e);
            },
            '27': function (e) {
                return escHandler.call(this, e);
            },
            '8': function (e) {
                return clearHandler.call(this, e);
            },
            '46': function (e) {
                return clearHandler.call(this, e);
            },
            'keypress': function (e) {
                if (e.metaKey || e.ctrlKey) {
                    return;
                }
                var dg = $(this);
                var param = dg.datagrid('cell'); // current cell information
                if (!param) {
                    return;
                }
                var input = dg.datagrid('input', param);
                if (!input) {
                    var tmp = $('<span></span>');
                    tmp.html(String.fromCharCode(e.which));
                    var c = tmp.text();
                    tmp.remove();
                    if (c) {
                        dg.datagrid('editCell', {
                            index: param.index,
                            field: param.field,
                            value: c
                        });
                        return false;
                    }
                }
            }
        },
        onBeforeCellEdit: function (index, field) { },
        onCellEdit: function (index, field, value) {
            var input = $(this).datagrid('input', {
                index: index,
                field: field
            });
            if (input) {
                if (value != undefined) {
                    input.val(value);
                }
            }
        },
        onCancelEdit: function (index, row) {
            var opts = $(this).datagrid('options');
            //opts.editIndex = undefined;
            var idValue = row[opts.idField || 'mfid'];
            var sameObjs = $.grep(opts.oldDatas, function (item, index) { return item[opts.idField || 'mfid'] == idValue; });

            if (sameObjs.length == 1) {
                $(this).datagrid('updateRow', { index: index, row: sameObjs[0] });
            }

            //if (opts.onCancelEdit) opts.onCancelEdit.call(target, index, row);
        },
        onSelectCell: function (index, field) { },
        onUnselectCell: function (index, field) { },
        onInsertRow: function (index, row) { },
        onBeforeSave: function (index) { },
        onSuccess: function () { },
        onError: function () { }
    });

    function navHandler(e, dir) {
        var dg = $(this);
        var param = dg.datagrid('cell');
        var input = dg.datagrid('input', param);
        if (!input) {
            dg.datagrid('gotoCell', dir);
            return false;
        }
    }

    function enterHandler(e) {
        var dg = $(this);
        var cell = dg.datagrid('cell');
        if (!cell) {
            return;
        }
        var input = dg.datagrid('input', cell);
        if (input) {
            endCellEdit(this, true);
        } else {
            dg.datagrid('editCell', cell);
        }
        return false;
    }

    function escHandler(e) {
        endCellEdit(this, false);
        return false;
    }

    function clearHandler(e) {
        var dg = $(this);
        var param = dg.datagrid('cell');
        if (!param) {
            return;
        }
        var input = dg.datagrid('input', param);
        if (!input) {
            dg.datagrid('editCell', {
                index: param.index,
                field: param.field,
                value: ''
            });
            return false;
        }
    }

    function getCurrCell(target) {
        var cell = $(target).datagrid('getPanel').find('td.datagrid-row-selected');
        if (cell.length) {
            return {
                index: parseInt(cell.closest('tr.datagrid-row').attr('datagrid-row-index')),
                field: cell.attr('field')
            };
        } else {
            return null;
        }
    }

    function unselectCell(target, p) {
        var opts = $(target).datagrid('options');
        var cell = opts.finder.getTr(target, p.index).find('td[field="' + p.field + '"]');
        cell.removeClass('datagrid-row-selected');
        opts.onUnselectCell.call(target, p.index, p.field);
    }

    function unselectAllCells(target) {
        var panel = $(target).datagrid('getPanel');
        panel.find('td.datagrid-row-selected').removeClass('datagrid-row-selected');
    }

    function selectCell(target, p) {
        var opts = $(target).datagrid('options');
        if (opts.singleSelect) {
            unselectAllCells(target);
        }
        var cell = opts.finder.getTr(target, p.index).find('td[field="' + p.field + '"]');
        cell.addClass('datagrid-row-selected');
        opts.onSelectCell.call(target, p.index, p.field);
    }

    function getSelectedCells(target) {
        var cells = [];
        var panel = $(target).datagrid('getPanel');
        panel.find('td.datagrid-row-selected').each(function () {
            var td = $(this);
            cells.push({
                index: parseInt(td.closest('tr.datagrid-row').attr('datagrid-row-index')),
                field: td.attr('field')
            });
        });
        return cells;
    }

    function gotoCell(target, p) {
        var dg = $(target);
        var opts = dg.datagrid('options');
        var panel = dg.datagrid('getPanel').focus();

        var cparam = dg.datagrid('cell');
        if (cparam) {
            var input = dg.datagrid('input', cparam);
            if (input) {
                input.focus();
                return;
            }
        }

        if (typeof p == 'object') {
            _go(p);
            return;
        }
        var cell = panel.find('td.datagrid-row-selected');
        if (!cell) {
            return;
        }
        var fields = dg.datagrid('getColumnFields', true).concat(dg.datagrid('getColumnFields'));
        var field = cell.attr('field');
        var tr = cell.closest('tr.datagrid-row');
        var rowIndex = parseInt(tr.attr('datagrid-row-index'));
        var colIndex = $.inArray(field, fields);

        if (p == 'up' && rowIndex > 0) {
            rowIndex--;
        } else if (p == 'down') {
            if (opts.finder.getRow(target, rowIndex + 1)) {
                rowIndex++;
            }
        } else if (p == 'left') {
            var i = colIndex - 1;
            while (i >= 0) {
                var col = dg.datagrid('getColumnOption', fields[i]);
                if (!col.hidden) {
                    colIndex = i;
                    break;
                }
                i--;
            }
        } else if (p == 'right') {
            var i = colIndex + 1;
            while (i <= fields.length - 1) {
                var col = dg.datagrid('getColumnOption', fields[i]);
                if (!col.hidden) {
                    colIndex = i;
                    break;
                }
                i++;
            }
        }

        field = fields[colIndex];

        _go({
            index: rowIndex,
            field: field
        });

        function _go(p) {
            dg.datagrid('scrollTo', p.index);
            unselectAllCells(target);
            selectCell(target, p);
            var td = opts.finder.getTr(target, p.index, 'body', 2).find('td[field="' + p.field + '"]');
            if (td.length) {
                var body2 = dg.data('datagrid').dc.body2;
                var left = td.position().left;
                if (left < 0) {
                    body2._scrollLeft(body2._scrollLeft() + left * (opts.isRtl ? -1 : 1));
                } else if (left + td._outerWidth() > body2.width()) {
                    body2._scrollLeft(body2._scrollLeft() + (left + td._outerWidth() - body2.width()) * (opts.isRtl ?
                        -1 : 1));
                }
            }
        }
    }

    // end the current cell editing
    function endCellEdit(target, accepted) {
        var dg = $(target);
        var opts = dg.datagrid('options');
        var cell = dg.datagrid('cell');
        if (cell) {
            var input = dg.datagrid('input', cell);
            if (input) {
                if (accepted) {
                    if (dg.datagrid('validateRow', cell.index)) {
                        dg.datagrid('endEdit', cell.index);
                        _refreshUpdateRows(cell, dg);
                        dg.datagrid('gotoCell', cell);
                    } else {
                        dg.datagrid('gotoCell', cell);
                        input.focus();
                        return false;
                    }
                } else {
                    dg.datagrid('cancelEdit', cell.index);
                    dg.datagrid('gotoCell', cell);
                }
            }
        }
        return true;
    }

    function _refreshUpdateRows(cell, dg) {
        var opts = dg.datagrid('options');
        if (opts.oldDatas) {
            var rows = dg.datagrid('getRows');
            var row = rows[cell.index];
            var tmpIndex = cell.index - (rows.length - opts.oldDatas.length);
            //判断元数据是否被修改
            if (tmpIndex >= 0) {
                var oldData = opts.oldDatas[tmpIndex];
                var isEqual = true;

                if (!oldData[cell.field] || row[cell.field] != oldData[cell.field]) {
                    isEqual = false;
                    var inserts = dg.datagrid('getChanges', "inserted");
                    if (!opts.updateRows.contains(row) && !inserts.contains(row)) {
                        opts.updateRows.push(row)
                    }
                }

                if (isEqual) {
                    opts.updateRows.remove(row);
                }
            }
        }
    }

    function _mfSort(index, type, dg) {
        if ("up" == type) {
            if (index != 0) {
                var toup = dg.datagrid('getData').rows[index];
                var todown = dg.datagrid('getData').rows[index - 1];
                dg.datagrid('getData').rows[index] = todown;
                dg.datagrid('getData').rows[index - 1] = toup;
                dg.datagrid('refreshRow', index);
                dg.datagrid('refreshRow', index - 1);
                dg.datagrid('checkRow', index - 1);
                dg.datagrid('uncheckRow', index);
                return true;
            }
        } else if ("down" == type) {
            var rows = dg.datagrid('getRows').length;
            if (index != rows - 1) {
                var todown = dg.datagrid('getData').rows[index];
                var toup = dg.datagrid('getData').rows[index + 1];
                dg.datagrid('getData').rows[index + 1] = todown;
                dg.datagrid('getData').rows[index] = toup;
                dg.datagrid('refreshRow', index);
                dg.datagrid('refreshRow', index + 1);
                dg.datagrid('checkRow', index + 1);
                dg.datagrid('uncheckRow', index);
                return true;
            }
        }
    }

    function editCell(target, param) {
        var dg = $(target);
        var opts = dg.datagrid('options');
        var input = dg.datagrid('input', param);
        if (input) {
            dg.datagrid('gotoCell', param);
            input.focus();
            return;
        }
        if (!endCellEdit(target, true)) {
            return;
        }
        if (opts.onBeforeCellEdit.call(target, param.index, param.field) == false) {
            return;
        }

        var fields = dg.datagrid('getColumnFields', true).concat(dg.datagrid('getColumnFields'));
        $.map(fields, function (field) {
            var col = dg.datagrid('getColumnOption', field);
            col.editor1 = col.editor;
            if (field != param.field) {
                col.editor = null;
            }
        });

        var col = dg.datagrid('getColumnOption', param.field);
        if (col.editor) {
            dg.datagrid('beginEdit', param.index);
            var input = dg.datagrid('input', param);
            if (input) {
                if (opts.onBeforeSave.call(target, param.index) == false) {
                    setTimeout(function () {
                        dg.datagrid('selectRow', param.index);
                    }, 0);
                    return;
                }

                dg.datagrid('gotoCell', param);
                setTimeout(function () {
                    input.unbind('.cellediting').bind('keydown.cellediting', function (e) {
                        if (e.keyCode == 13) {
                            opts.navHandler['13'].call(target, e);
                            return false;
                        }
                    });
                    input.focus();
                }, 0);
                opts.onCellEdit.call(target, param.index, param.field, param.value);
            } else {
                dg.datagrid('cancelEdit', param.index);
                dg.datagrid('gotoCell', param);
            }
        } else {
            dg.datagrid('gotoCell', param);
        }

        $.map(fields, function (field) {
            var col = dg.datagrid('getColumnOption', field);
            col.editor = col.editor1;
        });
    }

    function enableCellSelecting(target) {
        var dg = $(target);
        var state = dg.data('datagrid');
        var panel = dg.datagrid('getPanel');
        var opts = state.options;
        var dc = state.dc;
        panel.attr('tabindex', 1).css('outline-style', 'none').unbind('.cellediting').bind('keydown.cellediting',
            function (e) {
                var h = opts.navHandler[e.keyCode];
                if (h) {
                    return h.call(target, e);
                }
            });
        dc.body1.add(dc.body2).unbind('.cellediting').bind('click.cellediting', function (e) {
            var tr = $(e.target).closest('.datagrid-row');
            if (tr.length && tr.parent().length) {
                var td = $(e.target).closest('td[field]', tr);
                if (td.length) {
                    var index = parseInt(tr.attr('datagrid-row-index'));
                    var field = td.attr('field');
                    var p = {
                        index: index,
                        field: field
                    };
                    if (opts.singleSelect) {
                        selectCell(target, p);
                    } else {
                        if (opts.ctrlSelect) {
                            if (e.ctrlKey) {
                                if (td.hasClass('datagrid-row-selected')) {
                                    unselectCell(target, p);
                                } else {
                                    selectCell(target, p);
                                }
                            } else {
                                unselectAllCells(target);
                                selectCell(target, p);
                            }
                        } else {
                            if (td.hasClass('datagrid-row-selected')) {
                                unselectCell(target, p);
                            } else {
                                selectCell(target, p);
                            }
                        }
                    }
                }
            }
        }).bind('mouseover.cellediting', function (e) {
            var td = $(e.target).closest('td[field]');
            if (td.length) {
                td.addClass('datagrid-row-over');
                td.closest('tr.datagrid-row').removeClass('datagrid-row-over');
            }
        }).bind('mouseout.cellediting', function (e) {
            var td = $(e.target).closest('td[field]');
            td.removeClass('datagrid-row-over');
        });

        opts.isRtl = dg.datagrid('getPanel').css('direction').toLowerCase() == 'rtl';
        opts.OldOnBeforeSelect = opts.onBeforeSelect;
        opts.onBeforeSelect = function () {
            return true;
        };
        dg.datagrid('clearSelections');
    }

    function disableCellSelecting(target) {
        var dg = $(target);
        var state = dg.data('datagrid');
        var panel = dg.datagrid('getPanel');
        var opts = state.options;
        opts.onBeforeSelect = opts.OldOnBeforeSelect || opts.onBeforeSelect;
        panel.unbind('.cellediting').find('td.datagrid-row-selected').removeClass('datagrid-row-selected');
        var dc = state.dc;
        dc.body1.add(dc.body2).unbind('cellediting');
    }

    function enableCellEditing(target) {
        var dg = $(target);
        var opts = dg.datagrid('options');
        var panel = dg.datagrid('getPanel');
        panel.attr('tabindex', 1).css('outline-style', 'none').unbind('.cellediting').bind('keydown.cellediting',
            function (e) {
                var h = opts.navHandler[e.keyCode];
                if (h) {
                    return h.call(target, e);
                }
            }).bind('keypress.cellediting', function (e) {
                return opts.navHandler['keypress'].call(target, e);
            });
        panel.panel('panel').unbind('.cellediting').bind('keydown.cellediting', function (e) {
            e.stopPropagation();
        }).bind('keypress.cellediting', function (e) {
            e.stopPropagation();
        }).bind('mouseover.cellediting', function (e) {
            var td = $(e.target).closest('td[field]');
            if (td.length) {
                td.addClass('datagrid-row-over');
                td.closest('tr.datagrid-row').removeClass('datagrid-row-over');
            }
        }).bind('mouseout.cellediting', function (e) {
            var td = $(e.target).closest('td[field]');
            td.removeClass('datagrid-row-over');
        });

        function _styler(value, row, index, field) {
            var opts = $(target).datagrid('options')
            var idfield = opts.idField || 'mfid';
            var olddata = opts.oldDatas;
            for (var i = 0; i < olddata.length; i++) {
                if (olddata[i][idfield] && olddata[i][idfield] == row[idfield]) {
                    if (olddata[i][field] != value) {
                        return "background:url('/BaseResource/Plugin/easyui/mfextend/dirty.gif') right 0px no-repeat;";
                    }
                }
            }
            return '';
        }

        var fields = $(target).datagrid('getColumnFields');

        $.each(fields, function (index, item) {
            var colOpt = $(target).datagrid('getColumnOption', item);
            if (colOpt.editor) {
                colOpt.styler = _styler;
            }
        })

        opts.CellEdit = true;
        opts.isRtl = dg.datagrid('getPanel').css('direction').toLowerCase() == 'rtl';
        opts.oldOnClickCell = opts.onClickCell;
        opts.oldOnClickRow = opts.onClickRow;
        opts.oldOnDblClickCell = opts.onDblClickCell;
        opts.onClickCell = function (index, field, value) {
            if (opts.clickToEdit) {
                $(this).datagrid('editCell', {
                    index: index,
                    field: field
                });
            } else {
                if (endCellEdit(this, true)) {
                    $(this).datagrid('gotoCell', {
                        index: index,
                        field: field
                    });
                }
            }
 
            opts.oldOnClickCell.call(this, index, field, value);
        }
        opts.onClickRow = function (index, row)
        {

            
            opts.oldOnClickRow.call(this, index, row);
        }
        if (opts.dblclickToEdit) {
            opts.onDblClickCell = function (index, field, value) {
                $(this).datagrid('editCell', {
                    index: index,
                    field: field
                });
                opts.oldOnDblClickCell.call(this, index, field, value);
            }
        }
        opts.OldOnBeforeSelect = opts.onBeforeSelect;
        opts.onBeforeSelect = function () {
            return true;
        };
        dg.datagrid('clearSelections')
    }

    function disableCellEditing(target) {
        var dg = $(target);
        var panel = dg.datagrid('getPanel');
        var opts = dg.datagrid('options');
        opts.onClickCell = opts.oldOnClickCell || opts.onClickCell;
        opts.onDblClickCell = opts.oldOnDblClickCell || opts.onDblClickCell;
        opts.onBeforeSelect = opts.OldOnBeforeSelect || opts.onBeforeSelect;
        panel.unbind('.cellediting').find('td.datagrid-row-selected').removeClass('datagrid-row-selected');
        panel.panel('panel').unbind('.cellediting');
    }

    $.extend($.fn.datagrid.defaults.view, {
        onBeforeRender: function (target, rows) {
            var opts = $(target).datagrid('options');
            $.each(rows, function (index, item) {
                delete item['state'];
                if (opts.idField == 'mfid' && !item['mfid']) {
                    item['mfid'] = guid();
                }
            })
            opts.oldDatas = jQuery.extend(true, [], rows);//克隆旧数据            
        }

    })

    $.extend($.fn.datagrid.methods, {
        endEditingCell: function (jq, param) {
            return jq.each(function () {
                endCellEdit(this, true);
            });
        },
        editCell: function (jq, param) {
            return jq.each(function () {
                editCell(this, param);
            });
        },
        isEditing: function (jq, index) {
            var opts = $.data(jq[0], 'datagrid').options;
            var tr = opts.finder.getTr(jq[0], index);
            return tr.length && tr.hasClass('datagrid-row-editing');
        },
        gotoCell: function (jq, param) {
            return jq.each(function () {
                gotoCell(this, param);
            });
        },
        enableCellEditing: function (jq) {
            return jq.each(function () {
                enableCellEditing(this);
            });
        },
        disableCellEditing: function (jq) {
            return jq.each(function () {
                disableCellEditing(this);
            });
        },
        enableCellSelecting: function (jq) {
            return jq.each(function () {
                enableCellSelecting(this);
            });
        },
        disableCellSelecting: function (jq) {
            return jq.each(function () {
                disableCellSelecting(this);
            });
        },
        input: function (jq, param) {
            if (!param) {
                return null;
            }
            var ed = jq.datagrid('getEditor', param);
            if (ed) {
                var t = $(ed.target);
                if (t.hasClass('textbox-f')) {
                    t = t.textbox('textbox');
                }
                return t;
            } else {
                return null;
            }
        },
        cell: function (jq) { // get current cell info {index,field}
            return getCurrCell(jq[0]);
        },
        getSelectedCells: function (jq) {
            return getSelectedCells(jq[0]);
        },
        mfClearRows: function (jq) {
            var opts = $(jq[0]).datagrid('options');
            $(jq[0]).datagrid('loadData', { total: 0, rows: [] })
            opts.oldDatas.length = 0;
            opts.updateRows.length = 0;
        },
        mfGetSelections: function (jq) {
            var selectedCells = getSelectedCells(jq[0]);
            var rows = $(jq[0]).datagrid('getRows');
            var opts = $(jq[0]).datagrid('options');
            var selectedRows = [];
            $.each(selectedCells, function (index, item) {
                selectedRows.push(rows[item.index])
            });

            if (selectedRows.length == 0)
            {
                return $(jq[0]).datagrid('getSelections');
            }

            return selectedRows;
        },
        mfInsertRow: function (jq, index) {
            return jq.each(function () {
                var dg = $(this);
                var opts = $.data(this, 'datagrid').options;
                if (endCellEdit(this, true)) {
                    var rows = dg.datagrid('getRows');

                    function _add(index, row) {
                        if (index == undefined) {
                            if (opts.idField == 'mfid' && !row['mfid'])
                                row['mfid'] = guid();

                            dg.datagrid('appendRow', row);
                            //opts.editIndex = rows.length - 1;
                        } else {
                            if (opts.idField == 'mfid' && !row['mfid'])
                                row['mfid'] = guid();

                            dg.datagrid('insertRow', { index: index, row: row });
                            //opts.editIndex = index;
                        }
                    }
                    if (typeof index == 'object') {
                        _add(index.index, $.extend(index.row, {}))
                    } else {
                        _add(index, {});
                    }

                    opts.onInsertRow.call(this, index, rows[index]);
                }
            });
        },
        mfUpdateRow: function (jq, data) {
            return jq.each(function () {
                var dg = $(this);
                var opts = $.data(this, 'datagrid').options;

                dg.datagrid('updateRow', data);
                var row = dg.datagrid('getRows')[data.index];
                var inserts = dg.datagrid('getChanges', "inserted");
                if (!opts.updateRows.contains(row) && !inserts.contains(row)) {
                    opts.updateRows.push(row)
                }
            });
        },
        mfRejectChanges: function (jq) {
            return jq.each(function () {
                var dg = $(this);
                var opts = $.data(this, 'datagrid').options;

                dg.datagrid('rejectChanges');
                opts.updateRows = [];
            });
        },
        mfDeleteRow: function (jq, index) {
            return jq.each(function () {
                var dg = $(this);
                var opts = $.data(this, 'datagrid').options;

                if (endCellEdit(this, true)) {
                    var rows = [];
                    if (index == undefined) {
                        rows = dg.datagrid('mfGetSelections');
                    } else {
                        var rowIndexes = $.isArray(index) ? index : [index];
                        for (var i = 0; i < rowIndexes.length; i++) {
                            var row = opts.finder.getRow(this, rowIndexes[i]);
                            if (row) {
                                rows.push(row);
                            }
                        }
                    }

                    var rowarr = dg.datagrid('getData').rows;
                    var rowsLength = rows.length;
                    for (var i = 0; i < rowsLength; i++) {

                        var Index = rowarr.indexOf(rows[0]) - (dg.datagrid('getRows').length - opts.oldDatas.length);
                        //判断元数据是否有数据被删除?
                        if (Index >= 0) {
                            //删除元数据中被删除的数据
                            opts.oldDatas.splice(Index, 1);
                            //删除更新数组中被删除的数据 因为既然数据已经被删除我们就没必要在将该数据拿到后台进行更新操作了 直接删除就可以了
                            opts.updateRows.remove(rows[0])
                        }
                        //删除指定行
                        dg.datagrid('deleteRow', rowarr.indexOf(rows[0]));
                        rowarr = dg.datagrid('getData').rows;
                    }
                    dg.datagrid('clearSelections');
                }
            });
        },
        mfSetSaveUrl: function (jq,url) {
            return jq.each(function () {
                var opts = $.data(this, 'datagrid').options;
                opts.saveUrl = url;
            });
        },
        mfSaveRows: function (jq, bIgoreState) {
            return jq.each(function () {
                var dg = $(this);
                var opts = $.data(this, 'datagrid').options;

                if (endCellEdit(this, true)) {
                    var inserted = dg.datagrid('getChanges', "inserted");
                    var deleted = dg.datagrid('getChanges', "deleted");
                    var updated = opts.updateRows;

                    var rows = dg.datagrid('getData').rows;
                    $.each(rows, function (index, item) {
                        if (inserted.contains(item)) {
                            item['state'] = 'insert';
                        }
                        else if (updated.contains(item)) {
                            item['state'] = 'update';
                        }
                        else if (deleted.contains(item)) {
                            item['state'] = 'delete';
                        }
                        delete item['mfid'];
                    });

                    var url = opts.saveUrl;
                    if (url) {
                        var changed = (inserted.length != 0 || deleted.length != 0 || updated.length != 0);
                        if (bIgoreState) changed = true;

                        if (changed) {
                            addAjaxParam(opts.saveParamName, rows)
                            commitAjax(url, {
                                CallBack: function (data) {
                                    if (!data) {
                                        opts.onError.call(this);
                                        return;
                                    }

                                    dg.datagrid('reload');
                                    dg.datagrid('clearSelections');
                                    opts.oldDatas = jQuery.extend(true, [], dg.datagrid('getData').rows);//克隆旧数据 要不然修改标记不会被消除

                                    opts.onSuccess.call(this);
                                }
                            });
                        }
                    }
                }
            });
        },
        mfMoveUp: function (jq) {
            return jq.each(function () {
                var dg = $(this);
                var rows = dg.datagrid('getSelections');
                $.each(rows, function (index, item) {
                    item['index'] = dg.datagrid('getRowIndex', item);
                })
                rows = rows.sort(function (a, b) { return a.index - b.index; });
                for (var i = 0; i < rows.length; i++)
                {
                    var res = _mfSort(rows[i].index, 'up', dg);
                    if (!res) break;
                }             
            });
        },
        mfMoveDown: function (jq)
        {
            return jq.each(function () {
                var dg = $(this);
                var rows = dg.datagrid('getSelections');
                $.each(rows, function (index, item) {
                    item['index'] = dg.datagrid('getRowIndex', item);
                })
                rows = rows.sort(function (a, b) { return b.index - a.index; });
                for (var i = 0; i < rows.length; i++) {
                    var res = _mfSort(rows[i].index, 'down', dg);
                    if (!res) break;
                }
            });
        },
        mfValidate:function(jq)
        {
            var dg = $(jq[0]);
            var isValid = true;
            //编辑状态下验证
            var rows = dg.datagrid('getRows');
            var fields = dg.datagrid('getColumnFields', true).concat(dg.datagrid('getColumnFields'));

            $.each(rows, function (index, row) {
                if (!dg.datagrid('validateRow', index)) {
                    isValid = false;
                    return false;
                }
                    //验证required
                else {
                    $.each(fields, function (index1, field) {
                        var colOpts = dg.datagrid('getColumnOption', field);

                        if ((colOpts
                            && colOpts.editor
                            && colOpts.editor.options
                            && colOpts.editor.options.required)
                            && (!row[field] || row[field] == '')) {
                            isValid = false;
                            return false;
                        }
                    })
                }
            })
            return isValid;      
        }
    });

})(jQuery);

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

//删除
Array.prototype.remove = function (val) {
    var index = this.indexOf(val);
    if (index > -1) {
        this.splice(index, 1);
    }
}