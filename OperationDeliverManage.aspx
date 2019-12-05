<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OperationDeliverManage.aspx.cs" Inherits="OperationDeliverManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>发货申请运营部审批数据</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/pcCommon.js"></script>
    <script src="Scripts/base-loading.js"></script>
</head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
                    background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog"
        border="false" noheader="true" closed="true" modal="true">
    </div>
    <table id="dg" class="easyui-datagrid" title="发货申请运营部审批数据"><thead data-options="frozen:true"></thead>
    </table>
    <div id="tb" style="padding: 2px 5px;">
        运营审批日期:
        <input id="dateFrom" class="easyui-datebox" style="width: 110px">
        -
        <input id="dateTo" class="easyui-datebox" style="width: 110px">
        提交人:
        <input id="applyName" class="easyui-textbox" style="width: 110px"/>
        医院:
        <input id="hospital" class="easyui-textbox" style="width: 110px"/>
        产品:
        <input id="product" class="easyui-textbox" style="width: 110px"/>
        审核状态:
        <select id="isChecked" class="easyui-combobox" style="width: 110px"data-options="panelHeight:'auto',editable:false" >
            <option value="1" selected="selected">运营部未审核</option>
            <option value="2">运营部已审核</option>
            <option value="3">全部</option>
        </select>
        
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search'," onclick="dg_search();">查询</a>&nbsp;    
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="updateActualFee();">实发数量复审</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-cancel'" onclick="reject();">拒绝</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="updateReason();">未发货原因填写</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-leaveStock'" onclick="ExportFile();">导出Excel</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="updateDeliverCode();">填写快递单号</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="updateReceiptCode();">填写发票单号</a>

        <%--<a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick=" $('#dlg-addBranch').dialog('open');">网点增加</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'" onclick=" $('#dlg-addProduct').dialog('open');">产品增加</a>--%>
    </div>
    <div id="dlg-Reason" class="easyui-dialog" title="运营人员审批" data-options="iconCls:'icon-leaveStock',modal:true,closed:true,width:400,height:145">
        <div style="margin-top:5%">
			未发货原因:<input id="undeliverreason" class="easyui-textbox" data-options="multiline:true,prompt:'请输入未发货原因...'" style="width:80%">
		</div>
        <a style="margin-top:5%;margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="confirm();">确定</a>
        <a style="margin-top:5%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="closeDialog(dlg-Reason);">取消</a>
    </div>
    <div id="dlg-reject" class="easyui-dialog" title="运营人员审批" data-options="iconCls:'icon-leaveStock',modal:true,closed:true,width:400,height:145">
        <div style="margin-top:5%">
			拒绝原因:<input id="rejectReason" class="easyui-textbox" data-options="multiline:true,prompt:'请输入拒绝原因...'" style="width:80%">
		</div>
        <a style="margin-top:5%;margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="confirmReject();">确定</a>
        <a style="margin-top:5%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="$('#dlg-reject').dialog('close');">取消</a>
    </div>
    <div id="dlg-UpateActualFee" class="easyui-dialog" title="实发数量复审" data-options="iconCls:'icon-leaveStock',modal:true,closed:true,width:400,height:145">
        <div style="margin-top:5%">
			实发数量:<input id="actualFee" class="easyui-numberbox" data-options="multiline:true,prompt:'请输入实发数量...'" style="width:80%">
		</div>
        <a style="margin-top:5%;margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="certain();">确定</a>
        <a style="margin-top:5%"href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="closeDialog(dlg-UpateActualFee);">取消</a>
