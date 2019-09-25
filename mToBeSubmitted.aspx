<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mToBeSubmitted.aspx.cs" Inherits="mToBeSubmitted" %>

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
    <script src="Scripts/jquery-weui/jquery-weui.js"></script>
</head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;"
        class="easyui-dialog"
        border="false" noheader="true" closed="true" modal="true">
    </div>
    <div id="p1" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span class="m-title">待我提交</span>
            </div>
        </header>
        <div class="weui-search-bar" id="searchBar">
            <form class="weui-search-bar__form" onsubmit="return false;">
                <div class="weui-search-bar__box">
                    <i class="weui-icon-search"></i>
                    <input type="search" class="weui-search-bar__input" id="search_input" placeholder="搜索费用明细，备注" />
                    <a href="javascript:" class="weui-icon-clear" id="search_clear"></a>
                </div>
                <label for="search_input" class="weui-search-bar__label" id="search_text">
                    <i class="weui-icon-search"></i>
                    <span>搜索费用明细，备注</span>
                </label>
            </form>
            <a href="javascript:" class="weui-search-bar__cancel-btn" id="search_cancel">取消</a>
        </div>
        <div id="preview">
            <!-- 加上preview-->
        </div>
        <div class="weui-loadmore">
            <i class="weui-loading"></i>
            <span class="weui-loadmore__tips">正在加载</span>
        </div>
    </div>
    <div id="p2" class="easyui-navpanel" style="position: relative; padding: 20px" data-options="footer:'#footer'">
        <header>
            <div class="m-toolbar">
                <span class="m-title">报销单据明细</span><span style="color:red" id="isOverBudget"></span>
            </div>
            <div class="m-left">
                <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" style="width: 50px" onclick="$.mobile.back();$('#searchBar').show()">返回</a>
            </div>
        </header>
        <div>
            <form id="fm" class="easyui-form" method="post" data-options="novalidate:true">
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="编号:" prompt="编号" style="width: 100%" name="code" data-options="editable:false">
                    <input type="hidden" value="" id="docId" >
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="填报人:" prompt="填报人" style="width: 100%" name="name" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="产生时间:" prompt="填报时间" style="width: 100%" name="apply_time" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="产品:" prompt="产品" style="width: 100%" name="product" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="所属部门:" prompt="所属部门" style="width: 100%" name="department" data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="费用归属部门:" prompt="费用归属部门" style="width: 100%" name="fee_department" data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="费用明细:" prompt="费用明细" style="width: 100%" name="fee_detail" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="网点:" prompt="网点" style="width: 100%" name="branch" data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="金额:" prompt="金额" style="width: 100%" name="fee_amount" data-options="editable:false">
                </div>
               <%-- <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="实报金额:" prompt="实报金额" style="width: 100%" name="actual_fee_amount" data-options="editable:false">
                </div>--%>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="审批状态:" prompt="审批状态" style="width: 100%" name="status" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="备注:" prompt="备注" style="width: 100%;height:100px" name="remark" data-options="editable:false,multiline:true">
                </div>
                
                <div style="margin-bottom:10px">
                    <input id="processInfo" class="easyui-textbox" label="流程信息:" prompt="流程信息" style="width:100%;height:100px" name="processRecord" data-options="editable:false,multiline:true">
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
                <div style="margin-bottom: 10px">
                    <a href="javascript:void(0)" id="reEdit" class="easyui-linkbutton" style="width: 100%" onclick="">重新编辑</a>
                    <a href="javascript:void(0)" id="changeOverBudget" class="easyui-linkbutton" style="width: 100%" onclick="">预算外提交</a>
                </div>
            </form>
        </div>
    </div>
