<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mMySubmittedReimburse.aspx.cs" Inherits="mMySubmittedReimburse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link rel="stylesheet" href="Scripts/themes/mobile.css">
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/jquery.easyui.mobile.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/mobileCommon.js"></script>
    <script src="Scripts/weui.min.js"></script>
    <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    <script src="Scripts/weui-upload.js"></script>
    <style>
        html,
        body {
            height: auto
        }
    </style>
</head>

<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
                background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;"
        class="easyui-dialog" border="false" noheader="true" closed="true" modal="true">
    </div>
    <div id="p1" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span class="m-title">我提交的</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-search',size:'small',iconAlign:'right',plain:true, outline:true" 
                        onclick="$('#searchTime').dialog('open').dialog('center')">时间查询</a>
                </div>
                <div class="m-right">
                    <a href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true"
                        onclick="$.mobile.go('#pDownload');" style="width:100px">下载报销汇总表</a>
                </div>
            </div>
        </header>
        <div id="searchTime" class="easyui-dialog" style="padding:20px 6px;width:40%;" data-options="inline:true,modal:true,closed:true,title:'月份查询'">
            <div style="margin-bottom:10px">
                <div class="weui-cell">
                    <div class="weui-cell__bd">
                        <input id="chooseDate" class="weui-input" type="month" value="">
                    </div>
                </div>
            </div>
            <div class="dialog-button">
                <a href="javascript:void(0)" class="easyui-linkbutton" style="width:100%;height:35px" onclick="$('#searchTime').dialog('close');searchByTime()">查询</a>
            </div>
        </div>
        <div id="tt" class="easyui-tabs" data-options="tabHeight:40,fit:true,tabPosition:'bottom',border:false,pill:true,narrow:true,justified:true">
            <div id="preview1">
                <div class="panel-header tt-inner">
                    单据列表
                </div>
                <div class="weui-search-bar" id="searchBar">
                    <form class="weui-search-bar__form" onsubmit="return false;">
                        <div class="weui-search-bar__box">
                            <i class="weui-icon-search"></i>
                            <input type="search" class="weui-search-bar__input" id="search_input" placeholder="搜索费用明细，备注，人名" />
                            <a href="javascript:" class="weui-icon-clear" id="search_clear"></a>
                        </div>
                        <label for="search_input" class="weui-search-bar__label" id="search_text">
                            <i class="weui-icon-search"></i>
                            <span>搜索费用明细，备注，人名</span>
                        </label>
                    </form>
                    <a href="javascript:" class="weui-search-bar__cancel-btn" id="search_cancel">取消</a>
                </div>
                <div id="preview">

                </div>
                <div class="weui-loadmore">
                    <i class="weui-loading"></i>
                    <span class="weui-loadmore__tips">正在加载</span>
                </div>  
            </div>
            <div id="preview2">
                <div class="panel-header tt-inner">
                    单据统计
                </div>
                <div class="weui-panel weui-panel_access">
                    <div class="weui-panel__hd">提供以下两种统计方式</div>
                    <div class="weui-panel__bd">
                        <a href="javascript:void(0)" class="weui-media-box weui-media-box_appmsg" onclick="showStatistics('detail')">
                            <div class="weui-media-box__bd">
                                <h4 class="weui-media-box__title">按费用明细统计</h4>
                                <p class="weui-media-box__desc">展示已审批的费用明细，笔数，金额和实报金额</p>
                            </div>
                        </a>
                        <a href="javascript:void(0)" class="weui-media-box weui-media-box_appmsg" onclick="showStatistics('department')">
                            <div class="weui-media-box__bd">
                                <h4 class="weui-media-box__title">按费用归属部门统计</h4>
                                <p class="weui-media-box__desc">展示已审批的费用归属部门，笔数，金额和实报金额</p>
                            </div>
                        </a>
                    </div>
                </div>
            </div>
        </div> 
    </div>
    <div id="p2" class="easyui-navpanel" style="position:relative;padding:20px" data-options="footer:'#footer'">
        <header>
            <div class="m-toolbar">
                <span class="m-title">报销单据明细</span><span style="color:red" id="isOverBudget"></span>
            </div>
            <div class="m-left">
                <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true"
                    style="width:50px" onclick="$.mobile.back();$('#searchBar').show()">返回</a>
            </div>
        </header>
        <div>
            <form id="fm" class="easyui-form" method="post" data-options="novalidate:true">
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="单据编号:" prompt="单据编号" style="width:100%" name="code"
                        data-options="editable:false">
                </div>
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="填报人:" prompt="填报人" style="width:100%" name="name"
                        data-options="editable:false">
                </div>
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="产生时间:" prompt="产生时间" style="width:100%" name="apply_time"
                        data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="是否历史核销（4月以前）:" prompt="是否历史核销（4月以前）" style="width: 100%"
                        name="isPrepaid" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="产品:" prompt="产品" style="width: 100%" name="product"
                        data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px" id="project">
                    <input class="easyui-textbox" label="项目编号:" prompt="项目编号" style="width: 100%" name="project"
                        data-options="editable:false">
                </div>
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="所属部门:" prompt="所属部门" style="width:100%" name="department"
                        data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="费用归属部门:" prompt="费用归属部门" style="width:100%"
                        name="fee_department" data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="费用归属公司:" prompt="费用归属公司" style="width:100%"
                        name="fee_company" data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="费用明细:" prompt="费用明细" style="width:100%" name="fee_detail"
                        data-options="editable:false">
                </div>
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="网点:" prompt="网点" style="width:100%" name="branch"
                        data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="金额:" prompt="金额" style="width:100%" name="fee_amount"
                        data-options="editable:false">
                </div>
                <%--<div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="实报金额:" prompt="实报金额" style="width:100%" name="actual_fee_amount" data-options="editable:false">
                    </div>--%>
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="审批状态:" prompt="审批状态" style="width:100%" name="status"
                        data-options="editable:false">
                </div>
                <%--<div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="审批人:" prompt="审批人" style="width:100%" name="approver" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="审批时间:" prompt="审批时间" style="width:100%" name="approval_time" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="审批状态:" prompt="审批状态" style="width:100%" name="status" data-options="editable:false">
                    </div>--%>
                <%--<div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="审批结果:" prompt="审批结果" style="width:100%" name="ApprovalResult" data-options="editable:false">
                    </div>
                    <div style="margin-bottom:10px">
                        <input class="easyui-textbox" label="审批意见:" prompt="审批意见" style="width:100%;height:50px" name="ApprovalOpinions" data-options="editable:false,multiline:true">
                    </div>--%>
                <div style="margin-bottom:10px">
                    <input class="easyui-textbox" label="备注:" prompt="备注" style="width:100%;height:100px" name="remark"
                        data-options="editable:false,multiline:true">
                </div>

                <div style="margin-bottom:10px">
                    <input id="processInfo" class="easyui-textbox" label="流程信息:" prompt="流程信息"
                        style="width:100%;height:100px" name="processRecord"
                        data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom:10px">
                    <div class="weui-gallery" id="gallery">
                        <span class="weui-gallery__img" id="galleryImg"></span>
                        <div class="weui-gallery__opr">
                            <%--<a href="javascript:" class="weui-gallery__del">
                                    <i class="weui-icon-delete weui-icon_gallery-delete"></i>
                                </a>--%>
                        </div>
                    </div>
                    <div class="weui-cells weui-cells_form">
                        <div class="weui-cell">
                            <div class="weui-cell__bd">
                                <div class="weui-uploader">
                                    <div class="weui-uploader__hd">
                                        <p class="weui-uploader__title" style="font-size:10px">图片:</p>
                                    </div>
                                    <div class="weui-uploader__bd">
                                        <ul class="weui-uploader__files" id="uploaderFiles">
                                            <%--                                                <li class="weui-uploader__file" style="background-image:url(./images/pic_160.png)"></li>--%>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="margin-bottom: 10px" id="cancelDiv">
                    <a href="javascript:void(0)" id="cancel" class="easyui-linkbutton" style="width: 100%"
                        onclick="">撤销</a>
                </div>
            </form>
        </div>
    </div>
    <div id="p3" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <div class="m-title">统计页面</div>
                <div class="m-left">
                    <a href="#" class="easyui-linkbutton m-back" data-options="plain:true,outline:true,back:true">返回</a>
                </div>
            </div>
        </header>
        <table id="dg"></table>
    </div>
    <div id="dlg" class="easyui-dialog" title="单据详情" style="width: 80%; height: 500px; padding: 10px"
                data-options="closed:true,
				buttons: [{
					text:'关闭',                   
					handler:function(){
					 $('#dlg').dialog('close');
					}
				}]
			">
        
        <div id="preview3">

        </div>
    </div>
    <div style="padding:20px 40px" id="footer">
        <a id="sumbitter" href="#" class="easyui-linkbutton c1" data-options="iconAlign:'top',size:'small'">

        </a>
        <span>-></span>
        <a id="approver" href="#" class="easyui-linkbutton" data-options="iconAlign:'top',size:'small'">

        </a>
    </div>
    <div id="pDownload" class="easyui-navpanel" style="position: relative; padding: 20px">
        <header>
            <div class="m-toolbar">
                <span class="m-title">下载报销汇总表</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true"
                        onclick="$.mobile.back()">返回</a>
                </div>
            </div>
        </header>
        <div>
            <select id="company" class="easyui-combobox" style="width:100%;"
                data-options="editable:false,panelHeight:'auto',label: '请选择公司',labelPosition: 'top'">
                <option value="江西业力科技集团有限公司">江西业力科技集团有限公司</option>
                <option value="江西东森科技发展有限公司">江西东森科技发展有限公司</option>
                <option value="江西业力医疗器械有限公司">江西业力医疗器械有限公司</option>
                <option value="南昌市中申医疗器械有限公司">南昌市中申医疗器械有限公司</option>
                <option value="南昌老康科技有限公司">南昌老康科技有限公司</option>
                <option value="天津吉诺泰普生物科技有限公司">天津吉诺泰普生物科技有限公司</option>
                <option value="南昌业力医学检验实验室有限公司">南昌业力医学检验实验室有限公司</option>
                <option value="九江傲沐科技发展有限公司">九江傲沐科技发展有限公司</option>
                <option value="南昌业力生物科技有限公司">南昌业力生物科技有限公司</option>
                <option value="上海会帆企业管理咨询中心">上海会帆企业管理咨询中心</option>
                <option value="上海恩焯企业管理咨询中心">上海恩焯企业管理咨询中心</option>
            </select>
            <!-- <input class="easyui-datebox" label="请选择月份:" labelPosition="top" style="width:100%;"> -->
            <div style="margin-bottom:10px;margin-top:20px">
                <p>请选择报销月份：</p>
                <div class="weui-cell">
                    <div class="weui-cell__bd">
                        <input id="chooseDate1" class="weui-input" type="month" value="">
                    </div>
                </div>
            </div>

            <a href="javascript:void(0)" class="easyui-linkbutton c1" plain="true" outline="true" style="width:100%;"
                data-options="disabled:false" onclick="DownloadExcel()">下载</a><br><br><br>
            <div id="download-list"></div>
            <!-- <a href="Template/营销中心费用报销新套表-.xls" style="width:100%;">下载营销中心费用报销封面模板</a><br><br><br>
                <a href="Template/职能线费用报销套表-.xls" style="width:100%;">下载职能线费用报销封面模板</a> -->
        </div>
    </div>
    <script src="Scripts/jquery-weui/jquery-weui.js"></script>
    <script type="text/javascript">
        var Url = "mMySubmittedReimburse.aspx";
        var loading = false; var totalData;
        var pageSize = 5; var pageStart = 0; var level = 0; var year; var month;
        $(document).ready(function () {
            weui.searchBar('#searchBar');
            var code = GetQueryString('docCode');
            var date = new Date;
            year = date.getFullYear();
            month = date.getMonth() + 1;
            month = (month < 10 ? "0" + month : month);
            if (code == null) {
                DgLoad('','','',year,month);
            }
            else {
                ShowDocumentByCode(code);
            }
            InitDate();
        });

        $(document).keydown(function (event) {
            if (event.keyCode == 13) {
                $("#preview").empty();
                loading = false;
                pageSize = 5; pageStart = 0;
                DgLoad("", "", $("#search_input").val(),year,month);
            }
        });

        function searchByTime() {
            var date = $("#chooseDate").val();
            year = date.split("-")[0];
            month = date.split("-")[1];
            $("#preview").empty();
            pageSize = 5; pageStart = 0;
            DgLoad("", "", $("#search_input").val(), year, month)
        }

        //$("#search_input").bind("input propertychange", function (event) {
        //    $("#preview").empty();
        //    loading = false;
        //    pageSize = 5; pageStart = 0;
        //    DgLoad("", "", $("#search_input").val());
        //});

        $("#preview1").infinite().on("infinite", function () {
            if (loading)
                return;
            loading = true;
            $('.weui-loadmore').show();
            setTimeout(function () {
                pagination(totalData, pageSize, pageStart);
                if ((totalData.total - pageStart) <= pageSize) {
                    $(".weui-loadmore").hide();
                    var html = "<div style=\"text-align:center\" class=\"weui-cells__title\" >已无更多数据</div>";
                    $("#preview").append(html);
                    loading = true;
                } else {
                    pageStart += pageSize;
                    loading = false;
                }
                //loadlist();

            }, 500);   //模拟延迟
        });

        function ShowDocumentByCode(code) {
            var res = AjaxSync(Url, { act: 'getDocument', docCode: code });
            try {
                var data = $.parseJSON(res);
                if (data[0].project == "请选择" || data[0].project == null) {
                    $("#project").css("display", "none");
                }
                if (data[0].isPrepaid == '0') {
                    data[0].isPrepaid = '否'
                } else {
                    data[0].isPrepaid = '是'
                }
                showProcessAndAttachment(data[0]);
                $("#searchBar").hide()
                $.mobile.go('#p2');
                data[0].apply_time = data[0].apply_time.replace("T", " ").substring(0, 16)
                $('#fm').form('load', data[0]);
                if (data[0].isOverBudget == 1) {
                    $('#isOverBudget').html("(超预算)");
                }

                level = data[0].Level;

                if (data[0].status != '审批中' && data[0].status != '已拒绝') {
                    $("#cancelDiv").css("display", "none");
                } else {
                    $("#cancelDiv").css("display", "block");
                    $("#cancel").attr("onclick", "cancel('" + data[0].code + "')")
                }
            } catch (error) {
                $.messager.alert('提示', res);
            }
        }

        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }

        function showProcessAndAttachment(data) {
            Loading(true);
            $.ajax({
                url: 'mMySubmittedReimburse.aspx',
                data: { act: 'getProcessInfoAndAttachment', docCode: data.id },
                dataType: 'json',
                type: 'post',
                success: function (msg) {
                    var data = JSON.parse(msg.data);
                    var processData = JSON.parse(msg.count);
                    var attachment = JSON.parse(msg.attachment);
                    $("#uploaderFiles").empty();
                    if (attachment != null && attachment.length > 0) {
                        var imageHtml = "";
                        for (i = 0; i < attachment.length; i++) {
                            imageHtml += "<li class=\"weui-uploader__file\" style=\"background-image:url(" + attachment[i].ImageUrl + ")\"></li>";
                        }
                        $("#uploaderFiles").append(imageHtml);
                    }

                    var html = "";
                    $.each(processData, function (i, v) {
                        html += v.Name + " " + v.Time + " " + v.ApprovalResult + " " + v.ApprovalOpinions + "\r\n";
                    })
                    $("#processInfo").textbox("setValue", html);

                    $("#footer a:not(:first)").remove();
                    $("#footer").children("span:not(:first)").remove();
                    $("#sumbitter").linkbutton({ text: data[0].name });
                    var approvers = data[0].approver;
                    var leaders = approvers.split(",");
                    $.each(leaders, function (i, v) {
                        if (v != '') {
                            var html = "";
                            if (level - 1 > i) {
                                html =
                                    '<a href="#" class="easyui-linkbutton c1" data-options="iconAlign:\'top\',size:\'small\'">';
                            } else {
                                html =
                                    '<a href="#" class="easyui-linkbutton c2" data-options="iconAlign:\'top\',size:\'small\'">';
                            }
                            html += v;
                            //if (num - 1 > i) {
                            //    html += '<span class="m-badge">√</span>'
                            //} else {
                            //    html += '<span class="m-badge">×</span>'
                            //}
                            html += "</a >";
                            html += "<span>-></span>";
                            $("#footer").children("span:last").after(html);
                        } else {
                            i--;
                        }
                        $.parser.parse($('#footer'))
                        // initDatebox();
                    })
                    $("#footer").children("span:last").remove();
                    Loading(false)
                }
            })
        }

        function ShowDetail(data) {
            //加载流程信息和图片
            if (data.project == "请选择" || data.project == null) {
                $("#project").css("display", "none");
            }
            if (data.isPrepaid == '0') {
                data.isPrepaid = '否'
            } else {
                data.isPrepaid = '是'
            }
            showProcessAndAttachment(data)
            $("#searchBar").hide()
            $.mobile.go('#p2');
            $('#fm').form('load', data);
            level = data.Level;

            if (data.isOverBudget == 1) {
                $('#isOverBudget').html("(超预算)");
            }

            if (data.status != '审批中' && data.status != '已拒绝') {
                $("#cancelDiv").css("display", "none");
            } else {
                $("#cancelDiv").css("display", "block");
                $("#cancel").attr("onclick", "cancel('" + data.code + "')")
            }
        }

        function cancel(code) {
            $.messager.confirm('提示', '确实要撤销此单据吗？', function (r) {
                if (r) {
                    // 撤销
                    $.ajax({
                        url: "mMySubmittedReimburse.aspx",
                        data: { act: "cancel", code: code },
                        dataType: 'json',
                        type: 'post',
                        success: function (data) {
                            if (data.ErrCode == "0") {
                                location.href = "mToBeSubmitted.aspx"
                            } else {
                                $.messager.alert('提示', '撤销失败');
                            }
                        },
                        error: function () {
                            $.messager.alert('提示', '撤销失败');
                        }
                    })
                }
            });
        }

        function DgLoad(sort, order, keyword, year, month) {
            if (sort == undefined || sort == "") {
                sort = 'code'; order = 'desc';
            }
            var data = {
                act: 'getInfos',
                sort: sort, order: order, keyword: keyword,
                year: year, month: month
            };
            Loading(true);
            $.post(Url, data, function (res) {
                if (res != 'F' && res != "") {
                    totalData = $.parseJSON(res);
                    pagination(totalData, pageSize, pageStart);
                    pageStart += pageSize;
                } else {
                    $.messager.alert('提示', '暂无数据', 'info');
                }
                $(".weui-loadmore").hide();
                Loading(false);
            });
        }

        function pagination(datasource, pageSize, pageStart) {
            var tempData = datasource.rows;
            var count = (datasource.total - pageStart) > pageSize ? pageSize : (datasource.total - pageStart);
            for (i = pageStart; i < pageStart + count; i++) {
                var html = "";
                html += '<div class="weui-form-preview">';
                html += ' <div class="weui-form-preview__hd">';
                html += '<label class="weui-form-preview__label">金额</label>';
                html += '<em class="weui-form-preview__value">¥' + tempData[i].fee_amount + '</em>';
                html += '</div>';
                html += '<div class="weui-form-preview__bd">';
                html += '<p>';
                html += '<label class="weui-form-preview__label">编号</label>';
                html += '<span class="weui-form-preview__value">' + tempData[i].code + '</span>';
                html += '</p>';
                html += '<p>';
                html += '<label class="weui-form-preview__label">审批人</label>';
                html += '<span class="weui-form-preview__value">' + tempData[i].approver + '</span>';
                html += '</p>';
                html += '<p>';
                html += '<label class="weui-form-preview__label">费用明细</label>';
                html += '<span class="weui-form-preview__value">' + tempData[i].fee_detail + '</span>';
                html += '</p>';
                html += '<p>';
                html += '<label class="weui-form-preview__label">备注</label>';
                html += '<span class="weui-form-preview__value">' + (tempData[i].remark == "" ? "无" : tempData[i].remark) + '</span>';
                html += '</p>';
                html += '<p>';
                html += '<label class="weui-form-preview__label">状态</label>';
                //html += '<span class="weui-form-preview__value">' + '事业部' + tempData[i].status + "," + "财务部" + (tempData[i].ApprovalResult == "" ? "未审批" : tempData[i].ApprovalResult) + "," + (tempData[i].actual_fee_amount == "" ? "未付款" : "已付款") + '</span>';
                html += '<span class="weui-form-preview__value">';
                if (tempData[i].status == "审批中") {
                    html += '<span class="weui-badge" style= "background-color:slategray" >事业部审批中</span>';
                } else if (tempData[i].status == "已通过" || tempData[i].status == "已审批") {
                    html += '<span class="weui-badge" style= "background-color:limegreen" >事业部已通过</span>';
                } else {
                    html += '<span class="weui-badge" style= "background-color:crimson" >事业部已拒绝</span>';
                }

                if (tempData[i].account_result == "拒绝" && tempData[i].account_approver != "") {
                    html += '<span class="weui-badge" style= "background-color:crimson">财务已拒绝</span>';
                } else if (tempData[i].account_result == "同意" && tempData[i].account_approver != "") {
                    html += '<span class="weui-badge" style= "background-color:limegreen">财务已通过</span>';
                } else {
                    html += '<span class="weui-badge" style= "background-color:slategray">财务未审批</span>';
                }

                if (tempData[i].pay_amount == 0) {
                    html += '<span class="weui-badge" style= "background-color:slategray">未付款</span>';
                } else {
                    html += '<span class="weui-badge" style= "background-color:limegreen">已付款</span>';
                }

                html += '</span >';
                html += '</p>';
                html += '</div>';
                html += '<div class="weui-form-preview__ft">';
                html += "<a class='weui-form-preview__btn weui-form-preview__btn_primary' href='javascript:' onClick='ShowDetail(" + JSON.stringify(tempData[i]) + ")'>查看详情</a>";
                html += '</div>';
                html += '</div>';

                $("#preview").append(html);
            }
        }

        function InitDate() {
            var now = new Date();
            var month = (Array(2).join(0) + (now.getMonth() + 1)).slice(-2)//数字自动补零
            var date = now.getFullYear() + '-' + month;
            $("#chooseDate").val(date);
        }

        function DownloadExcel() {
            var data = {
                act: 'DownloadExcel',
                date: $("#chooseDate1").val(),
                company:$("#company").combobox('getValue')
            }
            Loading(true);
            $.post(Url, data, function (res) {
                Loading(false);
                $("#download-list").empty();
                response = $.parseJSON(res);
                if (response.ErrCode == 0) {
                    if (response.FileCount == 1) {
                        // window.location.href = 'ExportExcelHelper.aspx?fileName=' + response.FileName1 + '.xls&fileCode=' + response.FileCode1;
                        window.location.href = 'tempExportFile/' + response.FileCode1 + '.xls';
                    }
                    else {
                        //<a href="Template/营销中心费用报销新套表-.xls" style="width:100%;">下载营销中心费用报销封面模板</a><br><br><br>
                        var a = '<a href="tempExportFile/' + response.FileCode1
                            + '.xls" style="width:100%;">' + response.FileName1 + '-1</a><br><br><br>';
                        $("#download-list").append(a);
                        a = '<a href="tempExportFile/' + response.FileCode2
                            + '.xls" style="width:100%;">' + response.FileName2 + '-2</a><br><br><br>';
                        $("#download-list").append(a);
                    }

                } else {
                    $.messager.alert('提示', res, 'info');
                }
            }).error(function (xhr, status, info) {
                console.log(info)

            });
        }

        function initStatisticTable(type) {
            var column;
            if (type == 'detail') {
                column = [[
                    {
                        field: "fee_detail",
                        title: '费用明细',
                        align: "center",
                        width: 120,
                    },
                    {
                        field: 'sum',
                        title: '笔数',
                        align: "center",
                        width: 80,
                    }, 
                    {
                        field: 'prepaidAmount',
                        title: '公司垫付核销金额',
                        align: "center",
                        width: 120,
                        formatter(value,row,index) {
                            if (value === null || value === '')
                                return 0;
                            else
                                return value
                        }
                    },
                    {
                        field: 'feeAmount',
                        title: '报销金额',
                        align: "center",
                        width: 120,
                        formatter(value, row, index) {
                            if (value === null || value === '')
                                return 0;
                            else
                                return value
                        }
                    },
                    {
                        field: 'actualFeeAmount',
                        title: '财务审批金额',
                        align: "center",
                        width: 120,
                        formatter(value, row, index) {
                            if (value === null || value === '')
                                return 0;
                            else
                                return value
                        }
                    },
                    {
                        field: 'loanAmount',
                        title: '还备用金',
                        align: "center",
                        width: 120,
                        formatter(value, row, index) {
                            if (value === null || value === '')
                                return 0;
                            else
                                return value
                        }
                    },
                    {
                        field: 'payAmount',
                        title: '出纳付款金额',
                        align: "center",
                        width: 120,
                        formatter(value, row, index) {
                            if (value === null || value === '')
                                return 0;
                            else
                                return value
                        }
                    }
                ]]
            } else if (type == 'department') {
                column = [[
                    {
                        field: "fee_department",
                        title: '费用归属部门',
                        align: "center",
                        width: 120,
                    },
                    {
                        field: 'sum',
                        title: '笔数',
                        align: "center",
                        width: 80,
                    },
                    {
                        field: 'prepaidAmount',
                        title: '公司垫付核销金额',
                        align: "center",
                        width: 120,
                        formatter(value, row, index) {
                            if (value === null || value === '')
                                return 0;
                            else
                                return value
                        }
                    },
                    {
                        field: 'feeAmount',
                        title: '报销金额',
                        align: "center",
                        width: 120,
                        formatter(value, row, index) {
                            if (value === null || value === '')
                                return 0;
                            else
                                return value
                        }
                    },
                    {
                        field: 'actualFeeAmount',
                        title: '实报金额',
                        align: "center",
                        width: 120,
                        formatter(value, row, index) {
                            if (value === null || value === '')
                                return 0;
                            else
                                return value
                        }
                    },
                    {
                        field: 'loanAmount',
                        title: '还备用金',
                        align: "center",
                        width: 120,
                        formatter(value, row, index) {
                            if (value === null || value === '')
                                return 0;
                            else
                                return value
                        }
                    },
                    {
                        field: 'payAmount',
                        title: '出纳付款金额',
                        align: "center",
                        width: 120,
                        formatter(value, row, index) {
                            if (value === null || value === '')
                                return 0;
                            else
                                return value
                        }
                    }
                ]]
            }

            $('#dg').datagrid({
                singleSelect: true,
                fit: true,
                striped: true,
                rownumbers: true,
                nowrap: false,
                collapsible: false,
                fitColumns: false,
                columns: column,
                onSelect: function (index, data) {
                    initDialog(type, data);
                }
            })
        }

        function initDialog(type, data) {
            $('#dlg').dialog('open');
            $('#preview3').empty();
            if (type == "detail") {
                type = "fee_detail";
            } else if (type == "department") {
                type = "fee_department";
            }
            var html = "";
            for (var i = 0; i < detail.length; i++) {
                if (detail[i][type] == data[type]) {
                    var html = "";
                    html += '<div class="weui-form-preview">';
                    html += ' <div class="weui-form-preview__hd">';
                    html += '<label class="weui-form-preview__label">金额</label>';
                    html += '<em class="weui-form-preview__value">¥' + detail[i].fee_amount + '</em>';
                    html += '</div>';
                    html += '<p>';
                    html += '<label class="weui-form-preview__label">申请人</label>';
                    html += '<span class="weui-form-preview__value">' + detail[i].name + '</span>';
                    html += '</p>';
                    html += '<p>';
                    html += '<label class="weui-form-preview__label">费用归属部门</label>';
                    html += '<span class="weui-form-preview__value">' + detail[i].fee_department + '</span>';
                    html += '</p>';
                    html += '<p>';
                    html += '<label class="weui-form-preview__label">费用明细</label>';
                    html += '<span class="weui-form-preview__value">' + detail[i].fee_detail + '</span>';
                    html += '</p>';
                    html += '<p>';
                    html += '<label class="weui-form-preview__label">备注</label>';
                    html += '<span class="weui-form-preview__value">' + (detail[i].remark == "" ? "无" : detail[i].remark) + '</span>';
                    html += '</p>';
                    $("#preview3").append(html);

                }
            }
            //$.parser.parse($("#preview3").parent());
        }

        function showStatistics(type) {
            $.mobile.go('#p3')
            initStatisticTable(type);
            Loading(true);
            $.ajax({
                url: 'mMySubmittedReimburse.aspx',
                data: {
                    act: 'getStatistics',
                    year: year,
                    month: month,
                    type: type
                },
                dataType: 'json',
                type: 'post',
                success: function (data) {
                    statistics = JSON.parse(data.statistics).rows;
                    $('#dg').datagrid("loadData", statistics);
                    detail = JSON.parse(data.detail).rows;
                    Loading(false);
                }
            })
        }
    </script>
</body>

</html>