<%--        <a style="margin-top:5%"href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-back" onclick="quickCertain();">一键审批</a>--%>
    </div>
    <%--<div id="dlg-addBranch" class="easyui-dialog" title="增加网点" data-options="iconCls:'icon-add',modal:true,closed:true,width:400,height:200">
        <div style="margin-top:5%">
			网点名称:<input id="branchName" class="easyui-textbox" data-options="prompt:'请输入网点名称...'" style="width:80%">
		</div>
         <div style="margin-top:5%">
			网点编号:<input id="branchCode" class="easyui-textbox" data-options="prompt:'请输入网点编号...'" style="width:80%">
		</div>
        <a style="margin-top:5%;margin-left:35%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="AddBranch();">确定</a>
        <a style="margin-top:5%"href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="$('#dlg-addBranch').dialog('close');">取消</a>
    </div>
    <div id="dlg-addProduct" class="easyui-dialog" title="增加产品" data-options="iconCls:'icon-add',modal:true,closed:true,width:450,height:280">
        <div style="margin-top:5%">
            产品名称:<input id="productName" class="easyui-textbox" data-options="prompt:'请输入产品名称...'" style="width:80%">
        </div>
        <div style="margin-top:5%">
            产品编号:<input id="productCode" class="easyui-textbox" data-options="prompt:'请输入产品编号...'" style="width:80%">
        </div>
        <div style="margin-top:5%">
            产品规格:<input id="productSpec" class="easyui-textbox" data-options="prompt:'请输入产品规格...'" style="width:80%">
        </div>
        <div style="margin-top:5%">
            产品单位:<input id="productUnit" class="easyui-textbox" data-options="prompt:'请输入产品单位...'" style="width:80%">
        </div>
        <a style="margin-top:5%;margin-left:35%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="AddProduct();">确定</a>
        <a style="margin-top:5%"href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="$('#dlg-addProduct').dialog('close');">取消</a>
    </div>--%>
    <div id="dlg-deliverCode" class="easyui-dialog" title="快递单号填写" data-options="iconCls:'icon-leaveStock',modal:true,closed:true,width:400,height:145">
        <div style="margin-top:5%">
			快递单号:<input id="deliverCode" class="easyui-textbox" data-options="multiline:true,prompt:'请输入快递单号...'" style="width:80%" />
		</div>
        <a style="margin-top:5%;margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="confirmDeliverCode();">确定</a>
        <a style="margin-top:5%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="closeDialog(dlg-deliverCode);">取消</a>
    </div>
    <div id="dlg-receiptCode" class="easyui-dialog" title="发票单号填写" data-options="iconCls:'icon-leaveStock',modal:true,closed:true,width:400,height:145">
        <div style="margin-top:5%">
			发票单号:<input id="receiptCode" class="easyui-textbox" data-options="multiline:true,prompt:'请输入发票单号...'" style="width:80%" />
		</div>
        <a style="margin-top:5%;margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="confirmReceiptCode();">确定</a>
        <a style="margin-top:5%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="closeDialog(dlg-receiptCode);">取消</a>
    </div>
