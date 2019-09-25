<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mDeliverApplyReport.aspx.cs" Inherits="mDeliverApplyReport" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>发货申请表</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <link href="Scripts/themes/mobile.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/jquery.easyui.mobile.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/echarts/echarts.min.js"></script>
    <script src="Scripts/mobileCommon.js"></script>
    <script src="Scripts/weui.min.js"></script>
    <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
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
                <div class="m-title">发货申请表</div>
                <div class="m-right">
                    <a id="submit" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="submit()" style="width: 60px">提交</a>
                </div>
            </div>
        </header>
        <ul class="m-list">
            <li>发货类型:<a id="deliverType" style="text-align: right; width: 80%; float: right" href="javascript:void(0)" onclick="openit('发货类型')">请选择</a></li>
            <li>网点:<a id="hospitalName" style="text-align: right; width: 80%; float: right" href="javascript:void(0)" onclick="openit('发货单位')">请选择</a></li>
            <li>品种:<a id="productName" style="text-align: right; width: 80%; float: right" href="javascript:void(0)" onclick="openit('品种')">请选择</a></li>
            <li>代理商:<a id="agentName" style="text-align: right; width: 80%; float: right" href="javascript:void(0)" onclick="openit('代理商')">请选择</a></li>
            <li>规格:<a id="spec" style="text-align: right; width: 80%; float: right" href="javascript:void(0)" onclick="openit('规格')">请选择</a></li>
            <li>单位:<a id="unit" style="text-align: right; width: 80%; float: right" href="javascript:void(0)" onclick="openit('单位')">请选择</a></li>
            <li>发货数量:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="applyNumber" data-options="min:1,precision:0" class="easyui-numberbox" name="applyNumber" style="width: 40%; float: right; border: 0px" />
                </a>
            </li>
            <li>现有库存:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="stock" data-options="min:0,precision:0" class="easyui-numberbox" name="stock" style="width: 40%; float: right; border: 0px" />
                </a>
            </li>
            <li>上月纯销:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="netSales" data-options="min:0,precision:0" class="easyui-numberbox" name="netSales" style="width: 40%; float: right; border: 0px" />
                </a>
            </li>
            <li>进货周期:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="period" data-options="min:0,max:30,precision:0" class="easyui-numberbox" name="period" style="width: 40%; float: right; border: 0px" />
                </a>
            </li>
            <li>是否货票同行:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <select id="isStockReceiptTogether" onchange="changeStockReceiptTogether(this.options[this.options.selectedIndex].value)">
                        <option value="是">是</option>
                        <option value="否">否</option>
                    </select>
                </a>
            </li>
            <li>收货地址:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="deliverAddress" data-options="min:0,precision:0" class="easyui-textbox" name="stock" style="width: 40%; float: right; border: 0px" />
                </a>
            </li>
            <li>收货联系人:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="deliverName" data-options="min:0,precision:0" class="easyui-textbox" name="stock" style="width: 40%; float: right; border: 0px" />
                </a>
            </li>
            <li>收货电话:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input id="deliverPhone" data-options="min:0,precision:0" class="easyui-textbox" name="stock" style="width: 40%; float: right; border: 0px" />
                </a>
            </li>
            <li>知悉人:<a id="informer" style="text-align: right; width: 60%; float: right" href="javascript:void(0)" onclick="openit('知悉人')">请选择</a></li>
            <li>备注:
                <a style="text-align: right; width: 80%; float: right" href="javascript:void(0)">
                    <input data-options="multiline:true" id="remark" class="easyui-textbox" name="remark" style="width: 90%; float: right; border: 0px; height: 120px" />
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
                                <input id="uploaderInput" name="uploaderInput" class="weui-uploader__input zjxfjs_file" type="file" accept="image/*" multiple="">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div style="padding: 20px 40px" id="footer">

            <a id="sumbitter" href="#" class="easyui-linkbutton" data-options="iconAlign:'top',size:'small'"></a>
        </div>
    </div>
    <div id="p2" class="easyui-navpanel" data-options="footer:'#footer2'">
        <header>
            <div class="m-toolbar">
                <span id="p2-title" class="m-title">Detail</span>
                <div class="m-left">
                    <a href="javascript:void(0)" class="easyui-linkbutton m-back" plain="true" outline="true" onclick="$.mobile.back()">返回</a>
                </div>
                <div class="m-right">
                    <a id="confirmInformer" href="javascript:void(0)" class="easyui-linkbutton" plain="true" outline="true" onclick="confirmInformer()" style="width: 60px">确定</a>
                </div>
            </div>
        </header>
        <div>
            <input id="search" class="easyui-textbox" style="width: 100%;">
            <ul class="m-list" id="detailList">
            </ul>
            <div id="dl" style="height: 500px" data-options="
                border: false,
                lines: true,
                singleSelect: false
                ">
            </div>
        </div>
        <div style="padding: 20px 40px" id="footer2">
            <a id="informerFooter" href="#" class="easyui-linkbutton" data-options="iconAlign:'right',size:'small'">知悉人:</a>
        </div>
    </div>
    <%-- <form id="form">
            <div style="margin-bottom:10px">
               <select id="deliverType" class="easyui-combobox" name="deliverType" label="发货类型:*" style="width:100%">
                    <option value="0">发公司直营网点</option>
                    <option value="1">发商业单位</option>
                    <option value="2">发代理商</option>
                    <option value="3">外购</option>
                    <option value="4">借货</option>
                    <option value="5">赠品/样品</option>
                </select>
            </div>
            <div style="margin-bottom:10px">
                <input id="hospitalName" name="hospitalName" value="" label="发货单位:*" prompt="发货单位" style="width:100%">
            </div>
            <div style="margin-bottom:10px">
                <input id="productName" name="productName" value="" label="品种:*" prompt="品种" style="width:100%">
            </div>
            <div style="margin-bottom:10px">
                <input id="spec" name="spec" label="规格:*" prompt="规格" style="width:100%">
            </div>
            <div style="margin-bottom:10px">
                <input id="unit" name="unit" label="单位:*" prompt="单位" style="width:100%">
            </div>
            <div style="margin-bottom:10px">
                <input id="applyNumber" name="applyNumber" class="easyui-numberbox" label="发货数量:*" prompt="发货数量" style="width:100%">
            </div>
            <div style="margin-bottom:10px">
                <input id="knowMember" name="remark" class="easyui-textbox" label="知悉人:" prompt="知悉人" style="width:100%">
            </div>
            <div style="margin-bottom:10px">
                <input id="remark" name="remark" class="easyui-textbox" label="备注:" prompt="备注" style="width:100%">
            </div>
        </form>
    </div>--%>
