var wfStyleConfig = {
    visoTemplate: {}
}

//wfStyleConfig.visoTemplate.playAudioNode = '<div class="pa" id="{{id}}" style="top:{{top}}px;left:{{left}}px"><a class="btn btn-success" href="#" role="button">放音</a></div>'

wfStyleConfig.DotPaintStyle = {
    outlineStroke: 'gray',
    strokeWidth: 0,
    'stroke-dasharray': '5,5' // 用于创建虚线
};

wfStyleConfig.connectorPaintStyle = {
    outlineStroke: 'gray',
    strokeWidth: 0
};

wfStyleConfig.connectorHoverStyle = {
    outlineStroke: 'blue',
    strokeWidth: 2
};

wfStyleConfig.baseStyle = {
    //endpoint: ['Dot', {
    //    radius: 8,
    //    fill: 'pink'
    //}], // 端点的形状
    connectorStyle: wfStyleConfig.connectorPaintStyle, // 连接线的颜色，大小样式
    connectorHoverStyle: wfStyleConfig.connectorHoverStyle,
    paintStyle: {
        strokeStyle: '#1e8151',
        stroke: '#7AB02C',
        fill: 'pink',
        fillStyle: '#1e8151',
        radius: 4
    }, // 端点的颜色样式
    hoverPaintStyle: {
        stroke: 'blue',
        fill: 'blue',
        outlineStroke: 'blue'
     },
    isSource: true, // 是否可以拖动（作为连线起点）
    connector: ['Straight'],  // 连接线的样式种类有[Bezier],[Flowchart],[StateMachine ],[Straight ]
    isTarget: true, // 是否可以放置（连线终点）
    maxConnections: -1, // 设置连接点最多可以连接几条线
    connectorOverlays: [
      ['Arrow', {
          fill: 'gray',
          width: 10,
          length: 10,
          location: 1
      }],
      //['Label', {
      //    label: 'X',
      //    cssClass: 'arrowLabel',
      //    labelStyle: {
      //        color: 'red'
      //    },
      //    events: {
      //        click: function (labelOverlay, originalEvent) {
      //            confirmBox('是否删除', function (b) {
      //                if (b) {
      //                    jsPlumb.deleteConnection(labelOverlay.component);
      //                }
      //            })                  
      //        }
      //    }
      //}]
    ]
}