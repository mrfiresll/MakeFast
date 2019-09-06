$(function () {
    quickPanels();
});

//快捷面板
function quickPanels() {
    $('#pp').portal({
        border: false,
        //fit:true
    });

    //面板1 我的作业项目清单
    var projPanel = $('<div><table id="projGrid"></table></div>').appendTo('body');
    projPanel.panel({
        title: '我的作业项目清单',
        height: 400,
        collapsible: true,
        onLoad: function () {
            $('#pp').portal('resize');
        }
    });
    $('#pp').portal('add', {
        panel: projPanel,
        columnIndex: 0
    });
    //面板2 我的审批
    var spPanel = $('<div><table id="spGrid"></table></div>').appendTo('body');
    spPanel.panel({
        title: '我的审批',
        height: 340,
        //href: '/WorkFlow/FlowInstManage/Index',
        collapsible: true,
        onLoad: function () {
            $('#pp').portal('resize');
        }
    });
    $('#pp').portal('add', {
        panel: spPanel,
        columnIndex: 0
    });
    InitSpGrid();
    InitProjGrid();
    //方式1 通过设置 $('#pp').portal('resize') 延迟执行，解决其适应慢的问题
    //问题现象可以通过替换为方式2，来进行演示。
    var timer = 0;
    $(window).resize(function () {
        clearTimeout(timer);
        timer = setTimeout(function () {
            $('#pp').portal('resize');
        }, 200);
    });

    //方式2
    //$(window).resize(function () {
    //$('#pp').portal('resize');
    //});
}

