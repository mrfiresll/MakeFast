$(function () {

    //固定南北 panel 分隔条
    $('#mainLayout').layout('panel', 'north').panel('panel').resizable('disable');
    //$('#mainLayout').layout('panel', 'south').panel('panel').resizable('disable');
    tabClose();
    tabCloseEven();
    // LoadFirsUnReadMessage();

    InitLeftTree();
    LoadTopModules();
    LoadFirstAccordTree();
    addTab('首页', '/AutoUI/Main/Home', 'iconfont iconfont-shouye', false);
    //getEmailCount();
    //createRightBotDialog();
    //GetIntervalTime();

    // LoadFirsUnReadMessage();
});
function getEmailCount() {   //获取未读消息条数，并显示在红圈中
    commitAjax("/Sys/LanEmailManage/GettUnReadEmailCount",
      {
          CallBack: function (data) {
              var info = data.toJSON();
              if (info == 0) {
                  $('#emailCount').html('');
                  $('#emailCount').removeClass('unread');
              }
              else {
                  if (!$('#emailCount').hasClass('unread')) {
                      $('#emailCount').addClass('unread');
                  }
                  $('#emailCount').html(info);
              }
          }
      })
}

function LoadTopModules() {
    commitAjax("/AutoUI/Main/GetModules",
        {
            CallBack: function (data) {
                var dataObj = data;
                $.each(dataObj, function (i, e) {
                    var aHtml = "<div onclick='LoadAccordTree(" + e.id + ")' ";
                    aHtml += "class='easyui-menubutton' ";
                    aHtml += "data-options='iconCls:\"" + e.iconCls + "\",hasDownArrow:false'>" + e.text + "</a>";
                    $("#headerMenuLeft").append(aHtml);
                    //对于后添加easyui元素需要再次渲染而调用的函数。
                    $.parser.parse();
                })
            }
        })
}

function LoadFirstAccordTree() {
    Clearnav();
    commitAjax("/AutoUI/Main/GetFirstMenus",
         {
             CallBack: function (data) {
                 if (data == "0")
                     window.location = "url";

                 $.each(data, function (i, e) {
                     var id = e.id;
                     $("#westAccordion").accordion('add', {
                         title: e.text,
                         content: "<ul id='tree" + id + "'></ul>",
                         selected: true,
                         iconCls: e.iconCls
                     });
                     //$.parser.parse();//再次加载easyUI

                     $("#tree" + id).tree({
                         url: "/AutoUI/Main/GetChildrenFunc?id=" + id,
                         onClick: function (node) {
                             var tabTitle = node.text;
                             var url = node.url;
                             //var icon = node.iconCls;
                             addTab(tabTitle, url, '', true);
                         }
                     });
                 });
             }
         })
}

function LoadAccordTree(topId) {
    Clearnav();
    commitAjax("/AutoUI/Main/GetChildrenFunc?id="+ topId,
         {
             CallBack: function (data) {
                 if (data == "0")
                     window.location = "url";

                 $.each(data, function (i, e) {
                     var id = e.id;
                     $("#westAccordion").accordion('add', {
                         title: e.text,
                         content: "<ul id='tree" + id + "'></ul>",
                         selected: true,
                         iconCls: e.iconCls
                     });
                     //$.parser.parse();//再次加载easyUI

                     $("#tree" + id).tree({
                         url: "/AutoUI/Main/GetChildrenFunc?id=" + id,
                         onClick: function (node) {
                             var tabTitle = node.text;
                             var url = node.url;
                             //var icon = node.iconCls;
                             addTab(tabTitle, url, '', true);
                         }
                     });
                 });
             }
         })
}

// 初始化左侧
function InitLeftTree() {

    // 导航菜单绑定初始化
    $("#westAccordion").accordion({
        fillSpace: true,
        fit: true,
        border: false,
        animate: false
    });
    hoverMenuItem();

    //$('#wnav li a').live('click', function () {
    //    var tabTitle = $(this).children('.nav').text();

    //    var url = $(this).attr("rel");
    //    var menuid = $(this).attr("ref");
    //    var icon = getIcon(menuid, icon);

    //    addTab(tabTitle, url, icon);
    //    $('#wnav li div').removeClass("selected");
    //    $(this).parent().addClass("selected");
    //});

}

/**
 * 菜单项鼠标Hover
 */