</body>
<script>
    var productFlag = false; var hospitalFlag = false;
    var chooseInformer = new Array();
    var chooseInformerId = new Array();
    var uploadFileUrls = new Array();

    $(function () {
        InitProcessInfo();
        $('#dl').datalist(); //初始化datalist
    })
    function changeStockReceiptTogether(val) {
        if (val === '否')
            $.messager.alert('提示', '如果货票不同行，请在备注中注明发票收货信息', 'info')
    }

    var openit = function (target) {
        var data;
        var productName = $("#productName").html();
        if (target == "发货类型") {
            data = { act: "findDeliverType", q: "" };
        }
        else if (target == "知悉人") {
            data = { act: "findInformer", q: "" };
        }
        else if (target == "发货单位") {
            $("#agentName").html('');
            data = { act: "findHospitalName", q: "" };
        }
        else if (target == "代理商") {
            data = { act: "findAgentName", q: $("#hospitalName").html(),};
        }
        else if (target == "品种") {
            data = { act: "findProductName", q: "" };
        }
        else if (productName == "请选择") {
            $.messager.alert('提示', '未输入品种，无法选择规格和单位！', 'info');
            return;
        }
        else if (target == "规格") {
            data = { act: "findSpec", q: productName };
        }
        else if (target == "单位") {
            data = { act: "findUnit", q: productName };
        }

        findName(data);
        $('#p2-title').html(target);
        $.mobile.go('#p2');
    }
    $("#search").textbox({
        icons: [{
            iconCls: 'icon-search',
            handler: function (e) {
                var target = $('#p2-title').html();
                var urldata;
                var q = $("#search").textbox("getValue");
                var productName = $("#productName").html();
                if (target == "发货类型") {
                    data = { act: "findDeliverType", q: q };
                } else if (target == "知悉人") {
                    data = { act: "findInformer", q: q };
                } else if (target == "发货单位") {
                    data = { act: "findHospitalName", q: q };
                } else if (target == "代理商") {
                    data = { act: "findAgentName", q: q };
                } else if (target == "品种") {
                    data = { act: "findProductName", q: q };
                } else if (productName == "请选择") {
                    $.messager.alert('提示', '未输入品种，无法选择规格和单位！', 'info');
                } else if (target == "规格") {
                    data = { act: "findSpec", q: productName };
                    act = "findSpec";
                } else if (target == "单位") {
                    data = { act: "findUnit", q: productName };
                }
                findName(data);
            }
        }]
    })

    var confirmInformer = function (data) {
        $("#informer").html(chooseInformer);

        $.mobile.back();
    }

    var findName = function (data) {
        Loading(true);

        $.ajax({
            url: 'mDeliverApplyReport.aspx',
            data: data,
            type: 'post',
            dataType: 'json',
            success: function (msg) {
                Loading(false)
                if (data.act == "findInformer") {
                    $("#detailList").empty();
                    chooseInformerId = new Array();
                    chooseInformer = new Array();
                    $.each($("#footer2").children("a"), function (i, v) {
                        if (i != 0) {
                            $(v).remove();
                        }
                    })
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
                                $.each($("#footer2").children("a"), function (i, v) {
                                    if (i != 0 && ($(v).text().replace(/^\s+|\s+$/g, "") == row.item || $(v).text() == row.item)) {
                                        $(v).remove();
                                        return;
                                    }
                                })
                            } else {
                                chooseInformer.push(row.item);
                                chooseInformerId.push(row.userId);
                                var html = '<a javascript:void(0)" onclick="removeInformer(this,\'' + row.item + '\',\'' + row.userId + '\',\'' + index + '\')" class="easyui-linkbutton" data-options="iconCls:\'icon-no\',iconAlign:\'right\',size:\'small\'">';
                                html += row.item;
                                html += "</a >";
                                $("#informerFooter").after(html);
                                $.parser.parse($("#informerFooter").parent());
                            }
                        }
                    })
                } else {
                    $("#detailList").empty();
                    $("#dl").datalist('loadData', { total: 0, rows: [] })
                    if (msg == "" || msg == null) {
                        $.messager.alert('提示', '暂无数据，请重新输入', 'info')
                    } else {
                        var html = "";
                        for (i = 0; i < msg.length; i++) {
                            html += '<li><a href="javascript:void(0)" onclick="setValue(\'' + data.act + '\',\'' + msg[i].target + '\')">' + msg[i].target + '</a></li>';
                        }
                        $("#detailList").append(html);
                    }
                }
            }
        })
    }

    function removeInformer(a, v, id, index) {
        $(a).remove();
        $('#dl').datalist('unselectRow', index);
        chooseInformer.splice(indexOf(chooseInformer, v), 1);
        chooseInformerId.splice(indexOf(chooseInformerId, id), 1);
    }

    var setValue = function (act, data) {
        if (act == "findProductName") {
            $("#productName").html(data);
        } else if (act == "findHospitalName") {
            $("#hospitalName").html(data);
        } else if (act == "findSpec") {
            $("#spec").html(data);
        } else if (act == "findUnit") {

            $("#unit").html(data);
        } else if (act == "findDeliverType") {
            $("#deliverType").html(data);
            //if (data == "赠品/样品") {
            //    UpdateApprover();
            //}
            //else {
            //    InitProcessInfo();
            //}
        } else if (act == "findInformer") {
            $("#informer").html(data);
        } else if (act == "findAgentName") {
            $("#agentName").html(data);
        }
        $.mobile.back();
        $("#search").textbox('clear');
    }

    function UpdateApprover() {
        $.post('mDeliverApplyReport.aspx', { act: 'updateApprover' },
            function (res) {
                if (data.ErrCode == 0) {
                    var data = JSON.parse(res);
                    approverIds = new Array();
                    $("#footer").empty();
                    var html = "";
                    for (var i = 0; i < data.approvers.length; i++) {
                        if (i > 0) {
                            html += '····';
                        }
                        html += '<a id="approver" href="#" class="easyui-linkbutton" data-options="iconAlign:\'top\',size:\'small\'">' + data.approvers[i].name + ' </a>';
                        approverIds.push(data.approvers[i].userId);
                    }
                    $("#footer").append(html);
                    $.parser.parse("#footer");
                }
            })
    }

    var submit = function () {
        var deliverType = $("#deliverType").html();
        var hospitalName = $("#hospitalName").html();
        var productName = $("#productName").html();
        var agentName = $("agentName").html();
        var spec = $("#spec").html();
        var unit = $("#unit").html();
        var informer = $("#informer").html();
        var applyNumber = $("#applyNumber").numberbox("getValue");
        var remark = $("#remark").textbox("getValue");
        var stock = $("#stock").numberbox("getValue");
        var netSales = $("#netSales").numberbox("getValue");
        var period = $("#period").numberbox("getValue");
        var isStockReceiptTogether = $("#isStockReceiptTogether").val();
        var deliverAddress = $("#deliverAddress").textbox("getValue");
        var deliverName = $("#deliverName").textbox("getValue");
        var deliverPhone = $("#deliverPhone").textbox("getValue");

        if (deliverType == "发公司直营网点")
            deliverType = "0";
        else if (deliverType == "发商业单位")
            deliverType = "1";
        else if (deliverType == "发代理商")
            deliverType = "2";
        else if (deliverType == "外购")
            deliverType = "3";
        else if (deliverType == "借货")
            deliverType = "4";
        else if (deliverType == "赠品/样品")
            deliverType = "5";

        if (hospitalName == "请选择" || productName == "请选择" || spec == "请选择" || unit == "请选择" ||
            applyNumber == "" || stock == "" || netSales == "" || period == "" || deliverType == "请选择" || deliverAddress == "" || deliverName == "" || deliverPhone == "") {
            $.messager.alert('提示', '存在空值！', 'info');
            return;
        }

        // 发货数量校验 若校验失败则第一级审批人为“王新”
        var criteria = (netSales / 30 * period) - stock;
        if (applyNumber > criteria) {
            $.messager.alert('提示', '发货数量超出标准值，第一级审批人调整为王新', 'info', function () {
                Loading(true);
                $.ajax({
                    url: 'mDeliverApplyReport.aspx',
                    data: {
                        act: 'insertDeliverApplyReport', approverIds: JSON.stringify(approverIds), hospitalName: hospitalName,
                        productName: productName, spec: spec, unit: unit, informer: informer, applyNumber: applyNumber, agentName: agentName,
                        remark: remark, deliverType: deliverType, chooseInformerId: JSON.stringify(chooseInformerId),
                        uploadFileUrls: JSON.stringify(uploadFileUrls), stock: stock, netSales: netSales, period: period, isStockReceiptTogether: isStockReceiptTogether,
                        deliverAddress: deliverAddress, deliverName:deliverName, deliverPhone:deliverPhone
                    },
                    type: 'post',
                    dataType: 'json',
                    success: function (msg) {
                        Loading(false);
                        $.messager.alert('提示', '保存成功', 'info', function () { location.href = 'http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=0' });
                    },
                    error: function (msg) {
                        Loading(false);
                        $.messager.alert('提示', '保存失败', 'info', function () { $.mobile.back() });
                        $.mobile.back();
                    }
                })
            });
        } else {
            Loading(true);
            $.ajax({
                url: 'mDeliverApplyReport.aspx',
                data: {
                    act: 'insertDeliverApplyReport', approverIds: JSON.stringify(approverIds), hospitalName: hospitalName,
                    productName: productName, spec: spec, unit: unit, informer: informer, applyNumber: applyNumber, agentName: agentName,
                    remark: remark, deliverType: deliverType, chooseInformerId: JSON.stringify(chooseInformerId),
                    uploadFileUrls: JSON.stringify(uploadFileUrls), stock: stock, netSales: netSales, period: period,
                },
                type: 'post',
                dataType: 'json',
                success: function (msg) {
                    Loading(false);
                    $.messager.alert('提示', '保存成功', 'info', function () { location.href = 'http://yelioa.top//mDeliverApplyReportAppRoval.aspx?type=0' });
                },
                error: function (msg) {
                    Loading(false);
                    $.messager.alert('提示', '保存失败', 'info', function () { $.mobile.back() });
                    $.mobile.back();
                }
            })
        }
    }
    var approverIds = new Array();
    var InitProcessInfo = function () {
        Loading(true);
        // 加载流程信息
        $.post('mDeliverApplyReport.aspx', { act: 'getProcessInfo' }, function (res) {
            res = JSON.parse(res);
            $("#footer").empty();
            var html = "";
            for (var i = 0; i < res.length; i++) {
                if (i > 0) {
                    html += '····';
                }
                html += '<a id="approver" href="#" class="easyui-linkbutton" data-options="iconAlign:\'top\',size:\'small\'">' + res[i].name + ' </a>';
                approverIds.push(res[i].userId);
            }
            $("#footer").append(html);
            $.parser.parse("#footer");
        });

        Loading(false);
    }
</script>
</html>
