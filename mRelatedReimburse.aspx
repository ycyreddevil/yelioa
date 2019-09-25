<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mRelatedReimburse.aspx.cs" Inherits="mRelatedReimburse" %>

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
        .weui-uploader__file{
            height: 79px !important;
            width: 79px !important
        }
    </style>
</head>

<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;"
        class="easyui-dialog"
        border="false" noheader="true" closed="true" modal="true">
    </div>
    <div id="p1" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span class="m-title">抄送我的</span>
            </div>
        </header>
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
                <span class="m-title">报销单据明细</span>
            </div>
            <div class="m-left">
                <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" style="width: 50px" onclick="$.mobile.back();$('#searchBar').show()">返回</a>
            </div>
        </header>
        <div>
            <form id="fm" class="easyui-form" method="post" data-options="novalidate:true">
               <%-- <div style="margin-bottom: 10px">
                    <input id="docId" class="easyui-textbox" label="编号:" prompt="编号" style="width: 100%" name="id" data-options="editable:false">
                </div>--%>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="填报人:" prompt="填报人" style="width: 100%" name="name" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="产生时间:" prompt="填报时间" style="width: 100%" name="apply_time" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="是否历史核销（4月以前）:" prompt="是否历史核销（4月以前）" style="width: 100%" name="isPrepaid" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="产品:" prompt="产品" style="width: 100%" name="product" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px" id="project">
                    <input class="easyui-textbox" label="项目编号:" prompt="项目编号" style="width: 100%" name="project" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="所属部门:" prompt="所属部门" style="width: 100%" name="department" data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="费用归属部门:" prompt="费用归属部门" style="width: 100%" name="fee_department" data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" id="fee_detail" label="费用明细:" prompt="费用明细" style="width: 100%" name="fee_detail" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="网点:" prompt="网点" style="width: 100%" name="branch" data-options="editable:false,multiline:true">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="金额:" prompt="金额" style="width: 100%" name="fee_amount" data-options="editable:false">
                </div>
                <div style="margin-bottom: 10px">
                    <input class="easyui-textbox" label="实报金额:" prompt="实报金额" style="width: 100%" name="actual_fee_amount" data-options="editable:false">
                </div>
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
                <div style="margin-bottom:10px">
                    <a id="detailInfo" style="width: 100%;" href="javascript:void(0)" class="easyui-linkbutton"
                        onclick="$.mobile.go('#p5');">查看明细信息</a>
                </div>
            </form>
        </div>
        <div style="padding: 20px 40px" id="footer">
            <a id="sumbitter" href="#" class="easyui-linkbutton c1" data-options="iconAlign:'top',size:'small'"></a>
            <span>-></span>
        </div>
        <div id="p5" class="easyui-navpanel" style="position: relative; padding: 20px">
            <header>
                <div class="m-toolbar">
                    <span id="p5-title" class="m-title">报销明细数据</span>
                    <div class="m-left">
                        <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true"
                            onclick="$.mobile.back()">返回</a>
                    </div>
                </div>
            </header>
            <div>
                <ul class="m-list" id="reimburseDetailApply"></ul>
            </div>
        </div>
    </div>
    <script src="Scripts/jquery-weui/jquery-weui.js"></script>
    <script type="text/javascript">
        //禁止浏览器返回按钮
        //history.pushState(null, null, document.URL);
        //window.addEventListener('popstate', function () {
        //    history.pushState(null, null, document.URL);
        //});

        var Url = "mRelatedReimburse.aspx";
        var loading = false; var totalData;
        var pageSize = 5; var pageStart = 0; var level = 0;
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
        function ShowDocumentByCode(code) {
            var res = AjaxSync(Url, { act: 'getDocument', docCode: code });
            try {
                showProcessAndAttachment(data[0]);
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
        }

        function showProcessAndAttachment(data) {
            if (data.status != '审批中' && data.status != '已拒绝') {
                $("#cancelDiv").css("display", "none");
            } else {
                $("#cancelDiv").css("display", "block");
                $("#cancel").attr("onclick", "cancel('" + data.code + "')")
            }
            Loading(true);
            $.ajax({
                url: 'mMySubmittedReimburse.aspx',
                data: { act: 'getProcessInfoAndAttachment', docCode: data.id },
                dataType: 'json',
                type: 'post'
            }).
                done(msg => {
                    var data1 = JSON.parse(msg.data);
                    var processData = JSON.parse(msg.count);
                    var attachment = JSON.parse(msg.attachment);
                    var reimburseDetailData = JSON.parse(msg.detail);

                    $("#uploaderFiles").empty();
                    if (attachment != null && attachment.length > 0) {
                        var imageHtml = "";
                        for (i = 0; i < attachment.length; i++) {
                            imageHtml += "<li class=\"weui-uploader__file\" style=\"background-image:url(" + attachment[i].ImageUrl + ")\"></li>";
                        }
                        $("#uploaderFiles").append(imageHtml);
                    }

                    var num = processData.length;
                    var html = "";
                    $.each(processData, function (i, v) {
                        html += v.Name + " " + v.Time + " " + v.ApprovalResult + " " + v.ApprovalOpinions + "\r\n";
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
                        //$.parser.parse();
                        // initDatebox();
                    })
                    $("#footer").children("span:last").remove();

                    if (data.project == "请选择" || data.project == null) {
                        $("#project").css("display", "none");
                    }
                    if (data.isPrepaid == '0') {
                        data.isPrepaid = '否'
                    } else {
                        data.isPrepaid = '是'
                    }

                    $('#searchBar').hide();
                    $('#fm').form('load', data);
                    $("#docId").val(data.id);
                    level = data.Level;
                    if (data.isOverBudget == 1) {
                        $('#isOverBudget').html("(预算外单据)");
                    }
                    else {
                        $('#isOverBudget').html("");
                    }

                    $('#reimburseDetailApply').empty();
                    for (let i = 0; i < reimburseDetailData.length; i++) {
                        let html = '<li>发票类型:'
                        html += '<input disabled class="receiptCode" style="width: 40%; float: right; border: 0px" value="' + reimburseDetailData[i].FeeType + '"/>'
                        html += '</li>'
                        html += '<li class="specialLI">发票用途:'
                        html += '<input disabled class="receiptCode" style="width: 40%; float: right; border: 0px" value="' + reimburseDetailData[i].ReceiptType + '"/>'
                        html += '</li>'
                        html += '<li>上传发票:'
                        html += '<div class="weui-gallery" id="gallery" onclick="hiddenImage(this)">'
                        html += '<span class="weui-gallery__img" id="galleryImg"></span>'
                        html += '<div class="weui-gallery__opr">'
                        html += '</div>'
                        html += '</div>'
                        html += '<div class="weui-cells weui-cells_form" style="margin-top:0px">'
                        html += '<div class="weui-cell">'
                        html += '<div class="weui-cell__bd">'
                        html += '<div class="weui-uploader">'
                        html += '<div class="weui-uploader__bd">'
                        html += '<ul class="weui-uploader__files" id="receiptImages" onclick="showImage(this)">'
                        html += '<li class="weui-uploader__file" style="background-image:url(\'' + reimburseDetailData[i].ReceiptAttachment + '\')"></li>'
                        html += '</ul>'
                        html += '<div class="weui-uploader__input-box" style="display: none">'
                        html += ' <input id="receiptImage" name="receiptImage" onchange="uploadReceiptImage(this)" class="weui-uploader__input zjxfjs_file"'
                        html += ' type="file" accept="image" style="width:79px;height:79px !important;">'
                        html += '<input class="receiptImage" type="hidden" style="width: 40%; float: right; border: 0px" />'
                        html += '</div>'
                        html += '</div>'
                        html += '</div>'
                        html += '</div>'
                        html += '</div>'
                        html += '</li>'
                        html += '<li>发票日期:'
                        html += '<input disabled class="receiptTime" name="date" style="width: 40%; float: right; border: 0px" value="' + reimburseDetailData[i].ReceiptDate.substring(0, reimburseDetailData[0].ReceiptDate.indexOf(' ')) + '"/>'
                        html += '</li>'
                        html += '<li>发票编码:'
                        html += '<input disabled class="receiptCode" style="width: 40%; float: right; border: 0px" value="' + reimburseDetailData[i].ReceiptCode + '"/>'
                        html += '</li>'
                        html += '<li>发票号码:'
                        html += '<input disabled class="receiptNum" style="width: 40%; float: right; border: 0px" value="' + reimburseDetailData[i].ReceiptNum + '"/>'
                        html += '</li>'
                        html += '<li>发票金额:'
                        html += ' <input disabled class="receiptAmount" type="number" name="date" style="width: 40%; float: right; border: 0px" value="' + reimburseDetailData[i].ReceiptAmount + '"/>'
                        html += '</li>'
                        html += '<li>发票税额:'
                        html += ' <input disabled class="receiptTax" type="number" name="date" style="width: 40%; float: right; border: 0px" value="' + reimburseDetailData[i].ReceiptTax + '"/>'
                        html += '</li>'
                        html += '<li>发票地点:'
                        html += '<input disabled class="receiptAddress" type="text" name="date" style="width: 40%; float: right; border: 0px" value="' + reimburseDetailData[i].ReceiptPlace + '"/>'
                        html += '</li>'
                        html += '<li>活动内容描述:'
                        html += '<textarea disabled class="receiptDescr" name="date" style="width: 40%; height:100px;float: right; border: 0px">' + reimburseDetailData[i].ReceiptDesc + '</textarea>'
                        html += '</li>';
                        html += ' <br /> <HR style="border:3px double #987cb9" width="100%" color=#987cb9 SIZE=3> <br />';

                        $('#reimburseDetailApply').append(html);
                    }

                    if ($("#fee_detail").textbox('getValue').indexOf("推广活动") > -1 || $("#fee_detail").textbox('getValue').indexOf("区域日常") > -1) {
                        $(".qytgSelect").css('display', 'block');
                    } else if ($("#fee_detail").textbox('getValue').indexOf("差旅费") > -1) {
                        $(".clSelect").css('display', 'block');
                    } else if ($("#fee_detail").textbox('getValue').indexOf("会议费") > -1) {
                        $(".hySelect").css('display', 'block');
                    } else if ($("#fee_detail").textbox('getValue').indexOf("岗位补贴") > -1) {
                        $(".gwbtSelect").css('display', 'block');
                    } else if ($("#fee_detail").textbox('getValue').indexOf("培训费") > -1) {
                        $(".pxSelect").css('display', 'block');
                    }

                    $.parser.parse($('#reimburseDetailApply'));

                    $.mobile.go('#p2');
                    Loading(false);
                })
        }

        function DgLoad(sort, order, keyword) {
            if (sort == undefined || sort == "") {
                sort = 'code'; order = 'desc';
            }
            var data = {
                act: 'getInfosRelatedToMe',
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

        function agree() {
            approve("同意");
        }
        function disagree() {
            approve("不同意");
        }

        function approve(result) {
            //var obj = $('#fm2').serializeArray();
            //var data = JSON.stringify(obj);
            var opinion = $('#opinion').textbox('getValue');
            var data = { act: 'approvalReimburse', docCode: $("#docId").textbox("getValue"), ApprovalResult: result, ApprovalOpinions: opinion };
            Loading(true);
            $.ajax({
                url: Url,
                data: data,
                type: 'post',
                dataType: 'json',
                success: function (res) {
                    Loading(false);
                    if (res == "当前用户无审批权限")
                        window.location.href = "http://yelioa.top/mMobileReimbursement.aspx";

                    $.messager.alert('提示', '审批成功', 'info', function (data) {
                        window.location.href = "http://yelioa.top/mMobileReimbursement.aspx";
                    });
                }
            })
        }
    </script>
</body>
</html>
