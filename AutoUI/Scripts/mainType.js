function beginEditNode(node) {
    if (node.parentId) {
        $(this).tree('beginEdit', node.target);
    }

}

function endEditNode(node) {
    addAjaxParam("Id", node.id);
    addAjaxParam("Text", node.text);
    commitAjax('/AutoUI/ConfigUIDef/MainType/UpdateMainTypeName', {
        CallBack: function (data) {

        }
    })
}

function showContextMenu(e, node) {
    e.preventDefault();
    $(this).tree('select', node.target);
    $('#mm').menu('show', {
        left: e.pageX,
        top: e.pageY
    })
}

function appendNode() {
    var t = $('#dgTree');
    var node = t.tree('getSelected');
    addAjaxParam("ParentId", node.id);
    addAjaxParam("FullId", node.fullId);
    addAjaxParam("DBName", node.dbName);
    commitAjax('/AutoUI/ConfigUIDef/MainType/AddMainType', {
        CallBack: function (data) {
            if (data) {
                t.tree('append', {
                    parent: (node ? node.target : null),
                    data: [{
                        id: data.Id,
                        text: data.Text,
                        parentId: node.id,
                        dbName: data.DBName,
                        fullId: data.FullId
                    }]
                });
            }
        }
    });

}

function removeNode() {
    var t = $('#dgTree');
    var node = t.tree('getSelected');
    addAjaxParam("FullId", node.fullId);
    commitAjax('/AutoUI/ConfigUIDef/MainType/RemoveMainType', {
        CallBack: function (data) {
            if (data) {
                if (node.parentId) {
                    t.tree('remove', node.target);
                }
                else {
                    $.each(t.tree('getChildren', node.target), function (index, item) {
                        t.tree('remove', item.target);
                    })
                }
            }
        }
    });
}

function collapse() {
    var node = $('#dgTree').tree('getSelected');
    $('#dgTree').tree('collapse', node.target);
}
function expand() {
    var node = $('#dgTree').tree('getSelected');
    $('#dgTree').tree('expand', node.target);
}

function selectNode(node) {
    //top
    if (!node.parentId || node.parentId == '') {
        $('#mf_grid').datagrid('load', {
            DBName: '',
            MainType: ''
        });
    }
    else {
        $('#mf_grid').datagrid('load', {
            DBName: node.text,
            MainType: node.id
        });
    }

    if (typeof (mainTypeTreeNodeSelect) != "undefined")
        mainTypeTreeNodeSelect(node);
}

function onBeforeDrop(targetRowDom, sourceRow, point) {
    var targetRow = $('#dgTree').tree('getNode', targetRowDom);
    //模块节点不能做任何调整
    if (sourceRow.dbName == sourceRow.id){
        return false;
    }

    //目标不能为模块节点
    if (targetRow.dbName == targetRow.id) {
        return false;
    }

    //不能为顶部的同级节点
    if (point == 'top' || point == 'bottom') {
        var parentId = $('#dgTree').tree('getNode', targetRowDom).parentId;
        if (!parentId) return false;
    }
}

function onDrop(targetRowDom, sourceRow, point) {
    var targetId = $('#dgTree').tree('getNode', targetRowDom).id;

    addAjaxParam("sourceID", sourceRow.id);
    addAjaxParam("targetID", targetId);
    addAjaxParam("point", point);
    commitAjax("/AutoUI/ConfigUIDef/MainType/MoveMainType");
}