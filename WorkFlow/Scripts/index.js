var wkfSettingAreaId = "diagramContainer";
var workFlowDefId = queryString('id');
var flowDataExt = { nodes: [], connects: [] };

$(function () {
    jsPlumb.ready(main);
    jsPlumb.importDefaults({
        ConnectionsDetachable: false //一般来说拖动创建的链接，可以再次拖动，让链接断开。如果不想触发这种行为，可以设置false
    })
})

function main() {
    //左侧备选区
    $('.wkfNode').draggable({
        helper: 'clone',
        scope: 'dragGroupIndex1',
        containment: '#' + wkfSettingAreaId
    });
    //右侧节点布置区
    $('#' + wkfSettingAreaId).droppable({
        scope: 'dragGroupIndex1',//当选中多个节点的时候节点组
        drop: function (event, ui) {
            var pos = { Left: ui.position.left, Top: ui.position.top };
            addNode(ui.draggable[0].dataset.template, pos);
        }
    })

    // 单点击了连接线
    jsPlumb.bind('click', function (conn, originalEvent) {
        if (openConnectDetailWindow)
        {
            var id = conn.sourceId + '_' + conn.targetId;
            openConnectDetailWindow(id, conn);
        }
    })

    //当建立连接
    jsPlumb.bind('beforeDrop', function (e) {
        var sNodeAnchor = e.connection.endpoints[0].anchor.type;
        var eNodeAnchor = e.connection.endpoints[1].anchor.type;

        flowDataExt.connects.push({
            Id: e.sourceId + '_' + e.targetId,
            SNodeDefId: e.sourceId,
            SNodeAnchor: sNodeAnchor,
            ENodeDefId: e.targetId,
            ENodeAnchor: eNodeAnchor,
            Name: '送下一环节',
            WFDefId: workFlowDefId
        });

        return true;
    })

    if (typeof (loadFlowDesign) != "undefined")
        loadFlowDesign();
}

function getBaseNodeConfig() {
    return wfStyleConfig.baseStyle;
}

//-----------------------flowapi------------------------------start
//增加节点
function addNode(template, node, isLoad) {
    if (!node.Id || node.Id == '')
        node.Id = uuid.v1();

    flowDataExt.nodes.push(node);
    var anchors = [];
    if (template.indexOf('Start') != -1)
    {
        anchors = ['RightMiddle'];
    }
    else if (template.indexOf('Process') != -1)
    {
        anchors = ['LeftMiddle', 'RightMiddle'];
    }
    else if (template.indexOf('End') != -1)
    {
        anchors = ['LeftMiddle'];
    }

    var html = Mustache.render($('#tpl-' + template).html(), node);
    $('#' + wkfSettingAreaId).append(html)
    var config = getBaseNodeConfig();

    if (isLoad)//清空连线配置，由addConnect来配置
    {
        delete config.connectorStyle;
        delete config.connectorHoverStyle;
    }

    jsPlumb.draggable(node.Id, {
        containment: 'parent'
    })

    $.each(anchors, function (index, item) {
        jsPlumb.addEndpoint(node.Id, {
            anchors: item,
            uuid: node.Id + '_' + item
        }, config)
    })
}

function addConnect(item, sId, tId, isDot)
{
    var sourceId = sId + '_' + item.SNodeAnchor;
    var targetId = tId + '_' + item.ENodeAnchor;

    flowDataExt.connects.push(item);
    if (isDot)
    {
        jsPlumb.connect({ uuids: [sourceId, targetId], paintStyle: wfStyleConfig.DotPaintStyle, hoverPaintStyle: wfStyleConfig.DotPaintStyle });
    }
    else
    {
        jsPlumb.connect({ uuids: [sourceId, targetId], paintStyle: wfStyleConfig.connectorPaintStyle, hoverPaintStyle: wfStyleConfig.connectorPaintStyle });
    }    
}

function getAllNodes()
{
    var res = [];
    if ($('.startNode').length != 0)
    {
        var sId = $('.startNode').attr('id');
        var tmp = {
            Id: sId,
            Left: parseInt($('.startNode').css('left')),
            Top: parseInt($('.startNode').css('top')),
            WFNodeType: 'Start',
            Name: '开始',
            WFDefId: workFlowDefId
        };
        var nodeExtData = getNodeExtData(tmp.Id);
        jQuery.extend(true, nodeExtData, tmp);
        res.push(nodeExtData);
    }

    $.each($('.processNode'), function (i, e) {
        var sId = e.id;
        var tmp = {
            Id: sId,
            Left: parseInt(e.style.left),
            Top: parseInt(e.style.top),
            WFNodeType: 'Process',
            Name: $('#' + sId + 'Title').html(),
            WFDefId: workFlowDefId
        };
        var nodeExtData = getNodeExtData(tmp.Id);
        jQuery.extend(true, nodeExtData, tmp);
        res.push(nodeExtData);
    })

    if ($('.endNode').length != 0) {
        var tmp = {
            Id: $('.endNode').attr('id'),
            Left: parseInt($('.endNode').css('left')),
            Top: parseInt($('.endNode').css('top')),
            WFNodeType: 'End',
            Name: '结束',
            WFDefId: workFlowDefId
        };
        var nodeExtData = getNodeExtData(tmp.Id);
        jQuery.extend(true, nodeExtData, tmp);
        res.push(nodeExtData);
    }
    
    return res;
}

function getAllConnections() {
    var res = [];
    $.each(jsPlumb.getAllConnections(), function (i, e) {
        var sNodeAnchor = e.endpoints[0].anchor.type;
        var eNodeAnchor = e.endpoints[1].anchor.type;
        var tmp = {
            Id: e.sourceId + '_' + e.targetId,
            SNodeDefId: e.sourceId,
            SNodeAnchor: sNodeAnchor,
            ENodeDefId: e.targetId,
            ENodeAnchor: eNodeAnchor,
            WFDefId: workFlowDefId
        };
        var connectExtData = getConnectExtData(tmp.Id);
        jQuery.extend(true, connectExtData, tmp);
        res.push(connectExtData);
    })

   
    return res;
}

function deleteNode(id) {
    confirmBox('是否删除', function (b) {
        if (b) {
            jsPlumb.remove(id);
        }
    })
}

function deleteConnect(conn)
{
    jsPlumb.deleteConnection(conn);
}

function getNodeExtData(nodeId) {
    var ress = $.grep(flowDataExt.nodes, function (item, index) {
        return item.Id == nodeId;
    })
    if (ress.length > 0)
        return ress[0];
}

function getConnectExtData(connectId) {
    var ress = $.grep(flowDataExt.connects, function (item, index) {
        return item.Id == connectId;
    })
    if (ress.length > 0)
        return ress[0];
}
//-----------------------flowapi------------------------------end