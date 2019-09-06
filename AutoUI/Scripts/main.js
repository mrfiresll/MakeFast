$(function () {

    //固定南北 panel 分隔条
    $('#mainLayout').layout('panel', 'north').panel('panel').resizable('disable');
    //$('#mainLayout').layout('panel', 'south').panel('panel').resizable('disable');
    tabClose();
    tabCloseEven();
    // LoadFirsUnReadMessage();

    LoadTopModules();
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
                    var aHtml = "<div onclick='LoadAccordTree(\"" + e.id + "\",\"" + e.text + "\")' ";
                    aHtml += "class='easyui-menubutton' ";
                    aHtml += "data-options='iconCls:\"" + e.iconCls + "\",hasDownArrow:false'>" + e.text + "</a>";
                    $("#headerMenuLeft").append(aHtml);

                })
                $.parser.parse();

                if (dataObj.length > 0) {
                    LoadAccordTree(dataObj[0].id, dataObj[1].text);
                }
            }
        })
}

function LoadAccordTree(topId, topName) {
    var $caption = $('<div class="theme-left-menu-caption"></div>')
                    .append('<span class="right iconfont theme-left-menu-switch">&#xf0025;</span>' + topName || '未命名');
    $('.theme-left-menu').html('');
    //$('.theme-left-menu').append($caption);
    commitAjax("/AutoUI/Main/GetChildrenFuncWithSub?id=" + topId,
         {
             CallBack: function (data) {
                 $.each(data, function (i, e) {
                     var id = e.id;

                     var $dl = $("<dl><dt>" + e.text + "</dt></dl>");
                     var $dd = $("<dd></dd>");
                     var $ul = $("<ul></ul>");
                     $.each(e.children, function (index, item) {
                         var action = "addTab(tabTitle, url, '', true)";
                         var $li = $("<li><a href='#' onclick='addTab(\"" + item.text + "\",\"" + item.url + "\",\"\",true)'>" + item.text + "</a></li>");
                         $ul.append($li);
                     })
                     $dl.append($dd.append($ul));
                     $('.theme-left-menu').append($dl);

                 });
                 bindMenuEvent();
             }
         })
}

function bindMenuEvent() {
    $('.theme-header-navigate-combobox').each(function () {
        $(this).combo('panel').panel({ cls: "theme-header-navigate-combobox-panel" });
    });

    var theme_left_layout = $(".theme-layout").layout("panel", 'west');
    var theme_left_menu_switch = true;
    $(".theme-left-menu-switch").on("click", function (event) {
        if (theme_left_menu_switch) {
            $(".theme-left-user-panel").hide(); /*隐藏左侧菜单用户面板*/
            $(".theme-left-menu dl").hide(); /*隐藏左侧子菜单*/

            theme_left_layout.panel('resize', { width: 1 });
            $(".theme-left-menu").css({ "width": "1px" });
            $(".theme-header-logo").hide();
            theme_left_menu_switch = false;
        } else {
            $(".theme-left-user-panel").show(); /*显示左侧菜单用户面板*/
            $(".theme-left-menu dl").show(); /*显示左侧子菜单*/

            theme_left_layout.panel('resize', { width: 180 });
            $(".theme-left-menu").css({ "width": "180px" });
            $(".theme-header-logo").show();
            theme_left_menu_switch = true;
        }
        $(".theme-layout").layout('resize', { width: '100%' }); /*重置框架*/
    });

    var theme_left_menu_switch_hide = true;
    $(".theme-left-menu-switch-hide").on("click", function (event) {
        if (theme_left_menu_switch_hide) {
            //theme_left_layout.panel('resize',{width:1});
            $(".theme-layout").layout('remove', 'west');
            //$(".theme-left-layout").addClass('theme-left-layout-hide');
            theme_left_menu_switch_hide = false;
        } else {
            //theme_left_layout.panel('resize',{width:180});
            $(".theme-layout").layout('add', {
                region: 'west',
                bodyCls: 'theme-left-layout',
                href: 'menu_hide_left_content.html',
                border: false,
                width: 200
            });
            //$(".theme-left-layout").removeClass('theme-left-layout-hide');
            theme_left_menu_switch_hide = true;
        }
        $(".theme-layout").layout('resize', { width: '100%' }); /*重置框架*/
    });


    $(".theme-left-menu dl dt,.theme-inside-left-menu dl dt").on("click", function (event) {
        if (theme_left_menu_switch) {
            var node = $(this).next("dd");
            if (node.is(":hidden")) {
                node.show(); /*如果元素为隐藏,则将它显现*/
            } else {
                node.hide(); /*如果元素为显现,则将其隐藏*/
            }
        }
    });

    /*
	$(".theme-left-menu dl dt").on("mousemove",function(event) {
		if(!theme_left_menu_switch){
			var node=$(this).next("dd");
			node.addClass(".theme-left-menu-node-show");
			
		}
	});
	*/

    $(".theme-left-menu li").on("click", function (event) {
        $(".theme-left-menu li").removeClass("selected");
        $(this).addClass("selected");
    });
    $(".theme-inside-left-menu li").on("click", function (event) {
        $(".theme-inside-left-menu li").removeClass("selected");
        $(this).addClass("selected");
    });


    /*
	setInterval(function(){
		var nowDate  = new Date();
		var nowYear  = nowDate.getFullYear();
		var nowMonth = nowDate.getMonth().toString().length==1?"0"+nowDate.getMonth():nowDate.getMonth();
		var nowDays  = nowDate.getDate().toString().length==1?"0"+nowDate.getDate():nowDate.getDate();

		var nowHours = nowDate.getHours().toString().length==1?"0"+nowDate.getHours():nowDate.getHours();
		var nowMinute  = nowDate.getMinutes().toString().length==1?"0"+nowDate.getMinutes():nowDate.getMinutes();
		var nowSeconds  = nowDate.getSeconds().toString().length==1?"0"+nowDate.getSeconds():nowDate.getSeconds();

	    $("#theme-header-navigate-datetime").html(nowYear+"年"+nowMonth+"月"+nowDays+"日 "+nowHours+":"+nowMinute+":"+nowSeconds);
	},1000);
	*/


};

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