</body>
<script>
    var loading = false; var totalData;
    var pageSize = 5; var pageStart = 0;var level = 0;

    var Url = "mToBeSubmitted.aspx";
    $(document).ready(function () {
        weui.searchBar('#searchBar');
        var code = GetQueryString('docCode');
        if (code == null) {
            DgLoad();
        }
        else {
            ShowDocumentByCode(code);
        }
    });

    $(document).keydown(function(event){
        if(event.keyCode == 13){
            $("#preview").empty();
            loading = false;
            pageSize = 5; pageStart = 0;
            DgLoad("", "", $("#search_input").val());
        }
    });

    //$("#search_input").bind("input propertychange", function (event) {
    //    $("#preview").empty();
    //    loading = false;
    //    pageSize = 5; pageStart = 0;
    //    DgLoad("", "", $("#search_input").val());
    //});
    $("#p1").infinite().on("infinite", function () {
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

    function reEdit(code) {
        $.messager.confirm('提示','确实要重新编辑此单据吗？',function(r){
            if (r){
		        location.href="mFinanceReimburse.aspx?docCode="+code
            }
        });
    }

    function changeOverBudget(code) {
        $.messager.confirm('提示','确实要修改为超预算单据吗？',function(r){
            if (r){
		        location.href="mFinanceReimburse.aspx?docCode="+code+"&isOverBudget=1"
            }
        });
    }

    function ShowDocumentByCode(code) {
            
        var res = AjaxSync(Url, { act: 'getDocument', docCode: code });
        try {
            var data = $.parseJSON(res);
            if (data == "当前用户无审批权限") {
                $.messager.alert('提示', data, '确定',function () {
                    window.location.href = "http://yelioa.top/mMobileReimbursement.aspx";
                });
            } else {
                showProcessAndAttachment(data[0]);
                $('#searchBar').hide();
                $.mobile.go('#p2');
                $('#fm').form('load', data[0]);
                $("#docId").val(data[0].id);
                level = data[0].Level;
                $("#reEdit").attr("onclick", "reEdit(" + data[0].code + ")");
                $("#changeOverBudget").attr("onclick", "changeOverBudget(" + data[0].code + ")");

                if (data[0].isOverBudget == 1) {
                    $('#isOverBudget').html("(超预算)");
                }
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
    function ShowDetail(data) {
        showProcessAndAttachment(data);
        $.mobile.go('#p2');
        $('#searchBar').hide();
        $('#fm').form('load', data);
        $("#docId").val(data.id);
        level = data.Level;
        $("#reEdit").attr("onclick", "reEdit(" + data.code + ")");
        $("#changeOverBudget").attr("onclick", "changeOverBudget(" + data.code + ")");
        if (data.isOverBudget == 1) {
            $('#isOverBudget').html("(超预算)");
        }
    }

    function showProcessAndAttachment(data) {
        Loading(true);
        $.ajax({
            url: 'mMySubmittedReimburse.aspx',
            data: { act: 'getProcessInfoAndAttachment', docCode: data.id },
            dataType: 'json',
            type: 'post',
            success: function (msg) {
                var data1 = JSON.parse(msg.data);
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
                    if (i == 0 || v.ApprovalResult == "撤消") {
                        html += data1[0].name + " " + v.Time.substr(0, 10) + v.Time.substr(v.Time.length - 9, 9) + " " + v.ApprovalResult + " " + v.ApprovalOpinions + "\r\n";
                    } else{
                        html += data1[0].approver.split(",")[i - 1] + " " + v.Time.substr(0, 10) + v.Time.substr(v.Time.length - 9, 9) + " " + v.ApprovalResult + " " + v.ApprovalOpinions + "\r\n";
                    }
                })
                $("#processInfo").textbox("setValue", html);

                $("#footer a:not(:first)").remove();
                $("#footer").children("span:not(:first)").remove();
                $("#sumbitter").linkbutton({ text: data1[0].name });
                var approvers = data1[0].approver;
                var leaders = approvers.split(",");
                $.each(leaders, function (i, v) {
                    if (v != '') {
                        var html = "";
                        if (level - 1 > i) {
                            html = '<a href="#" class="easyui-linkbutton c1" data-options="iconAlign:\'top\',size:\'small\'">';
                        } else {
                            html = '<a href="#" class="easyui-linkbutton c2" data-options="iconAlign:\'top\',size:\'small\'">';
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

                    }
                    $.parser.parse()
                    // initDatebox();
                })
                $("#footer").children("span:last").remove();
                Loading(false)
            }
        })
    }

    function DgLoad(sort, order, keyword) {
        if (sort == undefined || sort == "") {
            sort = 'code'; order = 'desc';
        }
        var data = {
            act: 'getList',
            sort: sort, order: order, keyword: keyword
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

    function pagination(tempData, pageSize, pageStart) {
        var count = (tempData.total - pageStart) > pageSize ? pageSize : (tempData.length - pageStart);
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
            } else if (tempData[i].status == "草稿") {
                html += '<span class="weui-badge" style= "background-color:slategray" >已撤销</span>';
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

            if (tempData[i].actual_fee_amount == "") {
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
</script>
</html>
