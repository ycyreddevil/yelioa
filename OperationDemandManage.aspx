<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OperationDemandManage.aspx.cs" Inherits="OperationDemandManage" %>

<!DOCTYPE html>



<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>货需申请运营部审批数据</title>
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
        提交日期:
        <input id="dateFrom" class="easyui-datebox" style="width: 110px">
        -
        <input id="dateTo" class="easyui-datebox" style="width: 110px">
        提交人:
        <input id="applyName" class="easyui-textbox" style="width: 110px"/>
        收货单位:
        <input id="hospital" class="easyui-textbox" style="width: 110px"/>
        产品:
        <input id="product" class="easyui-textbox" style="width: 110px"/>
        审核状态:
        <select id="isChecked" class="easyui-combobox" style="width: 110px"data-options="panelHeight:'auto',editable:false" >
            <option value="1" selected="selected">审批中</option>
            <option value="2">已审批</option>
            <option value="3">全部</option>
        </select>
        
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search'," onclick="dg_search();">查询</a>&nbsp;
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="ExportFile();">导出Excel</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="updateActualFee();">实发数量复审</a>
         <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="reject();">拒绝</a>

        <div id="dlg-UpateActualFee" class="easyui-dialog" title="实发数量复审" data-options="iconCls:'icon-leaveStock',modal:true,closed:true,width:400,height:145">
        <div style="margin-top:5%">
			批准数量:<input id="actualFee" class="easyui-numberbox" data-options="multiline:true,prompt:'请输入批准数量...'" style="width:80%">
		</div>
        <a style="margin-top:5%;margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="certain();">确定</a>
        <a style="margin-top:5%"href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="$('#dlg-UpateActualFee').dialog('close');">取消</a>
    </div>

        <div id="dlg-reject" class="easyui-dialog" title="运营人员审批" data-options="iconCls:'icon-leaveStock',modal:true,closed:true,width:400,height:145">
        <div style="margin-top:5%">
			拒绝原因:<input id="rejectReason" class="easyui-textbox" data-options="multiline:true,prompt:'请输入拒绝原因...'" style="width:80%">
		</div>
        <a style="margin-top:5%;margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="confirmReject();">确定</a>
        <a style="margin-top:5%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="$('#dlg-reject').dialog('close');">取消</a>
    </div>

    </div>
</body>
<script>

    $(function () {

        initDatagrid();
    })

    function dg_search() {
        dg_load();
    }

    function updateActualFee() {
        $('#dlg-UpateActualFee').dialog("open");
        $("#actualFee").numberbox("setValue", "");
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
            $.post("OperationDemandManage.aspx",
                { action: "reject", opinion: opinion, Ids: JSON.stringify(Ids) },
                function (res) {
                    if (res != "") {
                        $.messager.alert('提示', '保存成功', 'info');
                        parent.Loading(false);
                        initDatagrid();
                        $('#dlg-reject').dialog('close');
                    }
                    else {
                        $.messager.alert('提示', '填写失败', 'info');
                        parent.Loading(false);
                    }
                });
        }
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
                $.messager.alert('提示', '存在批准数量大于申请数量的单据，无法进行复审', 'info');
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
                        url: 'OperationDemandManage.aspx',
                        data: {
                            action: 'updateActualFee',
                            codes: JSON.stringify(Ids),
                            actual_fee_amount: JSON.stringify(ActualNumbers),
                            submitters: JSON.stringify(submitter)
                        },
                        type: 'post',
                        dataType: 'json',
                        success: function (data) {
                            $.messager.alert('提示', '批准数量复审成功', 'info');
                            $('#dlg-UpateActualFee').dialog("close", true);

                            initDatagrid();
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

    function DatagridClear(dgId) {
        $('#' + dgId).datagrid('loadData', { total: 0, rows: [] });
    }

    function ExportFile()
    {
        ExportToExcel('货需申请单据信息.xlsx','货需申请单据信息','dg');
       
    }


    function dg_load(){
        var starttm = $("#dateFrom").datebox('getValue') == "" ? "" : $("#dateFrom").datebox('getValue') + " 00:00:00";
        var endtm = $("#dateTo").datebox('getValue') == "" ? "" : $("#dateTo").datebox('getValue') + " 23:59:59";
        var apply_name = $("#applyName").textbox('getValue');
        var hospital = $("#hospital").textbox('getValue');
        var product = $("#product").textbox('getValue');
        var isChecked = $('#isChecked').combobox('getValue');
        parent.Loading(true);
        $.post( 'OperationDemandManage.aspx',
             {
                action: 'getData',
                starttm: starttm,
                endtm: endtm,
                apply_name: apply_name,
                hospital: hospital,
                product: product,
                isChecked: isChecked
            }, function (res) {
                 var data = JSON.parse(res);
                 parent.Loading(false);
                 if (data.ErrCode == 0) {
                     var document = JSON.parse(data.Document);
                     $('#dg').datagrid('loadData',document);
                 }
                 else  {
                      $.messager.alert('提示', data.ErrMsg, 'info');
                 }
            })
    }
   

   

    var initDatagrid = function () {
        
        $('#dg').datagrid({
            
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
                   field: 'ApproveTime', width: 20, align: 'center', title: '审批时间'                   
                },
 
                {
                   field: 'Status', width: 20, align: 'center', title: '审批状态'                   
                },
                 {
                   field: 'LMT', width: 20, align: 'center', title: '提交时间'                   
                },
                { field: 'UserName', width: 10, align: 'center', title: '提交人' },
                { field: 'hospitalName', width: 20, align: 'center', title: '医院' },
                { field: 'agentName', width: 15, align: 'center', title: '代理商名称' },
                {
                    field: 'productName', width: 20, align: 'center', title: '产品',                 
                },
                {
                    field: 'Specification', width: 20, align: 'center', title: '规格',                 
                },
                {
                    field: 'Unit', width: 20, align: 'center', title: '型号',                 
                },
                
                {
                    field: 'DeliverNumber', width: 20, align: 'center', title: '货需申请数量',
                  
                },
                { field: 'ApprovalNumber', width: 20, align: 'center', title: '批准数量' },

                { field: 'Stock', width: 20, align: 'center', title: '本月终端库存' },
               
                { field: 'netSales', width:20, align: 'center', title: '本月纯销' },
                { field: 'Remark', width: 20, align: 'center', title: '备注' },
                { field: 'Opinion', width: 20, align: 'center', title: '意见/原因' },

                { field: 'OperationApprovalTime', width: 20, align: 'center', title: '运营部审批时间' },
               
            ]],
            
        });

        
    }
</script>
</html>

