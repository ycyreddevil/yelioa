<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mNetSalesApproval.aspx.cs" Inherits="mNetSalesApproval" %>
<%
    string type = Request["type"];
%>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/bootstrap/js/bootstrap.min.js"></script>
    <link href="Scripts/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="Scripts/ystep-master-v2.0/js/ystep.js"></script>
    <link href="Scripts/ystep-master-v2.0/css/ystep.css" rel="stylesheet" />
    <link href="Scripts/css.css" rel="stylesheet" />
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/jquery.easyui.mobile.js"></script>
    <script src="Scripts/mobileCommon.js"></script>
    <script src="Scripts/bootstrap-table/js/bootstrap-table.js"></script>
    <link href="Scripts/bootstrap-table/css/bootstrap-table.css" rel="stylesheet" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/bootstrap-step/bootstrap-step.js"></script>
    <link href="Scripts/bootstrap-step/bootstrap-step.css" rel="stylesheet" />
    <title>纯销单据审批</title>
    <style>
        input::-webkit-input-placeholder{
            text-align: center;
        }
        .form2_input{
            color:black;
        }
        form button {
            margin:10px 5px 15px 20px;
        }
        
    </style>
</head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
        background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog" border="false"
        noheader="true" closed="true" modal="true">
    </div>
    <div id="listTable" class="panel-body" style="padding-bottom:0px;">
        <div id="toolbar" class="btn-group">
            <%--<button id="btn_add" type="button" class="btn btn-default">
                <span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>返回
            </button>--%>
            <p id="overtitle" style="font-size:25px;margin-left:100px"></p>
            <button id="btn_edit" type="button" class="btn btn-default">
                <span class="glyphicon glyphicon-check" aria-hidden="true"></span>审批
            </button>
            <button id="btn_refresh" type="button" class="btn btn-default">
                <span class="glyphicon glyphicon-refresh" aria-hidden="true"></span>刷新
            </button>
        </div> 
        <table id="list"></table>
    </div>

    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            <h4 class="modal-title" id="myModalLabel">审批</h4>
            </div>
            <div class="modal-body">
            <textarea style="width:100%" placeholder="审批意见" id="approvalOption"></textarea>
            </div>
            <div class="modal-footer">
            <button type="button" class="btn btn-success" id="agree" >同意</button>
            <button type="button" class="btn btn-danger" id="decline">拒绝</button>
            <button type="button" class="btn" data-dismiss="modal">取消</button>
            </div>
        </div>
        </div>
    </div>

    <div class="modal fade" id="detailModal" tabindex="-1" role="dialog" aria-labelledby="detailModalLabel">
        <div class="modal-dialog" role="document">
        <div class="modal-content">
            <h4>
                <span class="span10">&nbsp;&nbsp;&nbsp;<strong id="title"></strong></span>
                <span class="btn-success" id="state"></span>
            </h4>
            <h4>
                <span id ="time"></span>&nbsp;
                <span id="sales"></span>&nbsp;
            </h4>
            <div >
                <input type="text" readonly class="form-control" value="医院"/>
                <input id="hospital" type="text" class="form-control form2_input" value=""/>
            </div>
            <div >
                <input type="text" readonly class="form-control" value="产品"/>
                <input id="product"  type="text" class="form-control form2_input" value=""/>
            </div>
            <div>
                <input type="text" readonly class="form-control" value="纯销数量"/>
                <input id="netSalesNum"  type="text" class="form-control form2_input" value=""/>
            </div>
            <%--<nav aria-label="...">
                <ul class="pager">
                <li class="previous"><a href="#"><span aria-hidden="true">&larr;</span> 上一个表单</a></li>
                <li class="active">1 <span class="sr-only">(current)</span></li>
                    /
                <li class="active">1 <span class="sr-only">(current)</span></li>
                <li class="next"><a href="#">下一个表单 <span aria-hidden="true">&rarr;</span></a></li>
                </ul>
            </nav>--%>
            <div>
                <input type="text" readonly class="form-control" value="审批流程"/>
                <div id="img_bag"> 
                   <div id="img" > 
                    <div style="position:relative;"> 
                     <div class="online"></div> 
                     <div class="ztb_main_01"> 
                      <ul class="ztb_content_01" id ="ystep"> 
                       <%--<li class="ztb_over"><a href="#" class="ztb_con_text">场景用例 </a>
                       </li> 
                       <li class="ztb_on"><a href="#" class="ztb_con_text">组件测试</a> 
                        </li> 
                       <li><a href="#" class="ztb_con_text">场景用例</a> 
                        </li>
                       <li><a href="#" class="ztb_con_text">组件测试</a> 
                        </li> --%>
                     </ul>
                     </div> 
                    </div> 
                   </div> 
                  </div> 
            </div>
            <div id="record">
                <input type="text" readonly class="form-control" value="审批记录"/>
                <ul class="list-group" id="approvalRecord">
                    
                </ul>
            </div>
            <button id="approval" type="button" class="btn btn-primary" style="margin-left:35%">审批</button>
            <button id="return" type="button" class="btn btn-primary" style="margin-left:35%">退回</button>
            <button id="cancel" type="button" class="btn btn-danger" data-dismiss="modal">取消</button>
        </div>
    </div>
    </div>