function hoverMenuItem() {
    $(".easyui-accordion").find('options').hover(function () {
        $(this).parent().addClass("hover");
    }, function () {
        $(this).parent().removeClass("hover");
    });
}

function Clearnav() {
    var pp = $('#westAccordion').accordion('panels');

    $.each(pp, function (i, n) {
        if (n) {
            var t = n.panel('options').title;
            $('#westAccordion').accordion('remove', t);
        }
    });

    pp = $('#westAccordion').accordion('getSelected');
    if (pp) {
        var title = pp.panel('options').title;
        $('#westAccordion').accordion('remove', title);
    }
}

function addTab(subtitle, url, icon, noClose) {
    if (!$('#tabs').tabs('exists', subtitle)) {
        $('#tabs').tabs('add', {
            title: subtitle,
            content: createFrame(url),
            closable: noClose,
            icon: icon
        });
    } else {
        $('#tabs').tabs('select', subtitle);
        $('#mm-tabupdate').click();
    }
    tabClose();
}

function createFrame(url) {
    //var iframdHeight = parseInt(this.$tabDiv.height());
    //var iframeSrc = opts.src.indexOf('?') >= 0 ? opts.src + "&tabid=" + tabID : opts.src + "?tabid=" + tabID;
    //var $iframe = $('<iframe id="' + tabID + '_iframe" name="' + tabID + '_iframe" frameborder="0" scrolling="auto" style="width:100%;' + (iframdHeight ? "height:" + (iframdHeight - 26).toString() + "px" : "") + '" src="' + iframeSrc + '"></iframe>');

    var s = '<iframe scrolling="auto" frameborder="0" name="tabFrame"  id="tabFrame" src="' + url + '" style="width:100%;height:100%;"></iframe>';
    //s += '@StackExchange.Profiling.MiniProfiler.RenderIncludes()';
    return s;
}

function tabClose() {
    /* 双击关闭TAB选项卡 */
    $(".tabs-inner").dblclick(function () {
        var subtitle = $(this).children(".tabs-closable").text();
        $('#tabs').tabs('close', subtitle);
    });
    /* 为选项卡绑定右键 */
    $(".tabs-inner").bind('contextmenu', function (e) {
        $('#mm').menu('show', {
            left: e.pageX,
            top: e.pageY
        });

        var subtitle = $(this).children(".tabs-closable").text();

        $('#mm').data("currtab", subtitle);
        $('#tabs').tabs('select', subtitle);
        return false;
    });
}
// 绑定右键菜单事件
function tabCloseEven() {
    // 刷新
    $('#mm-tabupdate').click(function () {
        var currTab = $('#tabs').tabs('getSelected');
        var url = $(currTab.panel('options').content).attr('src');
        $('#tabs').tabs('update', {
            tab: currTab,
            options: {
                content: createFrame(url)
            }
        });
    });
    // 关闭当前
    $('#mm-tabclose').click(function () {
        var currtab_title = $('#mm').data("currtab");
        if (currtab_title == '首页') return;
        $('#tabs').tabs('close', currtab_title);
    });
    // 全部关闭
    $('#mm-tabcloseall').click(function () {
        $('.tabs-inner span').each(function (i, n) {
            var t = $(n).text();
            if (t == '首页') return true;
            $('#tabs').tabs('close', t);
        });
    });
    // 关闭除当前之外的TAB
    $('#mm-tabcloseother').click(function () {
        $('#mm-tabcloseright').click();
        $('#mm-tabcloseleft').click();
    });
    // 关闭当前右侧的TAB
    $('#mm-tabcloseright').click(function () {
        var nextall = $('.tabs-selected').nextAll();
        if (nextall.length == 0) {
            // msgShow('系统提示','后边没有啦~~','error');
            //alert('后边没有了');
            return false;
        }
        nextall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            if (t == '首页') return true;
            $('#tabs').tabs('close', t);
        });
        return false;
    });
    // 关闭当前左侧的TAB
    $('#mm-tabcloseleft').click(function () {
        var prevall = $('.tabs-selected').prevAll();
        if (prevall.length == 0) {
            //alert('到头了');
            return false;
        }
        prevall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            if (t == '首页') return true;
            $('#tabs').tabs('close', t);
        });
        return false;
    });

    // 退出
    $("#mm-exit").click(function () {
        $('#mm').menu('hide');
    });
}

