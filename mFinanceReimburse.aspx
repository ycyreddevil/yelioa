<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mFinanceReimburse.aspx.cs" Inherits="mFinanceReimburse" %>

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
    <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    <script src="Scripts/weui.min.js"></script>
    <script src="Scripts/weui-upload.js"></script>
    <script src="Scripts/ajaxfileupload.js"></script>
    <style>
        .m-list .textbox.easyui-fluid {
            border: none;
        }

        #_easyui_textbox_input1 {
            text-align: right
        }

        #_easyui_textbox_input2 {
            text-align: right
        }

        #_easyui_textbox_input4 {
            text-align: right
        }

        input[type=date]::-webkit-inner-spin-button {
            visibility: hidden;
        }

        input[type="date"]::-webkit-clear-button {
            display: none;
        }

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
    <div id="p1" class="easyui-navpanel" style="position: relative; padding: 20px" data-options="footer:'#footer'">
        <header>
            <div class="m-toolbar">
                <span class="m-title" id="title">移动报销</span>
                <div class="m-left">
                    <a id="getApprover" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true"
                        onclick="getProcessInfo()" style="width:100px">获取审批人</a>
                </div>
                <div class="m-right">
                    <a id="submit" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true"
                        onclick="submit()" style="width:60px">提交</a>
                </div>
            </div>
        </header>

        <ul class="m-list">
            <li>日期:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="apply_time" type="date" name="date" style="width: 40%; float: right; border: 0px" />
                </a>
            </li>
            <li id="isHistoricalVerification">是否公司垫付:
                <a style="text-align: right; width: 60%; float: right" href="javascript:void(0)">
                    <select id="isPrepaid">
                        <option value="0">否</option>
                        <option value="1">是</option>
                    </select>
                </a>
            </li>
            <li id="isHasReceiptLi" style="display:none">是否到票:
                <a style="text-align: right; width: 60%; float: right" href="javascript:void(0)">
                    <select id="isHasReceipt">
                        <option value="1">是</option>
                        <option value="0">否</option>
                    </select>
                </a>
            </li>
            <li id="isOverBudgetLi">是否超预算:
                <a style="text-align: right; width: 60%; float: right" href="javascript:void(0)">
                    <select id="isOverBudget">
                        <option value="0">否</option>
                        <option value="1">是</option>
                    </select>
                </a>
            </li>
            <li id="departmentLi" style="display: none">部门:<a id="department"
                style="text-align: right; width: 60%; float: right" href="javascript:void(0)"
                onclick="openit('部门')">请选择</a></li>
            <li>产品:<a id="product" style="text-align: right; width: 80%; float: right" href="javascript:void(0)"
                onclick="openit('产品')">请选择</a></li>
            <li>网点:<a id="branch" style="text-align: right; width: 80%; float: right" href="javascript:void(0)"
                onclick="openit('网点')">请选择</a></li>
            <li>费用归属部门:<a id="fee_department" style="text-align: right; width: 60%; float: right" href="javascript:void(0)"
                onclick="openit('费用归属部门')">请选择</a></li>
            <li>费用归属公司:
                <a style="text-align: right; width: 50%; float: right" href="javascript:void(0)"><select id="fee_company" style="width:100%"><option selected value="江西东森科技发展有限公司">江西东森科技发展有限公司</option>
                <option value="江西业力医疗器械有限公司">江西业力医疗器械有限公司</option><option value="南昌市中申医疗器械有限公司">南昌市中申医疗器械有限公司</option><option value="江西业力科技集团有限公司">江西业力科技集团有限公司</option><option value="南昌老康科技有限公司">南昌老康科技有限公司</option>
                <option value="天津吉诺泰普生物科技有限公司">天津吉诺泰普生物科技有限公司</option><option value="南昌业力医学检验实验室有限公司">南昌业力医学检验实验室有限公司</option><option value="九江傲沐科技发展有限公司">九江傲沐科技发展有限公司</option>
                <option value="上海恩焯企业管理咨询中心">上海恩焯企业管理咨询中心</option><option value="上海会帆企业管理咨询中心">上海会帆企业管理咨询中心</option>
                 </select></a></li>
            <li>费用明细:<a id="fee_detail" style="text-align: right; width: 50%; float: right" href="javascript:void(0)"
                onclick="openit('费用明细')">请选择</a></li>
            <li id="travelApplyLi"  style="display: none">勾选已提交的差旅申请:<a id="travelApply"
                style="text-align: right; width: 70%; float: right" href="javascript:void(0)"
                onclick="openit('差旅申请')">请选择</a></li>
            <li id="ylProjectLi" style="display: none">研发项目编号:<a id="project"
                style="text-align: right; width: 70%; float: right" href="javascript:void(0)"
                onclick="openit('项目编号')">请选择</a></li>
            <li id="acProjectLi" style="display: none">推广活动申请编号:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="acProject" data-options="" class="easyui-textbox" name="project"
                        style="width: 40%; float: right; border: 0px" />
                </a>
            </li>
            <%--<li id="loanLi"  style="display: block">勾选已提交的借款单:<a id="loan"
                style="text-align: right; width: 70%; float: right" href="javascript:void(0)"
                onclick="openit('借款单')">请选择</a></li>--%>
            <li>金额:
                <a style="text-align:right;width:80%;float:right" href="javascript:void(0)">
                    <input id="fee_amount" data-options="min:0,precision:2" class="easyui-numberbox" name="fee_amount"
                        style="width:40%;float:right;border:0px;"/>
                </a>
            </li>
            <li>知悉人:<a id="informerName" style="text-align: right; width: 60%; float: right" href="javascript:void(0)"
                onclick="openit('知悉人')">请选择</a></li>
            <li>备注:
                <br />
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input data-options="multiline:true" id="remark" class="easyui-textbox" name="remark"
                        style="width: 90%; float: right; border: 0px; height: 150px" />
                </a>
            </li>
        </ul>
        <div class="weui-gallery" id="gallery">
            <span class="weui-gallery__img" id="galleryImg"></span>
            <div class="weui-gallery__opr">
                <a href="javascript:" class="weui-gallery__del">
                    <i class="weui-icon-delete weui-icon_gallery-delete"></i>
                </a>
            </div>
        </div>
        <div class="weui-cells weui-cells_form">
            <div class="weui-cell">
                <div class="weui-cell__bd">
                    <div class="weui-uploader">
                        <div class="weui-uploader__hd">
                            <p class="weui-uploader__title" style="font-size: 10px">图片上传:</p>
                        </div>
                        <div class="weui-uploader__bd">
                            <ul class="weui-uploader__files" id="uploaderFiles">
                            </ul>
                            <div class="weui-uploader__input-box">
                                <input id="uploaderInput" name="uploaderInput" class="weui-uploader__input zjxfjs_file"
                                    type="file" accept="image/*" multiple="">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div style="padding: 20px 40px" id="footer">
        <a id="sumbitter" href="#" class="easyui-linkbutton" data-options="iconAlign:'top',size:'small'">提交人
        </a>
        <span>-></span>
    </div>
    <div id="p2" class="easyui-navpanel" data-options="footer:'#footer2'">
        <header>
            <div class="m-toolbar">
                <span id="p2-title" class="m-title">Detail</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true"
                        onclick="$.mobile.back()">Back</a>
                </div>
                <div class="m-right">
                    <a id="confirmInformer" href="javascript:void(0)" class="easyui-linkbutton" plain="true"
                        outline="true" onclick="confirmInformer()" style="width:60px">确定</a>
                </div>
            </div>
        </header>
        <div>
            <input id="search" class="easyui-textbox" style="width: 100%;">
            <ul class="m-list" id="detailList">
            </ul>
            <ul id="tree" class="easyui-tree" data-options="cascadeCheck:false"></ul>
            <div id="dlDiv">
                <div id="dl" style="height: 500px" data-options="
                    border: false,
                    lines: true,
                    singleSelect: false
                    ">
                </div>
            </div>
        </div>
        <div style="padding: 20px 40px" id="footer2">
            <a id="informerFooter" href="#" class="easyui-linkbutton"
                data-options="iconAlign:'right',size:'small'">知悉人:</a>
        </div>
    </div>
    <div id="p3" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span id="p3-title" class="m-title">Detail</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true"
                        onclick="$.mobile.back()">Back</a>
                </div>
            </div>
        </header>
        <div>
            <ul class="m-list" id="childrenFeeDetailList">
            </ul>
            <div id="dl2" style="height: 500px" data-options="
                border: false,
                lines: true,
                singleSelect: false
                ">
            </div>
        </div>
    </div>
    <div id="p4" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span id="p4-title" class="m-title">提示</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true"
                        onclick="$.mobile.back()">返回</a>
                </div>
                <div class="m-right">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true"
                        onclick="$.mobile.back()">确定</a>
                </div>            
            </div>
        </header>
        <div>
            <div id="dl4" style="height: 500px" data-options="
                border: false,
                lines: true,
                singleSelect: false
                ">
                <img src="resources/instructions.png" style="width: 100%" />
                <div style="margin: 50px 0 0; text-align: center">
                    <a href="javascript:void(0)" class="easyui-linkbutton" style="width: 200px; height: 60px" onclick="$.mobile.back()">请认真阅读，选错可能延误报销</a>
                </div>
            </div>
        </div>
    </div>
    <div id="dlg" class="easyui-dialog" title="请确定这次报销的明细数据" style="width: 80%; height: 350px; padding: 10px" data-options="closed:true,modal:true,
			buttons: [{
				text:'确定',
				iconCls:'icon-ok',
				handler:function(){
                    $('#dlg').dialog('close');
					submit();
				}
			},{
				text:'取消',
				handler:function(){
                    $('#dlg').dialog('close');
                    $('#reimburseDetailList').empty();
				}
			}]
		">
        <ul class="m-list" style="height:250px;" id="reimburseDetailList"></ul>
    </div>

    <script type="text/javascript">
        var uploadFileUrls = new Array();
        var productFlag = false; var branchFlag = false; var departmentFlag = false; var detailFlag = false;
        var i = 2; var approvers;
        var url = "mFinanceReimburse.aspx";
        var processFlag = false;
        var chooseInformerId = new Array();
        var chooseInformer = new Array();
        var approverData;
        var IsMarketingActivity = false;
        var reimburseDetail = new Array();
        var reimburseDetailImage = new Array();
        var isSalesDepartment = false

        $(document).ready(function () {
            $('#isPrepaid').change(function () {
                PrepaidAndHasReceiptValueChange();
            });
            $('#isHasReceipt').change(function () {
                PrepaidAndHasReceiptValueChange();
            });
            $('#dl').datalist();
            $("#apply_time").val(formatterDate(new Date()));
            // 判断是否存在多部门
            checkMultiDepartment();

            // 判断是否要回显数据
            var docCode = GetQueryString("docCode");
            if (docCode != "" && docCode != "undefined" && docCode != null) {
                dataEchoed(docCode);
            }

            var isOverBudget = GetQueryString("isOverBudget");
            if (isOverBudget == "1") {
                $("#title").html("预算外移动报销")
            }
        });

        $("#isPrepaid").change(function () {
            if ($("#isPrepaid").val() == '1') {
                $("#isHasReceiptLi").css('display', 'block')
                $('#loanLi').css('display', 'none')
            } else {
                $("#isHasReceiptLi").css('display', 'none')
                $('#loanLi').css('display', 'block')
            }
        })

        $(document).keydown(function (event) {
            if (event.keyCode == 13) {
                var target = $('#p2-title').html();
                var url = ""
                if (target == '产品') {
                    url = "findProductName";
                } else if (target == "网点") {
                    url = "findBranch";
                } else if (target == "费用明细") {
                    url = "findFeeDetail";
                } else if (target == "费用归属部门") {
                    url = "findFeeDepartment";
                } else if (target == "项目编号") {
                    url = "findProjectCode";
                } else {
                    url = "findInformer";
                }
                var name = $("#search").textbox("getValue");
                findName(name, url);
            }
        });
        var checkMultiDepartment = function () {
            $.ajax({
                url: url,
                data: { act: 'checkMultiDepartment' },
                dataType: 'json',
                type: 'post',
                success: function (data) {
                    if (data.length > 1) {
                        $("#departmentLi").css("display", "block")
                    } else {
                        $("#department").html(data[0].value)
                    }

                    /////////////////////////////////用于销售部人员不显示是否历史核销及是否到票
                    $.each(data, function (i, val) {
                        if (val.value.indexOf("营销中心/销售部") != -1) {
                            isSalesDepartment = true
                            //$("#isHistoricalVerification").css("display", "none");
                            $("#isHasReceiptLi").css("display", "none");
                            return false;
                        }
                    });

                },
                error: function (data) {
                    console.log(data);
                }
            });
        }

        var dataEchoed = function (docCode) {
            $.ajax({
                url: url,
                data: { act: 'dataEchoed', docCode: docCode },
                dataType: 'json',
                type: 'post',
                success: function (data) {
                    // 数据回显
                    var data = data[0];

                    //////////////////////////////////////////////Add by yyy 市场推广活动需要填写项目申请编号
                    if (data["fee_detail"] == "推广活动市场费" || data["fee_detail"] == "推广活动开发费" || data["fee_detail"] == "推广活动渠道费") {
                        $("#acProjectLi").css("display", "block");
                        IsMarketingActivity = true;
                    }
                    //////////////////////////////////////////////Add by yyy 市场推广活动需要填写项目申请编号 END
                    if (data["fee_detail"] == "研发费用金额") {
                        $("#ylProjectLi").css("display", "block");
                    }

                    // 是否核销数据以及是否到票数据回显
                    $("#isPrepaid").val(data["isPrepaid"]);
                    $("#isHasReceipt").val(data["isHasReceipt"])

                    for (var key in data) {
                        if (key == 'fee_company' || key == 'isOverBudget') {
                            $("#" + key).val(data[key]);
                        } else if (key != 'isPrepaid' && key != 'isHasReceipt') {
                            try {
                                $("#" + key).textbox("setValue", data[key]);
                            } catch (err) {
                                $("#" + key).html(data[key]);
                            }
                        }
                    }

                    // 把知悉人id带入
                    chooseInformerId = data.informerId.split(",");
                    // 把附件地址带入
                    uploadFileUrls = data.attachmentUrl.split(",");
                    if (uploadFileUrls != '' && uploadFileUrls != 'undefined') {
                        for (i = 0; i < uploadFileUrls.length; i++) {
                            var html = '<li class="weui-uploader__file" style="background-image:url(\'' + uploadFileUrls[i] + '\')"></li>';
                            $("#uploaderFiles").append(html);
                        }
                    }
                },
                error: function (data) {
                    console.log(data)
                }
            })
        }

        var checkApprover = function () {
            getProcessInfo();
        }

        $("#search").textbox({
            icons: [{
                iconCls: 'icon-search',
                handler: function (e) {
                    var target = $('#p2-title').html();
                    var url = ""
                    if (target == '产品') {
                        url = "findProductName";
                    } else if (target == "网点") {
                        url = "findBranch";
                    } else if (target == "费用明细") {
                        url = "findFeeDetail";
                    } else if (target == "费用归属部门") {
                        url = "findFeeDepartment";
                    } else if (target == "项目编号") {
                        url = "findProjectCode";
                    } else {
                        url = "findInformer";
                    }
                    var name = $("#search").textbox("getValue");
                    findName(name, url);
                }
            }],
        })

        var openit = function (target) {
            var url = ""
            if (target == '产品') {
                url = "findProductName";
            } else if (target == "网点") {
                url = "findBranch";
            } else if (target == "费用明细") {
                url = "findFeeDetail";
            } else if (target == "费用归属部门") {
                url = "findFeeDepartment";
            } else if (target == "部门") {
                url = "checkMultiDepartment";
            } else if (target == "项目编号") {
                url = "findProjectCode";
            } else if (target == "差旅申请") {
                url = "findTravelApply";
            } else if (target == "借款单") {
                url = "findLoan";
            } else {
                url = "findInformer";
            }

            findName("", url);
            $('#p2-title').html(target);
            $("#search").textbox("setValue", "");
            $.mobile.go('#p2');
        }

        var setValue = function (act, data) {
            if (act == "findProductName") {
                $("#product").html(data);
                $.mobile.go('#p1');
            } else if (act == "findBranch") {
                $("#branch").html(data);
                $.mobile.go('#p1');
            } else if (act == "findFeeDetail") {
                //$("#fee_detail").html(data);
                $("#childrenFeeDetailList").empty();
                $.ajax({
                    url: 'mFinanceReimburse.aspx',
                    data: { act: 'findChildrenFeeDetail', name: data, department: $("#fee_department").html() },
                    dataType: 'json',
                    type: 'post',
                    success: function (childrenData) {
                        //////////////////////////////////////////////Add by yyy 市场推广活动需要填写项目申请编号
                        if (data == "推广活动市场费" || data == "推广活动渠道费") {
                            $("#acProjectLi").css("display", "block");
                            IsMarketingActivity = true;
                        } else {
                            $("#acProjectLi").css("display", "none");
                            IsMarketingActivity = false;
                        }
                        //////////////////////////////////////////////Add by yyy 市场推广活动需要填写项目申请编号 END
                        if (data != "研发费用金额")
                            $("#ylProjectLi").css("display", "none");
                        else
                            $("#ylProjectLi").css("display", "block");

                        if (childrenData == "" || childrenData == null) {
                            if (data.indexOf("差旅费") > -1 && isSalesDepartment)
                                $("#travelApplyLi").css("display", "block")
                            else
                                $("#travelApplyLi").css("display", "none")

                            $("#fee_detail").html(data);
                            $.mobile.go('#p1');
                        } else {
                            var html = "";
                            for (i = 0; i < childrenData.length; i++) {
                                html += '<li><a href="javascript:void(0)" onclick="setChildrenFeeDetailValue(\'' +
                                    act +
                                    '\',\'' +
                                    childrenData[i].target +
                                    '\',\'' +
                                    data +
                                    '\')">' +
                                    childrenData[i].target +
                                    '</a></li>';
                            }
                            $("#p3-title").html(data);
                            $("#childrenFeeDetailList").append(html);
                            $.mobile.go('#p3');
                        }
                    }
                });
            } else if (act == "findFeeDepartment") {
                $("#fee_department").html(data);
                $.mobile.go('#p1');
            } else if (act == "checkMultiDepartment") {
                $("#department").html(data);
                $.mobile.go('#p1');
            } else if (act == "findProjectCode") {
                $("#project").html(data);
                $.mobile.go('#p1');
            } else {
                $("#informer").html(data);
                $.mobile.go('#p1');
            }
        }

        var setChildrenFeeDetailValue = function (act, data, parentData) {
            if (data.indexOf("差旅费") > -1 && isSalesDepartment)
                $("#travelApplyLi").css("display", "block")
            else
                $("#travelApplyLi").css("display", "none")

            $("#fee_detail").html(parentData + '-' + data);
            if (parentData == "研发费用金额") {
                $("#ylProjectLi").css("display", "block");
            }

            $.mobile.go('#p1');
        }

        var getProcessInfo = function () {
            var feeDetail = $("#fee_detail").html();
            var feeDepartment = $("#fee_department").html();
            var feeAmount = $("#fee_amount").numberbox("getValue");
            var department = $("#department").html();
            if ($("#departmentLi").css("display") == "none")
                department = "";

            approvers = "";

            if (feeAmount != "" && feeDetail != "请选择" && feeDepartment != "请选择" && department != '请选择') {
                processFlag = true;
                var isOverBudget;

                if (GetQueryString("isOverBudget") == "1" || $("#isOverBudget").val() == "1") {
                    isOverBudget = "1"
                } else {
                    isOverBudget = "0"
                }
                Loading(true)
                $.ajax({
                    url: 'mFinanceReimburse.aspx',
                    data: { act: 'getProcessInfo', feeDetail: feeDetail, feeDepartment: feeDepartment, feeAmount: feeAmount, department: department, isOverBudget: isOverBudget },
                    dataType: 'json',
                    type: 'post',
                    success: function (data) {
                        approverData = data.approverData;

                        $("#footer").empty();
                        var html = '<a id="sumbitter" href="#" class="easyui-linkbutton" data-options="iconAlign:\'top\',size:\'small\'">提交人 </a><span>-></span>'
                        $("#footer").append(html);

                        $("#sumbitter").linkbutton({ text: data.selfName });
                        approvers = data.leaders;
                        var leaders = data.leaders.split(",");
                        $.each(leaders, function (i, v) {
                            if (v != '') {
                                html = '<a href="#" class="easyui-linkbutton" data-options="iconAlign:\'top\',size:\'small\'">';
                                html += v;
                                html += "</a >";
                                html += "<span>-></span>";
                                $("#footer").children("span:last").after(html);
                            }
                            $.parser.parse($("#sumbitter").parent());
                        })
                        $("#footer").children("span:last").remove();
                        Loading(false);
                    }
                })
            } else {
                $.messager.alert('提示', '请填写金额，费用归属部门，部门和费用明细后才点击生成审批人', 'info')
            }
        }

        var findName = function (name, act) {
            Loading(true)
            if (act == "findFeeDepartment") {
                Loading(false);
                $("#detailList").empty();
                $('#tree').empty();
                $("#dlDiv").hide();
                $("#search").textbox('disable');
                initTree();
                TreeLoad();
            } else {
                $("#dlDiv").show();
                if (act == "findFeeDetail") {
                    if ($("#fee_department").html() == "" || $("#fee_department").html() == "请选择") {
                        $.messager.alert('提示', '请先选择费用归属部门，再选择费用明细！', 'info', function () {
                            $.mobile.go('#p1');
                            Loading(false);
                        });
                        return;
                    }
                    $("#search").textbox('disable');
                } else {
                    $("#search").textbox('enable');
                }

                $.ajax({
                    url: 'mFinanceReimburse.aspx',
                    data: { act: act, name: name, department: $("#fee_department").html(), product: $("#product").html() },
                    type: 'post',
                    dataType: 'json',
                    success: function (msg) {
                        Loading(false)
                        $("#detailList").empty();
                        $('#tree').empty();
                        chooseInformerId = new Array();
                        chooseInformer = new Array();
                        $.each($("#footer2").children("a"),
                            function (i, v) {
                                if (i != 0) {
                                    $(v).remove();
                                }
                            })

                        if (act == "findInformer") {
                            $("#confirmInformer").attr("onclick", "confirmInformer()")
                            $("#detailList").empty();
                            var informer = new Array();
                            for (i = 0; i < msg.length; i++) {
                                informer.push({
                                    "item": msg[i].target,
                                    "userId": msg[i].value
                                });
                            }
                            $('#dl').datalist({
                                data: informer,
                                textField: 'item',
                                valueField: 'userId',
                                onClickRow: function (index, row) {
                                    if (contains(chooseInformer, row.item)) {
                                        chooseInformer.splice(indexOf(chooseInformer, row.item), 1);
                                        chooseInformerId.splice(indexOf(chooseInformerId, row.userId), 1);
                                        $.each($("#footer2").children("a"),
                                            function (i, v) {
                                                if (i != 0 &&
                                                    ($(v).text().replace(/^\s+|\s+$/g, "") == row.item ||
                                                        $(v).text() == row.item)) {
                                                    $(v).remove();
                                                    return;
                                                }
                                            })
                                    } else {
                                        chooseInformer.push(row.item);
                                        chooseInformerId.push(row.userId);
                                        var html = '<a href="javascript:void(0)" onclick="removeInformer(this,\'' +
                                            row.item +
                                            '\',\'' +
                                            row.userId +
                                            '\',\'' +
                                            index +
                                            '\')" class="easyui-linkbutton" data-options="iconCls:\'icon-no\',iconAlign:\'right\',size:\'small\'">';
                                        html += row.item;
                                        html += "</a >";
                                        $("#informerFooter").after(html);
                                        $.parser.parse($("#informerFooter").parent());
                                    }
                                }
                            })
                        }
                        else if (act === 'findTravelApply') {
                            $("#confirmInformer").attr("onclick", "confirmTravelApply()")
                            $("#detailList").empty();
                            $('#dl').datalist({
                                data: msg,
                                textField: 'target',
                                valueField: 'value',
                            })
                        }
                        else if (act === 'findLoan') {
                            $("#confirmInformer").attr("onclick", "confirmLoan()")
                            $("#detailList").empty();
                            $('#dl').datalist({
                                data: msg,
                                textField: 'target',
                                valueField: 'value',
                            })
                        }
                        else {
                            $("#dl").datalist('loadData', { total: 0, rows: [] })
                            if (msg == "" || msg == null) {
                                if (act == "findFeeDetail") {
                                    $.messager.alert('提示', '所选部门暂无对应费用明细，请重新选择', 'info');
                                } else {
                                    $.messager.alert('提示', '暂无数据，请重新输入', 'info');
                                }
                            } else {
                                var html = "";
                                if (act != "findTravelApply") {
                                    for (i = 0; i < msg.length; i++) {
                                        html += '<li><a href="javascript:void(0)" onclick="setValue(\'' +
                                            act +
                                            '\',\'' +
                                            msg[i].target +
                                            '\')">' +
                                            msg[i].target +
                                            '</a></li>';
                                    }
                                }
                                else {
                                    for (i = 0; i < msg.length; i++) {
                                        html += '<li><a href="javascript:void(0)">' + msg[i].target + '</a></li>';
                                    }
                                }
                                
                                $("#detailList").append(html);
                            }
                        }
                    }
                });
            }
        }

        function CountCharInString(str, c) {
            // var str = "abbbbbsdsdsdasdlsfj";
            // var c = "b"; // 要计算的字符
            var regex = new RegExp(c, 'g'); // 使用g表示整个字符串都要匹配
            var result = str.match(regex);
            return !result ? 0 : result.length;
        }

        var submit = function () {
            var date = $("#apply_time").val();
            var productName = $("#product").html();
            var branchName = $("#branch").html();
            var feeDetail = $("#fee_detail").html();
            var feeDepartment = $("#fee_department").html();
            var feeAmount = $("#fee_amount").numberbox("getValue");
            var informer = $("#informer").html();
            //var pic = $("#file").filebox("getValue");
            var remark = $("#remark").textbox("getValue").replace("/t", "");
            var department = $("#department").html();
            var project = $("#project").html();
            var feeCompany = $("#fee_company").val();
            if (IsMarketingActivity) {
                project = $("#acProject").val();
            }

            if (!processFlag) {
                $.messager.alert('提示', '请先确定审批人！', 'info');
                return;
            }

            if (date == "" || informer == "请选择" || productName == "请选择" || branchName == "请选择" || feeDetail == "请选择" ||
                department == "请选择" || feeAmount == "" || feeCompany == "" || remark == "") {
                $.messager.alert('提示', '存在空值！', 'info');
                return;
            }

            //////////////////////////////////////////////Add by yyy 市场推广活动需要填写项目申请编号
            if (IsMarketingActivity && ((project.length != 12 && project.length != 13)
                && CountCharInString(project, '-') != 2)) {
                $.messager.alert('提示', '推广活动申请编号格式不正确，开发费格式应为“KF-CD1901-01”,市场费格式应为“IA1906-01-01”,渠道费格式应为“QD1901-01-01”', 'info');
                return;
            }

            ///费用明细和产品网点关联 
            if (feeDetail.indexOf("VIP维护") > -1 ||
                (feeDetail.indexOf("推广活动开发费") > -1 && department.indexOf("销售部") > -1) ||
                feeDetail.indexOf("市场调节基金") > -1 || feeDetail.indexOf("推广活动其他") > -1 ||
                (feeDetail.indexOf("推广活动市场费") > -1 && department.indexOf("销售部") > -1) || feeDetail.indexOf("销售折让") > -1) {
                if (productName == "综合" || productName == "非销售" || branchName == "综合" || branchName == "非销售") {
                    $.messager.alert('提示', '该费用科目对应网点产品不能选择综合或非销售', 'info');
                    return;
                }
            }
            else if ((feeDetail.indexOf("推广活动市场费") > -1 && department.indexOf("市场部") > -1) ||
                (feeDetail.indexOf("推广活动开发费") > -1 && department.indexOf("市场部") > -1)) {
                if (productName == "综合" || productName == "非销售") {
                    $.messager.alert('提示', '该费用科目对应产品不能选择综合或非销售', 'info');
                    return;
                }
                if (branchName != "综合") {
                    $.messager.alert('提示', '该费用科目对应网点只能选择综合', 'info');
                    return;
                }
            }
            else if (feeDetail.indexOf("工资社保金额") > -1) {
                if (productName != "非销售" || branchName != "非销售") {
                    $.messager.alert('提示', '该费用科目对应网点产品只能选择非销售', 'info');
                    return;
                }
            } else if ((feeDetail.indexOf("差旅费") > -1 && (feeDepartment.indexOf("销售部") > -1)) || feeDetail.indexOf("培训费") > -1 || feeDetail.indexOf("区域日常费用") > -1 || feeDetail.indexOf("推广活动渠道费") > -1) {
                if (productName != "综合" || branchName != "综合") {
                    $.messager.alert('提示', '该费用科目对应网点产品只能选择综合', 'info');
                    return;
                }
            }

            if ($("#departmentLi").css("display") == "none")
                department = "";

            // 房屋租赁费只能胡春燕提交
            if (feeDetail.indexOf("房租费") > -1 && approverData[0].name != '胡春燕') {
                $.messager.alert('提示', '您无权提交房租费报销！', 'info');
                return;
            }

            // 销售线-差旅费必须关联差旅申请
            if (feeDetail.indexOf("差旅费") > -1 && isSalesDepartment) {
                if ($("#travelApply").html() === '请选择' || $("#travelApply").html() === '') {
                    $.messager.alert('提示', '差旅费必须关联差旅申请', 'info');
                    return
                }

                let travelApplyFlag = true

                $.ajax({
                    url: 'mFinanceReimburse.aspx',
                    data: {
                        act: 'findTravelApplyAmount',
                        code: $("#travelApply").html()
                    },
                    async: false,
                    type: 'post',
                    dataType: 'text',
                    success: function (msg) {
                        const amount = JSON.parse(msg).amount
                        if (parseFloat(amount) < parseFloat(feeAmount)) {
                            $.messager.alert('提示', '报销金额不能高于差旅申请金额！', 'info');
                            travelApplyFlag = false
                        }
                    }
                })

                if (!travelApplyFlag)
                    return
            }

            var isOverBudget;

            if (GetQueryString("isOverBudget") == "1" || $("#isOverBudget").val() == "1") {
                isOverBudget = "1"
            } else {
                isOverBudget = "0"
            }

            Loading(true);
            $.ajax({
                url: 'mFinanceReimburse.aspx',
                data: {
                    act: 'submitReimburse',
                    approvers: approvers,
                    apply_time: date,
                    product: productName,
                    branch: branchName,
                    fee_detail: feeDetail,
                    fee_department: feeDepartment,
                    fee_amount: feeAmount,
                    remark: remark,
                    department: department,
                    project: project,
                    chooseInformerId: JSON.stringify(chooseInformerId),
                    approverData: JSON.stringify(approverData),
                    uploadFileUrls: JSON.stringify(uploadFileUrls),
                    docCode: GetQueryString("docCode"),
                    isOverBudget: isOverBudget,
                    isPrepaid: $("#isPrepaid").val(),
                    isHasReceipt: $("#isHasReceipt").val(),
                    reimburseDetail: JSON.stringify(reimburseDetail),
                    fee_company: feeCompany,
                    travelCode: $("#travelApply").html(),
                    loanCode: $("#loan").html()
                },
                type: 'post',
                dataType: 'text',
                success: function (msg) {
                    Loading(false);
                    $.messager.defaults = { ok: "确定", cancel: "取消" };

                    if (msg != '提交成功') {
                        if (msg.indexOf('预算不足') != -1) {
                            $.messager.confirm('提示', msg.substring(0, msg.lastIndexOf(',')) + ',是否进行预算外提交?', function (r) {
                                if (r) {
                                    var code = msg.substring(msg.lastIndexOf(',') + 1, msg.length)
                                    window.location.href = "mFinanceReimburse.aspx?docCode=" + code + "&isOverBudget=1";
                                } else {

                                }
                            })
                        } else {
                            $.messager.alert('提示', msg);
                        }
                    } else {
                        let alertmsg = '提交成功，是否继续提交新的报销单？'
                        if ('<%=user.userName%>' === '黄艳梅')
                            alertmsg = '恭喜你，报销成功一个亿!是否继续提交新的报销单？'
                        $.messager.confirm('提示', alertmsg, function (data) {
                            if (data) {
                                location.reload();
                            }
                            else {
                                window.location.href = "http://yelioa.top/mMySubmittedReimburse.aspx";
                            }
                        });
                    }

                },
                error: function (msg) {
                    Loading(false);
                    $.messager.alert('提示', '保存失败', 'info', function () { $.mobile.back() });
                    $.mobile.back();
                }
            })
        }

        function removeInformer(a, v, id, index) {
            $(a).remove();
            $('#dl').datalist('unselectRow', index);
            chooseInformer.splice(indexOf(chooseInformer, v), 1);
            chooseInformerId.splice(indexOf(chooseInformerId, id), 1);
        }

        function formatterDate(date) {
            var day = date.getDate() > 9 ? date.getDate() : "0" + date.getDate();
            var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : "0"
                + (date.getMonth() + 1);

            return date.getFullYear() + '-' + month + '-' + day;
        }

        function TreeLoad(selectedId) {
            var url = "MemberManage.aspx";
            var data = {
                act: 'getTree'
            };
            parent.Loading(true);
            $.post(url, data, function (res) {
                parent.Loading(false);
                if (res != "F") {
                    TreeDataJson = res;
                    var datasource = $.parseJSON(res);
                    $('#tree').tree("loadData", datasource);
                    company = $('#tree').tree('getRoot').text;
                    if (selectedId == null) {
                        selectedId = $('#tree').tree('getRoot').id;
                        SelectedNode = $('#tree').tree('getRoot');
                    }
                }
            });
        }

        function initTree() {
            $('#tree').tree({
                animate: true, lines: true,
                onClick: function (node) {
                    // 跨部门只能选择市场部或者运营部
                    //                     if (node.id == 292 || node.id == 293) {
                    $("#fee_department").html(node.text);
                    $('#tree').tree('expand', node.target);
                    //                     } else {
                    //                         $("#fee_department").html("请选择");
                    //                         $.messager.alert('提示', '跨部门只能选择市场部或者商务部');
                    //                     }

                    //$(this).tree('beginEdit', node.target);//点击可编辑

                    //  $.mobile.back();
                },
                formatter: function (node) {
                    var s = node.text;
                    // s += '&nbsp;<span style=\'color:blue\'>(' + node.MemberNumber + ')</span>';
                    return s;
                }
            });
        }
        var confirmInformer = function () {
            $("#informerName").html(chooseInformer);
            $.mobile.back();
        }

        var confirmTravelApply = function () {
            var travelApplys = $("#dl").datalist('getSelections')
            var travelApplyCode = ''
            travelApplys.forEach((v, i) => {
                travelApplyCode += v.value + ','
            })
            $("#travelApply").html(travelApplyCode)
            $.mobile.back();
        }

        var confirmLoan = function () {
            var loans = $("#dl").datalist('getSelections')
            var loanCode = ''
            loans.forEach((v, i) => {
                loanCode += v.value + ','
            })
            $("#loan").html(loanCode)
            $.mobile.back();
        }

        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }
    </script>
</body>

</html>