</body>
<script>
    var docCodes = new Array(); // 用于审批
    var docCode;

    //function IsPC() {
    //    var userAgentInfo = navigator.userAgent;
    //    var Agents = ["Android", "iPhone",
    //        "SymbianOS", "Windows Phone",
    //        "iPad", "iPod"];
    //    var flag = true;
    //    for (var v = 0; v < Agents.length; v++) {
    //        if (userAgentInfo.indexOf(Agents[v]) > 0) {
    //            flag = false;
    //            break;
    //        }
    //    }
    //    return flag;
    //}

    $(document).ready(function () {
        //InitDataList();
        // 加载bootstrap表格
        var oTable = new TableInit();
        oTable.init();

        // 当查询我提交的时候，把审批按钮隐藏，把退回按钮显示
        if('mine' == '<%=type%>'){
            $("#btn_edit").hide();
            $("#return").show();
            $("#approval").hide();
            $("#overtitle").html("我提交的单据列表")
        } else {
            $("#btn_edit").show();
            $("#return").hide();
            $("#approval").show();
            $("#overtitle").html("待我审批的单据列表")
        }
    })

    var TableInit = function () {
        var oTableInit = new Object();
        oTableInit.init = function () {
            $('#list').bootstrapTable({
                url: 'mNetSalesApproval.aspx',
                method: 'post',
                queryParams: oTableInit.queryParams,
                contentType: "application/x-www-form-urlencoded",
                //cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
                pagination: false,                   //是否显示分页（*）
                sortable: false,                     //是否启用排序
                sortOrder: "asc",                   //排序方式
                striped: "true",
                toolbar: "#toolbar",
                //search: false,                       //是否显示表格搜索，此搜索是客户端搜索，不会进服务端，所以，个人感觉意义不大
                //strictSearch: true,
                //showColumns: false,                  //是否显示所有的列
                //showRefresh: true,                  //是否显示刷新按钮
                //clickToSelect: true,                //是否启用点击选中行
                //uniqueId: "ID",                     //每一行的唯一标识，一般为主键列
                //showToggle: false,                    //是否显示详细视图和列表视图的切换按钮
                //cardView: false,                    //是否显示详细视图
                detailView: true,                   //是否显示父子表
                responseHandler: function (res) {
                    //console.log(res)
                    //res = $.parseJSON(res);
                    //console.log(res + "aaaaaa")
                    return res;
                },
                columns: [{
                    checkbox: true
                },{
                        field: 'group',
                        title: '医院名称',
                }, {
                        field: 'item',
                        title: '药品名称'
                }, {
                        field: 'flowNum',
                        title: '流向数量'
                }, {
                        field: 'NetSalesNum',
                        title: '纯销数量'
                }],
                onExpandRow: function (index, row, $detail) {
                    oTableInit.InitSubTable(index, row, $detail);
                },
                onCheck: function (row, $element) {
                    $($element).parent().parent().next(".detail-view").find("input[type='checkbox']").prop("checked", true);
                },
                onUncheck: function (row, $element) {
                    $($element).parent().parent().next(".detail-view").find("input[type='checkbox']").prop("checked", false);
                },
                onCheckAll: function () {
                    $("#list").find("input[type='checkbox']").prop("checked", true);
                },
                onUncheckAll: function () {
                    $("#list").find("input[type='checkbox']").prop("checked", false);
                },
                formatNoMatches: function () {
                    return "没有相关的匹配结果";
                },
                formatLoadingMessage: function () {
                    return "请稍等，正在加载中。。。";
                },
                onLoadSuccess: function (data) {
                    $(".detail-icon").trigger("click");
                }
            })
        }
        // 父级表格查询条件
        oTableInit.queryParams = function () {
            var temp = {
                act: 'getDatalist',
                type: '<%=type%>',
            };
            return temp;
        };
        // 加载子表格
        oTableInit.InitSubTable = function (index, row, $detail) {
            var cur_table = $detail.html('<table class="subTable"></table>').find('table');
            $(cur_table).bootstrapTable({
                url: 'mNetSalesApproval.aspx',
                type: 'post',
                clickToSelect: true,
                contentType: "application/x-www-form-urlencoded",
                queryParams: oTableInit.querySubParams(row.group, row.item),
                columns: [{
                    checkbox: true
                }, {
                    field: 'docCode',
                    title: '单据编号'
                }, {
                    field: 'sales',
                    title: '业务员'
                }, {
                    field: 'state',
                    title: '状态'
                }, {
                    field: 'createTime',
                    title: '提交时间'
                }, {
                    field: 'netSalesNum',
                    title: '纯销数量',
                    visible: false
                }, {
                    field: 'flowNum',
                    title: '流向数量',
                    visible: false
                }, {
                    field: 'hospital',
                    title: '医院名称',
                    visible:false
                }, {
                    field: 'product',
                    title: '产品名称',
                    visible: false
                }],
                formatNoMatches: function () {
                    return "没有相关的匹配结果";
                },
                formatLoadingMessage: function () {
                    return "请稍等，正在加载中。。。";
                },
                onLoadSuccess: function (data) {
                    $(".subTable input[name='btSelectAll']").remove();
                },
                onClickRow: function (row, $element) {
                    // 点击单个单据，进入详情页面
                    docCodes = new Array();
                    docCodes = row.docCode;
                    docCode = row.docCode;
                    // 加载单据详细信息
                    $("#detailModal").modal('show');

                    $("#hospital").val(row.hospital);
                    $("#product").val(row.product);
                    $("#sales").html(row.sales);
                    $("#netSalesNum").val(row.netSalesNum);
                    $("#time").html(row.CreateTime);
                    $("#state").html(row.state);
                    $("#title").text("纯销上报单据" + docCode);

                    // 加载审批列表
                    $.post('mNetSalesApproval.aspx', { act: 'getApprovalRecord', docCode: docCode }, function (res) {
                        var approvalList = $.parseJSON(res);
                        $("#approvalRecord").empty();
                        for (i = 0; i < approvalList.length; i++) {
                            $("#approvalRecord").append('<span class="badge" style="border-radius:0">' + approvalList[i].time + '</span>&nbsp;&nbsp;');
                            $("#approvalRecord").append('<span>' + approvalList[i].userName + ':' + approvalList[i].ApprovalResult + '</span>');
                            if (approvalList[i].ApprovalOpinions != null && approvalList[i].ApprovalOpinions != '') {
                                $("#approvalRecord").append('<span>(' + approvalList[i].ApprovalOpinions + ')</span></br>');
                            } else {
                                $("#approvalRecord").append('</br>');
                            }
                        }
                    });

                    // 加载审批流程
                    InitProcessInfo(row.docCode);
                }
            })
        }
        // 子表格查询条件
        oTableInit.querySubParams = function (hospital, product) {
            var temp = {
                act: 'getSubDataList',
                hospital: hospital,
                product: product,
                type: '<%=type%>',
            }
            return temp;
        }
        return oTableInit;
    }

    // 加载列表数据
    function InitProcessInfo(docCode) {
        Loading(true);
        // 加载流程信息
        $.post('mNetSalesApproval.aspx', { act: 'getProcessInfo',docCode:docCode }, function (res) {
            $("#ystep").empty();
            var processInfo = $.parseJSON(res);
            //var divs = "";
            //var divs = '<ul class="nav nav-pills nav-justified step step-square">';
            var divs = '<li class="ztb_over"><a href="#"  class="ztb_con_text">' + $("#sales").html() + '(提交人)</a></li>';
            for (i = 0; i < processInfo.length; i++) {
                var name = processInfo[i].userName;
                divs += '<li><a href="#" class="ztb_con_text">' + name + '</a></li>';
            }
            $("#ystep").append(divs);
        });

        Loading(false);
            //$("#ystep").loadStep({
            //    size: "small",
            //    color: "green",
            //    steps: processArray
            //});
            //$("#ystep1").loadStep({
            //    size: "small",
            //    color: "green",
            //    steps: processArray
            //});
            //判断是否是手机登陆
            //if (!IsPC()) {
            //    $(".nav-justified>li").css("float", "left");
            //}

            //// 加载流程进行到哪一步了（查询所有流程有几步 当前进行到第几步了）
            //$.post('mNetSalesApproval.aspx', { act: 'getProcessStage', docCode: docCode}, function (res) {
            //    var stageInfo = $.parseJSON(res);
            //    var total_record = parseInt(stageInfo.total_record) + 1;
            //    //$("#ystep").setStep(total_record);
            //    //$("#ystep1").setStep(total_record);
            //    for (i = 0; i < total_record; i++) {
            //        $($("#ystep").find("li").get(i)).addClass("ztb_over");
            //    }
            //    //bsStep((total_record));
            //});
    }

    //$("#edit").click(function () {
    //    $("#edit").css("display", "none"); 
    //    $("#approval").css("display", "none");
    //    $("#confirm").css("display", "block");
    //    $("#cancel").css("display", "block");
    //})

    $("#approval").click(function () {
        $("#cancel").trigger("click");
        $('#myModal').modal();
    })

    $("#agree").click(function () {
        Loading(true);
        $.post('mNetSalesApproval.aspx',
            { act: 'beginApproval', netSalesNum: $("#netSalesNum").val(),docCodes: docCodes, ApprovalOpinions: $("#approvalOption").val(), ApprovalResult: "同意审批"},
            function (res) {
                Loading(false);
                var msges = res.split(",");
                var jointMsg = "";
                for (i = 0; i < msges.length; i++) {
                    jointMsg += (msges[i]+"</br>")
                }
                $.messager.alert("警告",jointMsg);
                $("#myModal").modal('hide');
                $('#list').bootstrapTable('refresh')
            }
        );
    })

    $("#decline").click(function () {
        Loading(true);
        $.post('mNetSalesApproval.aspx',
            { act: 'beginApproval', docCodes: docCodes, netSalesNum: $("#netSalesNum").val(), ApprovalOpinions: $("#approvalOption").val(), ApprovalResult: "不同意审批" },
            function (res) {
                Loading(false);
                var msges = res.split(",");
                var jointMsg = "";
                for (i = 0; i < msges.length; i++) {
                    jointMsg += (msges[i] + "</br>")
                }
                $.messager.alert("提示", jointMsg);
                $("#myModal").modal('hide');
                $('#list').bootstrapTable('refresh')
            }
        );
    })

    $("#return").click(function () {
        Loading(true);
        $.post('mNetSalesApproval.aspx', { act: 'returnDocument', docCode: docCode },
            function (res) {
                //alert(res)
                Loading(false);
                $.messager.alert("提示", res);
                $("#detailModal").modal('hide');
                $('#list').bootstrapTable('refresh');
            });
    })

    $("#btn_refresh").click(function () {
        $('#list').bootstrapTable('refresh');
    })

    $("#btn_edit").click(function () {
        docCodes = new Array();
        //$("#list").find("input:checkbox:checked").each(function (i) {
        //    if (i != 0) {   // 去除全选的选择框
        //        console.log($(this).parent(".detail-view"))
        //    }
        //})
        var _flag = "valid";
        var _flag2 = "empty"
        $("#btn_edit").attr("data-toggle", "");
        $("#btn_edit").attr("data-target", "");
        $(".subTable").each(function (j) {
            var subBills = $(".subTable:eq(" + j + ")").find("input[type='checkbox']:checked");

            if (subBills.length != 0) {
                _flag2 = "not empty";
            }
            for (i = 0; i < subBills.length; i++) {
                var docCode = $(subBills[i]).parent().next().html();
                console.log(docCode)
                docCodes.push(docCode);
                var flowNum = subBills[i].flowNum;
                var netSalesNum = subBills[i].netSalesNum;
                if (parseInt(flowNum) > parseInt(netSalesNum)) {
                    _flag = "invalid";
                    break;
                }
            }
        })
        if (_flag == "valid") {
            if (_flag2 == "not empty") {
                $("#btn_edit").attr("data-toggle", "modal");
                $("#btn_edit").attr("data-target", "#myModal");
            } else {
                $.messager.alert("提示", "单据为空，不能进行审批");
                //alert("单据为空，不能进行审批");
            }
        } else (
            $.messager.alert("提示", "存在流向数量小于纯销数量的网点，不能进行审批")
        )    
    })
</script>
</html>