//我的审批列表初始化
function InitSpGrid() {
    ///alert(queryData);
    $("#spGrid").datagrid({
        //title: "工作流实例列表",
        rownumbers: true,
        //width: function () { return document.body.clientWidth * 0.9 },
        fit: true,
        singleSelect: true,
        nowrap: true,
        remoteSort: false,//前台排序   true则为后台排序 需要传参
        //idField: 'Id',
        sortOrder: 'asc',
        sortName: 'LatestOperateTime',
        striped: true,
        border: false,
        iconCls: 'icon icon-contract',
        url: '/WorkFlow/FlowInstManage/GetMyFlowInstOfNeedOperate',
        method: 'post',
        autoRowHeight: false,
        pagination: true,
        //toolbar:toolbar,
        //queryParams: queryData,
        pageSize: 10,
        frozenColumns: [[
            { field: 'Name', width: 400, title: "流程名称", sortable: true, },//sortable:true  动态排序
        ]],
        columns: [[
           
            { field: 'LastStaffName', width: 80, title: "审批人", sortable: true, align: 'center'},
            { field: 'LastCheckResult', width: 100, title: "审批结果", sortable: true, align: 'center'},
            {
                field: 'LatestOperateTime', width: 180, title: "审批时间", sortable: true, align: 'center',
                formatter: function (date) {
                    return date==null?"-":convertDateToyyyy_MM_dd_hh_mm_ss(date);
                }
            },
            { field: 'CurrentNodeMBName', width: 100, title: "当前节点", sortable: true, align: 'center'},
            { field: 'CurrentStaffName', width: 80, title: "待审批人", sortable: true, align: 'center'},
            {
                field: 'CreateTime', width: 180, title: "流程创建时间", sortable: true, align: 'center',
                formatter: function (date) {
                    return convertDateToyyyy_MM_dd_hh_mm_ss(date);
                }
            },
            { field: 'FlowStateStr', width: 80, title: "流程状态", sortable: true, align: 'center'},
            {
                field: 'operate', title: '操作', align: 'center', width: 100,
                formatter: function (value, row, index) {
                    var url = '/WorkFlow/FlowInstManage/ViewFlowInst?flowInstId=' + row.Id;
                    var str = '<a href="' + url + '" name="opera">查看</a>';
                    return str;
                }
            }
        ]],

    })
};
//我的作业项目清单
function InitProjGrid(queryData) {
    $("#projGrid").datagrid({
        //title: "工作流实例列表",
        rownumbers: true,
        //width: function () { return document.body.clientWidth * 0.9 },
        fit: true,
        singleSelect: true,
        nowrap: true,
        remoteSort: false,//前台排序 true则为后台排序 需要传参
        //idField: 'Id',
        striped: true,
        border: false,
        iconCls: 'icon icon-contract',
        url: '/BM/ProjInfoManage/GetMyProjListForDesk',
        method: 'post',
        autoRowHeight: false,
        pagination: true,
        //toolbar:toolbar,
        queryParams: queryData,
        pageSize: 10,
        columns: [[
            { field: 'ProjName', title: '项目名称', width: 400, sortable: true, rowspan: 2 },
            { field: 'Num', width: 150, title: "档案编号", sortable: true, rowspan: 2, align: 'center' },
            {
                field: 'operate', title: '过程详细内容', align: 'center', width: 100, sortable: true, rowspan: 2,
                formatter: function (value, row, index) {
                    var url = '/BM/ProjInfoManage/TabProjStaffWorkLog?id=' + row.Id + '&Num' + row.Num;
                    var str = '<a href="' + url + '" name="opera">添加</a>';
                    return str;
                }
            },
            {
                field: 'operate1', title: '项目实施策划', align: 'center', sortable: true, width: 100, rowspan: 2,
                 formatter: function (value, row, index) {
                        if (row.ProjPlan != "未创建") {
                            var url = row.ProjPlanUrl == '' ? '' : '/WorkFlow/FlowInstManage/ViewFlowInst?flowInstId=' + row.ProjPlanUrl + '&backUrl=' + encodeURI(window.location.href);
                            var str = '<a href="' + url + '" name="opera">' + row.ProjPlan + '</a>';
                        }
                        else {
                            var addUrl = row.ProjPlanUrl == '' ? '' : '/BM/ProjInfoManage/AddTab4_TabProjProgSign?projId=' + row.Id;
                            var tabName = "项目实施策划(表4)";
                            var str = '<a href="#" name="opera" style="color:red" onclick="addFlowInstTab(\'' + tabName + '\',\'' + addUrl + '\')" >' + row.ProjPlan + '</a>';

                        }
                        return str;
                    }
                
            },
            {
                field: 'operate2', sortable: true, width: 120, title: "合作/协作方评审", align: 'center', rowspan: 2,
                formatter: function (value, row, index) {
                    if (row.CoopOrCollProjReview != "未创建") {
                        var url = '/WorkFlow/FlowInstManage/ViewFlowInst?flowInstId=' + row.CoopOrCollProjReviewUrl + '&backUrl=' + encodeURI(window.location.href);
                        var str = '<a href="' + url + '" name="opera">' + row.CoopOrCollProjReview + '</a>';
                    }
                    else {
                        var addUrl = row.CoopOrCollProjReviewUrl == '' ? '' : '/BM/ProjInfoManage/AddTab5_TabProjCoopOrCollReview?projId=' + row.Id;
                        var tabName = "合作/协作方评审(表5)";
                        var str = '<a href="#" name="opera" style="color:red" onclick="addFlowInstTab(\'' + tabName + '\',\'' + addUrl + '\')" >' + row.CoopOrCollProjReview + '</a>';
                    }

                    return str;
                }
            },
           { title: '产品检验成果流转单', colspan: 2 },
           {
               field: 'operate3', title: '项目清册登记', align: 'center', width: 160, sortable: true, rowspan: 2,
               formatter: function (value, row, index) {
                   var url = '/BM/ProjInfoManage/EditProjInfo?id=' + row.Id;
                   var str = '<a href="' + url + '" name="opera" >查看(信息完整率：' + row.CompletedRate + '）</a>';
                   return str;
               }
           }
        ],
           [
           {
               field: 'operate4', title: '安装', width: 80, align: 'center', rowspan: 1,
               formatter: function (value, row, index) {
                   if (row.ProjResultCheckStep_A != "未创建") {
                       var url = '/WorkFlow/FlowInstManage/ViewFlowInst?flowInstId=' + row.ProjResultCheckStep_AUrl + '&backUrl=' + encodeURI(window.location.href);
                       var str = '<a href="' + url + '" name="opera">' + row.ProjResultCheckStep_A + '</a>';
                   }
                   else {
                       var addUrl = row.ProjResultCheckStep_AUrl == '' ? '' : '/BM/ProjInfoManage/AddTab6_TabProjResultCheckStep?projId=' + row.Id + "&jA=" + 1;
                       var tabName = "产品检验成果流转单(安装表6)";
                       var str = '<a href="#" name="opera" style="color:red" onclick="addFlowInstTab(\'' + tabName + '\',\'' + addUrl + '\')" >' + row.ProjResultCheckStep_A + '</a>';
                   }
                   return str;
               },
           },
           {
               field: 'operate5', title: '非安装', width: 80, align: 'center', rowspan: 1,
               formatter: function (value, row, index) {
                   if (row.ProjResultCheckStep_J != "未创建") {
                       var url = '/WorkFlow/FlowInstManage/ViewFlowInst?flowInstId=' + row.ProjResultCheckStep_JUrl + '&backUrl=' + encodeURI(window.location.href);
                       var str = '<a href="' + url + '" name="opera">' + row.ProjResultCheckStep_J + '</a>';
                   }
                   else {
                      var addUrl = row.ProjResultCheckStep_JUrl == '' ? '' : '/BM/ProjInfoManage/AddTab6_TabProjResultCheckStep?projId=' +row.Id + "&jA=" +0;
                      var tabName = "产品检验成果流转单(非安装表6)";
                      var str = '<a href="#" name="opera" style="color:red" onclick="addFlowInstTab(\'' + tabName + '\',\'' + addUrl + '\')" >' + row.ProjResultCheckStep_J + '</a>';
                   }
                  return str;
               },
           },

           ]],
        toolbar: "#tb1",
    })
    };
function addFlowInstTab(tabName, addUrl) {
    $.messager.confirm('提示', tabName + '不存在,是否增加？', function (r) {
        if(r) {
            $.post(addUrl,
                function (data) {
                    var dataObj = eval('(' +data + ')');
                    if (dataObj.Success) {
                        window.location.href = '/WorkFlow/FlowInstManage/StartSpecialFlowInst?flowInstId=' + dataObj.Data + '&backUrl=' + encodeURI(window.location.href);
                        //addTab(tabName, url);
                    }
                    else {
                        $.messager.alert('提示', '增加失败');
                    }
                });
        }
    })
    }

function searchData() {
    var queryData = {
            SProjName: $("#txtProjName").val(),
            SNum: $("#txtProjNum").val(),
        }
    InitProjGrid(queryData)
    }
function clearData() {
    $("#txtProjName").val("");
    $("#txtProjNum").val("");
}