</body>
<script>
    var Ids = new Array();
    var DeliverNumbers = new Array();
    var account_approval_results = new Array();
    var submitter = new Array();

    $(function () {
        //var d = new Date();
        //var firstDate = d.getFullYear() + "-" + (d.getMonth()+1) + "-1";
        //var todayDate = d.getFullYear() + "-" + (d.getMonth()+1) + "-" + d.getDate();

        //$("#dateFrom").datebox('setValue', firstDate);
        //$("#dateTo").datebox('setValue', todayDate);

        initDatagrid();
    })

    function updateActualFee() {
        $('#dlg-UpateActualFee').dialog("open");
        $("#actualFee").numberbox("setValue", "");
    }

    function certain() {
        Ids = new Array();
        ActualNumbers = new Array();
        submitter = new Array();
        $('#dlg-UpateActualFee').dialog("close", true);  
        var data = $("#dg").datagrid("getChecked");
        var actualFee = parseInt($("#actualFee").numberbox("getValue"));
        if (data.length == 0) {
            $.messager.alert('提示', '请选择要操作的单据', 'info');
        } else {
            var flag = true;
            $.each(data, function (i) {
                var id = data[i].Id;
                Ids.push(id);

                var deliverNumber = parseInt(data[i].DeliverNumber);
                var approvalNumber = parseInt(data[i].ApprovalNumber);
                ActualNumbers.push(actualFee + approvalNumber);

                submitter.push(data[i].UserName);

                if (actualFee + approvalNumber > deliverNumber) {
                    flag = false;
                }
            });

            if (!flag) {
                $.messager.alert('提示', '存在实发数量大于申请数量的单据，无法进行复审', 'info');
                return;
            }
            
            var msg = "确定要复审";
            $.each(Ids, function (i) {
                msg += "<br/>单据号：" + Ids[i] + ","
            })
            msg = msg.substring(0, msg.length - 1);
            msg += "的单据吗?"

            $.messager.confirm('确认', msg, function (r) {
                if (r) {
                    $.ajax({
                        url: 'OperationDeliverManage.aspx',
                        data: {
                            action: 'updateActualFee',
                            codes: JSON.stringify(Ids),
                            actual_fee_amount: JSON.stringify(ActualNumbers),
                            submitters: JSON.stringify(submitter)
                        },
                        type: 'post',
                        dataType: 'json',
                        success: function (data) {
                            $.messager.alert('提示', '实发数量复审成功', 'info');
                            $('#dlg-UpateActualFee').dialog("close", true);

                            $("#dg").datagrid('reload');
                            $('#dg').datagrid('unselectAll');
                        },
                        error: function (data) {
                            $.messager.alert('提示', '审批失败', 'info');
                            $('#dlg-UpateActualFee').dialog("close", true);
                        }
                    });
                }
            })
        }
    }

    function dg_search() {
        initDatagrid();
    }

    function closeDialog(dId) {
        $('#'+dId).dialog("close", true);  
    }

    function confirm() {
        Ids = new Array();

        var reason = $("#undeliverreason").textbox("getValue");

        var data = $("#dg").datagrid("getChecked");
        if (data.length == 0) {
            $.messager.alert('提示', '请选择要操作的单据', 'info');
        } else if (reason == "") {
            $.messager.alert('提示', '请选择审批结果', 'info');
        }else {  
            $.each(data, function (i) {
                var Id = data[i].Id;
                Ids.push(Id);
            });
            $('#dlg-Reason').dialog("close", true);
            parent.Loading(true);
            $.ajax({
                url: 'OperationDeliverManage.aspx',
                data: {
                    action: 'updateReason',
                    codes: JSON.stringify(Ids),
                    reason:reason
                },
                type: 'post',
                dataType: 'json',
                success: function (data) {
                    $.messager.alert('提示', '保存成功', 'info');
                    parent.Loading(false);
                    $("#dg").datagrid('reload');
                    $('#dg').datagrid('unselectAll');
                },
                error: function (data) {
                    $.messager.alert('提示', '填写失败', 'info');
                    parent.Loading(false);
                }
            })
        }
    }

    function confirmDeliverCode() {
        Ids = new Array();

        var deliverCode = $("#deliverCode").textbox("getValue");

        var data = $("#dg").datagrid("getChecked");
        if (data.length == 0) {
            $.messager.alert('提示', '请选择要操作的单据', 'info');
        } else if (deliverCode == "") {
            $.messager.alert('提示', '请填写快递单号', 'info');
        } else {
            $.each(data, function (i) {
                var Id = data[i].Id;
                Ids.push(Id);
            });
            $('#dlg-deliverCode').dialog("close", true);
            parent.Loading(true);
            $.ajax({
                url: 'OperationDeliverManage.aspx',
                data: {
                    action: 'updateDeliverCode',
                    codes: JSON.stringify(Ids),
                    deliverCode: deliverCode
                },
                type: 'post',
                dataType: 'json',
                success: function (data) {
                    $.messager.alert('提示', '保存成功', 'info');
                    parent.Loading(false);
                    $("#dg").datagrid('reload');
                    $('#dg').datagrid('unselectAll');
                },
                error: function (data) {
                    $.messager.alert('提示', '填写失败', 'info');
                    parent.Loading(false);
                }
            })
        }
    }

    function confirmReceiptCode() {
        Ids = new Array();

        var receiptCode = $("#receiptCode").textbox("getValue");

        var data = $("#dg").datagrid("getChecked");
        if (data.length == 0) {
            $.messager.alert('提示', '请选择要操作的单据', 'info');
        } else if (receiptCode == "") {
            $.messager.alert('提示', '请填写发票单号', 'info');
        } else {
            $.each(data, function (i) {
                var Id = data[i].Id;
                Ids.push(Id);
            });
            $('#dlg-receiptCode').dialog("close", true);
            parent.Loading(true);
            $.ajax({
                url: 'OperationDeliverManage.aspx',
                data: {
                    action: 'updateReceiptCode',
                    codes: JSON.stringify(Ids),
                    receiptCode: receiptCode
                },
                type: 'post',
                dataType: 'json',
                success: function (data) {
                    $.messager.alert('提示', '保存成功', 'info');
                    parent.Loading(false);
                    $("#dg").datagrid('reload');
                    $('#dg').datagrid('unselectAll');
                },
                error: function (data) {
                    $.messager.alert('提示', '填写失败', 'info');
                    parent.Loading(false);
                }
            })
        }
    }

    function updateReason() {
        $('#dlg-Reason').dialog('open');
    }

    function updateDeliverCode() {
        $('#dlg-deliverCode').dialog('open');
    }

    function updateReceiptCode() {
        $('#dlg-receiptCode').dialog('open');
    }

    function DatagridClear(dgId) {
        $('#' + dgId).datagrid('loadData', { total: 0, rows: [] });
    }

    function ExportFile()
    {
        ExportToExcel('发货申请单据信息.xlsx','发货申请单据信息','dg');
    }
    function reject() {
        $('#dlg-reject').dialog('open');
    }
    function confirmReject() {
        Ids = new Array();
        var opinion = $('#rejectReason').textbox('getValue');
        var data = $("#dg").datagrid("getChecked");
        if (data.length == 0) {
            $.messager.alert('提示', '请选择要操作的单据', 'info');
        } else if (opinion == "") {
            $.messager.alert('提示', '请填写原因', 'info');
        } else {
            $.each(data, function (i) {
                var Id = data[i].Id;
                Ids.push(Id);
            });
            $('#dlg-reject').dialog("close", true);
            parent.Loading(true);
            $.post("OperationDeliverManage.aspx",
                { action: "reject", opinion: opinion, Ids: JSON.stringify(Ids) },
                function (res) {
                    if (res != "") {
                        $.messager.alert('提示', '保存成功', 'info');
                        parent.Loading(false);
                        $("#dg").datagrid('reload');
                        $('#dg').datagrid('unselectAll');
                        $('#dlg-reject').dialog('close');
                    }
                    else {
                        $.messager.alert('提示', '填写失败', 'info');
                        parent.Loading(false);
                    }
                });
        }
    }

    function AddBranch() {
        var branchName = $('#branchName').textbox('getValue');
        var branchCode = $('#branchCode').textbox('getValue');
        if (branchCode == "" || branchCode == null || branchName == "" || branchName == null) {
            $.messager.alert('提示', '存在空值', 'info');
            return;
        }
        else {
            $.post("OperationDeliverManage.aspx",
                { action: "AddBranch", branchName: branchName, branchCode: branchCode },
                function(res) {
                    if (res == "操作成功") {
                        $('#dlg-addBranch').dialog('close');
                    }
                    $.messager.alert('提示', res, 'info');
                });
        }
    }

    function AddProduct() {
        var productName = $('#productName').textbox('getValue');
        var productCode = $('#productCode').textbox('getValue');
        var productSpec = $('#productSpec').textbox('getValue');
        var productUnit = $('#productUnit').textbox('getValue');
        if (productName == "" || productName == null || productCode == "" || productCode == null ||
            productSpec == "" || productSpec == null || productUnit == "" || productUnit == null) {
            $.messager.alert('提示', '存在空值', 'info');
            return;
        }
        else {
            $.post("OperationDeliverManage.aspx",
                { action: "AddProduct", productName: productName, productCode: productCode, productSpec: productSpec, productUnit: productUnit },
                function(res) {
                    if (res == "操作成功") {
                        $('#dlg-addProduct').dialog('close');
                    }
                    $.messager.alert('提示', res, 'info');
                });
        }
    }

    var initDatagrid = function () {
        var starttm = $("#dateFrom").datebox('getValue') == "" ? "" : $("#dateFrom").datebox('getValue') + " 00:00:00";
        var endtm = $("#dateTo").datebox('getValue') == "" ? "" : $("#dateTo").datebox('getValue') + " 23:59:59";
        var apply_name = $("#applyName").textbox('getValue');
        var hospital = $("#hospital").textbox('getValue');
        var product = $("#product").textbox('getValue');
        var isChecked = $('#isChecked').combobox('getValue');
        $('#dg').datagrid({
            url: 'OperationDeliverManage.aspx',
            queryParams: {
                action: 'getData',
                starttm: starttm,
                endtm: endtm,
                apply_name: apply_name,
                hospital: hospital,
                product: product,
                isChecked: isChecked
            },
            nowrap: false,
            singleSelect: false,
            fit: true,
            toolbar: '#tb',
            striped: true,
            rownumbers: true,
            collapsible: false,
            showFooter: true,
            fitColumns: true,
            columns: [[
                { field: 'code11', checkbox: true, width: 12, align: 'center', title: '复选框', sortable: "true", },
                { field: 'Id', width: 5, align: 'center', title: '单据编号', sortable: "true", },
                {
                   field: 'ApproveTime', width: 10, align: 'center', title: '审批时间'                   
                },
                {
                   field: 'OperationApprovalTime', width: 10, align: 'center', title: '运营审批时间'                   
                },
                //{ field: 'productCode', width: 10, align: 'center', title: '产品编码', sortable: "true" },
                { field: 'UserName', width: 8, align: 'center', title: '提交人' },
                { field: 'department', width: 20, align: 'center', title: '提交人部门' },
                {
                    field: 'DeliverStyle', width: 8, align: 'center', title: '发货方式',
                },
                // { field: 'deliveryWarehouse', width: 10, align: 'center', title: '发货仓库' },
                { field: 'hospitalName', width: 15, align: 'center', title: '医院' },
                {
                    field: 'productName', width: 15, align: 'center', title: '产品',
                    //formatter: function (data) {
                    //    return data.replace("[", "").replace("]", "").replace(/"/g, "");
                    //}
                },
                { field: 'agentName', width: 15, align: 'center', title: '代理商名称' },
                { field: 'spec', width:5, align: 'center', title: '规格' },
                { field: 'unit', width: 5, align: 'center', title: '单位' },
                {
                    field: 'DeliverNumber', width: 6, align: 'center', title: '申请数量',
                    //formatter: function (data) {
                    //    return data.replace("[", "").replace("]", "").replace(/"/g, "");
                    //}
                },
                { field: 'ApprovalNumber', width:6, align: 'center', title: '实发数量' },
                { field: 'Remark', width: 20, align: 'center', title: '备注' },
                { field: 'Reason', width: 20, align: 'center', title: '未发货原因' },
                { field: 'Opinion', width: 20, align: 'center', title: '原因/意见' },
                { field: 'IsStockReceiptTogether', width: 20, align: 'center', title: '是否货票同行' },
                { field: 'DeliverAddress', width: 20, align: 'center', title: '收货地址' },
                { field: 'DeliverName', width: 20, align: 'center', title: '联系人' },
                { field: 'DeliverPhone', width: 20, align: 'center', title: '联系人电话' },
            ]],
            idField: "Id",
            sortName: 'date',
            sortOrder: 'asc',
            onSortColumn: function (sort, order) {
                //dg_Load("", sort, order);
            }
        })
    }
</script>
</html>
