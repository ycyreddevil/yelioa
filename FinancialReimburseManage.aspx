<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FinancialReimburseManage.aspx.cs" Inherits="FinancialReimburseManage" %>

    <!DOCTYPE html>

    <html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title>移动报销财务审批数据</title>
        <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
        <meta name="description" content="" />
        <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
        <link href="Scripts/themes/icon.css" rel="stylesheet" />
        <link href="Scripts/themes/color.css" rel="stylesheet" />
        <script src="Scripts/jquery.min.js"></script>
        <script src="Scripts/pcCommon.js"></script>
        <script src="Scripts/jquery.easyui.min.js"></script>
        <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
        <script src="Scripts/base-loading.js"></script>
    </head>

    <body>
        <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
                    background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog"
            border="false" noheader="true" closed="true" modal="true">
        </div>
        <table id="dg" class="easyui-datagrid" title="移动报销财务审批查询">
            <thead data-options="frozen:true"></thead>
        </table>
        <div id="tb" style="padding: 2px 5px;">
            审批日期:
            <input id="dateFrom" class="easyui-datebox" style="width: 110px"> -
            <input id="dateTo" class="easyui-datebox" style="width: 110px">提交日期:
             <input id="applyDateFrom" class="easyui-datebox" style="width: 110px"> -
            <input id="applyDateTo" class="easyui-datebox" style="width: 110px">提交人:
            <input id="applyName" class="easyui-textbox" style="width: 110px" /> 部门:
            <input id="depart" class="easyui-textbox" style="width: 110px" /> 费用归属部门:
            <input id="fee_depart" class="easyui-textbox" style="width: 110px" /> 费用明细:
            <input id="fee_detail" class="easyui-textbox" style="width: 110px" /> 事业部审批结果:
            <select id="status" class="easyui-combobox" style="width:110px;" data-options="editable:false,panelHeight:'auto'">
                <option>全部</option>
                <option>审批中</option>
                <option>同意</option>
                <option>不同意</option>
            </select>
            财务审批结果:
            <select id="account_status" class="easyui-combobox" style="width:110px;" data-options="editable:false,panelHeight:'auto'">
                <option>全部</option>
                <option>未审批</option>
                <option>同意</option>
                <option>拒绝</option>
            </select>
            <%--<a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-leaveStock" onclick="OpenDialoge();">导出移动报销单据</a>--%>
                <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search'," onclick="dg_search();">查询</a>&nbsp;
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-leaveStock'" onclick="batchApproval();">审批</a>
                <%--<a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'" onclick="updateActualFee();">金额复审</a>--%>
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-print'" onclick="exportExcel();">导出</a>
                <%--<a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-import'" onclick="$('#dlg-budget-Import').dialog('open');">预算导入</a>--%>
                <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-import',closed:true"
                   onclick="$('#dlg-Import').dialog('open'); $('#fmFile').form('clear');">实报金额导入</a>
        </div>
        <div id="dlg-Approval" class="easyui-dialog" title="财务人员审批" data-options="iconCls:'icon-leaveStock',modal:true,closed:true,width:400,height:230">
            <div style="margin-top:5%;margin-left:5%">
                <span id="selectStatistique"></span>
            </div>
            <div style="margin-top:5%; margin-bottom:5%;margin-left:5%">
                审批意见:
                <input id="approvalOption" class="easyui-textbox" data-options="multiline:true,prompt:'请输入审批意见...'"
                    style="width:80%">
            </div>
            <div style="margin-top:5%; margin-bottom:5%;margin-left:5%">
                审批结果:
                <input type="radio" id="agree" name="approvalResult" />同意
                <input type="radio" id="decline" name="approvalResult" />拒绝
            </div>
            <a style="margin-left:33%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="confirm();">确定</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="closeDialog();">取消</a>
        </div>
        <div id="dlg-UpateActualFee" class="easyui-dialog" title="金额复审" data-options="iconCls:'icon-leaveStock',modal:true,closed:true,width:400,height:145">
            <div style="margin-top:5%">
                实报金额:
                <input id="actualFee" class="easyui-numberbox" data-options="multiline:true,prompt:'请输入实报金额...'" style="width:80%">
            </div>
            <a style="margin-top:5%;margin-left:24%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-ok" onclick="certain();">确定</a>
            <a style="margin-top:5%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-no" onclick="closeDialog();">取消</a>
            <a style="margin-top:5%" href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-back" onclick="quickCertain();">一键审批</a>
        </div>
        <div id="dlg-Import" class="easyui-dialog" title="信息导入" style="width: 800px; height: 60px"
            data-options="iconCls:'icon-import',modal:true,closed:true">
            <form id="fmFile" method="post" enctype="multipart/form-data">
                <input id="fbx" class="easyui-filebox" label="人员信息文件:" labelposition="left"
                    data-options="onChange:function(){uploadFiles('upload');},prompt:'请选择一个xls文件...'"
                    style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				    <input type="hidden" name="action" id="actFbx" />
            </form>
        </div>
        <div id="dlg-budget-Import" class="easyui-dialog" title="预算导入" style="width: 800px; height: 60px"
            data-options="iconCls:'icon-import',modal:true,closed:true">
            <form id="fmFile1" method="post" enctype="multipart/form-data">
                <input id="fbx1" class="easyui-filebox" label="预算文件:" labelposition="left"
                    data-options="onChange:function(){uploadBudgetFiles('uploadBudget');},prompt:'请选择一个xls文件...'"
                    style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
				    <input type="hidden" name="action" id="actFbx1" />
            </form>
        </div>
        <div id="dlg-Detail" class="easyui-dialog" title="移动报销明细数据" style="width: 1200px; height: 600px"
            data-options="iconCls:'icon-import',modal:true,closed:true">
            <table id="dgDetail" class="easyui-datagrid">
                <thead data-options="frozen:true"></thead>
            </table>
        </div>
        <div id="bigImgDialog">
            <img id="bigImg" style="width:800px; height:600px" src=""/>
        </div>
    </body>
    <script>
        var codes = new Array();
        var names = new Array();
        var account_approval_results = new Array();
        var sortName='code';
        var sortOrder='asc';

        $(function () {
            var d = new Date();
            var firstDate = d.getFullYear() + "-" + (d.getMonth() + 1) + "-1";
            var todayDate = d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate();

            $("#dateFrom").datebox('setValue', firstDate);
            $("#dateTo").datebox('setValue', todayDate);
            $("#applyDateFrom").datebox('setValue', firstDate);
            $("#applyDateTo").datebox('setValue', todayDate);

            initDatagrid();
        })

        function updateActualFee() {
            $('#dlg-UpateActualFee').dialog("open");
            $("#actualFee").numberbox("setValue", "");
        }

        function quickCertain() {
            codes = new Array();
            names = new Array();
            var originFees = new Array();
            $('#dlg-UpateActualFee').dialog("close", true);
            var data = $("#dg").datagrid("getChecked");
            if (data.length == 0) {
                $.messager.alert('提示', '请选择要操作的单据', 'info');
            } else {
                var flag = true;
                $.each(data, function (i) {
                    var code = data[i].code;
                    codes.push(code);

                    var originFee = data[i].fee_amount;
                    originFees.push(originFee);

                    var name = data[i].name;
                    names.push(name);

                    if (data[i].ApprovalResult != '同意') {
                        flag = false;
                    }
                });

                if (!flag) {
                    $.messager.alert('提示', '存在财务未通过的单据，无法进行复审', 'info');
                    return;
                }

                $.messager.confirm('确认', '确定要执行一键审批的操作吗？', function (r) {
                    if (r) {
                        $('#dlg-UpateActualFee').dialog("close", true);
                        parent.Loading(true);
                        $.ajax({
                            url: 'FinancialReimburseManage.aspx',
                            data: {
                                action: 'updateActualFee',
                                codes: JSON.stringify(codes),
                                originFees: JSON.stringify(originFees),
                                names: JSON.stringify(names),
                            },
                            type: 'post',
                            dataType: 'json',
                            success: function (data) {
                                $.messager.alert('提示', '一键审批成功', 'info');
                                $("#dg").datagrid('reload');
                                $('#dg').datagrid('unselectAll');
                                parent.Loading(false);
                            },
                            error: function (data) {
                                $.messager.alert('提示', '审批失败', 'info');
                                parent.Loading(false);
                            }
                        });
                    }
                })
            }
        }

        function certain() {
            codes = new Array();
            names = new Array();
            $('#dlg-UpateActualFee').dialog("close", true);
            var data = $("#dg").datagrid("getChecked");
            if (data.length == 0) {
                $.messager.alert('提示', '请选择要操作的单据', 'info');
            } else {
                var flag = true;
                $.each(data, function (i) {
                    var code = data[i].code;
                    codes.push(code);

                    var name = data[i].name;
                    names.push(name);

                    if (data[i].account_result != '同意') {
                        flag = false;
                    }
                });

                if (!flag) {
                    $.messager.alert('提示', '存在财务未通过的单据，无法进行复审', 'info');
                    return;
                }

                var msg = "确定要复审";
                $.each(codes, function (i) {
                    msg += "<br/>单据号：" + codes[i] + ","
                })
                msg = msg.substring(0, msg.length - 1);
                msg += "的单据吗?"

                $.messager.confirm('确认', msg, function (r) {
                    if (r) {
                        $.ajax({
                            url: 'FinancialReimburseManage.aspx',
                            data: {
                                action: 'updateActualFee',
                                codes: JSON.stringify(codes),
                                actual_fee_amount: $("#actualFee").numberbox("getValue"),
                                names: JSON.stringify(names),
                            },
                            type: 'post',
                            dataType: 'json',
                            success: function (data) {
                                $.messager.alert('提示', '金额复审成功', 'info');
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

        function exportExcel() {
            ExportToExcel('报销单据信息', '报销单据信息', 'dg');
            //var rows = $("#dg").datagrid("getRows");

            //if (rows.length == 0) {
            //    $.messager.alert('提示', '暂无数据，无法导出', 'info');
            //    return;
            //}

            //var starttm = $("#dateFrom").datebox('getValue') + " 00:00:00";
            //var endtm = $("#dateTo").datebox('getValue') + " 23:59:59";
            //var apply_name = $("#applyName").textbox('getValue');
            //var depart = $("#depart").textbox('getValue');
            //var fee_depart = $("#fee_depart").textbox('getValue');
            //var fee_detail = $("#fee_detail").textbox('getValue');

            //window.location.href = "FinancialReimburseManage.aspx?action=exportExcel&&starttm=" + starttm + "&&endtm=" + endtm +
            //    "&&apply_name=" + apply_name + "&&depart=" + depart + "&&fee_depart=" + fee_depart + "&&fee_detail=" + fee_detail;

            //$.ajax({
            //    url: 'FinancialReimburseManage.aspx',
            //    data: {
            //        action: 'exportExcel',
            //        dataJson: JSON.stringify(rows)
            //    },
            //    dataType: 'json',
            //    type: 'post',
            //    success: function (msg) {
            //        console.log(msg)
            //    }
            //})
        }

        function dg_search() {
            $('#dg').datagrid('clearSelections');
            initDatagrid();
        }

        function closeDialog() {
            $('#dlg-Approval').dialog("close", true);
        }

        function confirm() {
            codes = new Array();
            names = new Array();
            account_approval_results = new Array();

            var approvalResult = "";
            //console.log($("#dg").datagrid("getChecked"));
            if ($("#agree").is(":checked") == true) {
                approvalResult = "同意";
            } else if ($("#decline").is(":checked") == true) {
                approvalResult = "拒绝";
            }

            var data = $("#dg").datagrid("getChecked");
            if (data.length == 0) {
                $.messager.alert('提示', '请选择要操作的单据', 'info');
            } else if (approvalResult == "") {
                $.messager.alert('提示', '请选择审批结果', 'info');
            } else {
                $.each(data, function (i) {
                    var code = data[i].code;
                    codes.push(code);

                    var name = data[i].name;
                    names.push(name);

                    var account_approval_result = data[i].account_result;
                    account_approval_results.push(account_approval_result);
                });
                $('#dlg-Approval').dialog("close", true);
                parent.Loading(true);
                $.ajax({
                    url: 'FinancialReimburseManage.aspx',
                    data: {
                        action: 'approval',
                        codes: JSON.stringify(codes),
                        approvalOption: $("#approvalOption").textbox('getValue'),
                        approvalResult: approvalResult,
                        names: JSON.stringify(names),
                        account_approval_results: JSON.stringify(account_approval_results),
                    },
                    type: 'post',
                    dataType: 'json',
                    success: function (data) {
                        if (data.msg == "所选单据都已审批，请重新选择") {
                            $.messager.alert('提示', data.msg, 'info');
                        } else {
                            $.messager.alert('提示', '审批成功', 'info');
                        }
                        parent.Loading(false);
                        $("#dg").datagrid('reload');
                        $('#dg').datagrid('unselectAll');
                    },
                    error: function (data) {
                        $.messager.alert('提示', '审批失败', 'info');
                        parent.Loading(false);
                    }
                })
            }
        }

        function batchApproval() {
            //DatagridClear('dlg-Approval');
            // 清空数据
            $("#approvalOption").textbox("setValue", "");
            $("#agree").attr("checked", false);
            $("#decline").attr("checked", false);

            $('#dlg-Approval').dialog('open');
            var rows = $("#dg").datagrid('getSelections');
            var moneySum = 0;
            $.each(rows,function(index,val){
                try {
                    var money = Number(val.fee_amount);
                    if(money != NaN)
                        moneySum += money;
                } catch (error) {
                    
                }                
            });
            $('#selectStatistique').html("已选择"+rows.length+"条数据，金额合计："+moneySum);
        }

        function DatagridClear(dgId) {
            $('#' + dgId).datagrid('loadData', { total: 0, rows: [] });
        }

       function uploadFiles(type) {
            var fileName = $('#fbx').filebox('getValue');
            if (fileName == "") {
                return;
            }
            if (fileName.indexOf(".xls") == -1) {
                $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
            } else {
                $('#actFbx').val(type);
                parent.Loading(true);
                $('#fmFile').form('submit', {
                    url: "FinancialReimburseManage.aspx",
                    success: function (res) {
                        parent.Loading(false);
                        $.messager.alert('提示', res, 'info', function () {
                            $('#dlg-Import').dialog('close');
                            $("#dg").datagrid('reload');
                            $('#dg').datagrid('unselectAll');
                        });
                    }
                });
            }
        }
         function uploadBudgetFiles(type) {
            var fileName = $('#fbx1').filebox('getValue');
            if (fileName == "") {
                return;
            }
            if (fileName.indexOf(".xls") == -1) {
                $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
            } else {
                $('#actFbx1').val(type);
                parent.Loading(true);
                $('#fmFile1').form('submit', {
                    url: "FinancialReimburseManage.aspx",
                    success: function (res) {
                        parent.Loading(false);
                        $.messager.alert('提示', res, 'info', function () {
                            $('#dlg-budget-Import').dialog('close');
                            $('#fbx1').filebox('setValue','');
                        });
                    }
                });
            }
        }
        // function dg_Load(){
        //     var url = 
        // }

        var initDatagrid = function () {
            var applystarttm = $("#applyDateFrom").datebox('getValue')==""?"":$("#applyDateFrom").datebox('getValue') + " 00:00:00";
            var applyendtm = $("#applyDateTo").datebox('getValue') == "" ? "" : $("#applyDateTo").datebox('getValue') + " 23:59:59";
            var starttm = $("#dateFrom").datebox('getValue')==""?"":$("#dateFrom").datebox('getValue') + " 00:00:00";
            var endtm = $("#dateTo").datebox('getValue')==""?"":$("#dateTo").datebox('getValue')+ " 23:59:59";
            var apply_name = $("#applyName").textbox('getValue');
            var depart = $("#depart").textbox('getValue');
            var fee_depart = $("#fee_depart").textbox('getValue');
            var fee_detail = $("#fee_detail").textbox('getValue');
            var status = $("#status").combobox('getValue');
            var account_status = $("#account_status").combobox('getValue');

            if (status == '全部') {
                status = '';
            } 
            if (account_status == '全部') {
                account_status = '';
            } else if (account_status == '未审批') {
                account_status = 'null';
            }
            $('#dg').datagrid({
                url: 'FinancialReimburseManage.aspx',
                queryParams: {
                    action: 'getFinancialApprovalData',
                    applystarttm: applystarttm,
                    applyendtm: applyendtm,
                    starttm: starttm,
                    endtm: endtm,
                    apply_name: apply_name,
                    depart: depart,
                    fee_depart: fee_depart,
                    fee_detail: fee_detail,
                    status: status,
                    account_status: account_status,
                    sortName:sortName,
                    sortOrder:sortOrder
                },
                pagination: true,
                pageSize: 100,
                pageList: [100, 200, 500, 1000],
                loadFilter: DataGridPagerFilter,
                singleSelect: false,
                autoRowHeight: false,
                fit: true,
                toolbar: '#tb',
                nowrap: false,
                striped: true,
                rownumbers: true,
                collapsible: false,
                showFooter: true,
                fitColumns: false,
                columns: [[
                    { field: 'code11', checkbox: true, width: 120, align: 'center', title: '复选框', sortable: "true", },
                    {
                        field: 'operate', width: 120, align: 'center', title: '操作', sortable: "true",
                        formatter: function (data, all) {
                            return '<a href="javascript:void(0)" onClick="showDetail(\'' + all.code + '\')" class="showDetailCls">查看明细</a>'
                        }
                    },
                    { field: 'code', width: 120, align: 'center', title: '单据编号', sortable: "true", },
                    {
                        field: 'apply_time', width: 100, align: 'center', title: '产生日期', sortable: "true",
                        formatter: function (data) {
                            return data.split(" ")[0];
                        }
                    },
                    {
                        field: 'LMT', width: 100, align: 'center', title: '提交时间', sortable: "true"
                    },
                    //{ field: 'productCode', width: 10, align: 'center', title: '产品编码', sortable: "true" },
                    {
                        field: 'approval_time', width: 100, align: 'center', title: '审批日期', sortable: "true",
                        formatter: function (data) {
                            return data.split(" ")[0];
                        }
                    },
                    {
                        field: 'account_approval_time', width: 100, align: 'center', title: '财务审批日期', sortable: "true",
                        formatter: function (data) {
                            return data.split(" ")[0];
                        }
                    },
                    { field: 'name', width: 100, align: 'center', title: '提交人' },
                    { field: 'department', width: 200, align: 'center', title: '部门' },
                    // { field: 'deliveryWarehouse', width: 10, align: 'center', title: '发货仓库' },
                    { field: 'fee_company', width: 200, align: 'center', title: '费用归属公司' },
                    { field: 'fee_department', width: 200, align: 'center', title: '费用归属部门' },
                    {
                        field: 'product', width: 100, align: 'center', title: '产品',
                        //formatter: function (data) {
                        //    return data.replace("[", "").replace("]", "").replace(/"/g, "");
                        //}
                    },
                    { field: 'branch', width: 200, align: 'center', title: '网点' },
                    {
                        field: 'fee_detail', width: 100, align: 'center', title: '费用明细',
                        //formatter: function (data) {
                        //    return data.replace("[", "").replace("]", "").replace(/"/g, "");
                        //}
                    },
                    { field: 'project', width: 200, align: 'center', title: '项目编号' },
                    { field: 'fee_amount', width: 100, align: 'center', title: '费用金额' },
                    {
                        field: 'receiptAmount', width: 100, align: 'center', title: '发票金额'
                    },
                    { field: 'actual_fee_amount', width: 100, align: 'center', title: '实报金额' },
                    //{
                    //    field: 'status', width: 10, align: 'center', title: '审批状态',
                    //    //formatter: function (data) {
                    //    //    if (data == 1) {
                    //    //        return "审批中";
                    //    //    } else if (data == 2) {
                    //    //        return "已通过";
                    //    //    } else if (data == 3) {
                    //    //        return "已驳回";
                    //    //    } else if (data == 4) {
                    //    //        return "已取消";
                    //    //    }
                    //    //}
                    //},
                    {
                        field: 'approver', width: 100, align: 'center', title: '审批人',
                        //formatter: function (data) {
                        //    return data.replace("[", "").replace("]", "").replace(/"/g,"");
                        //}
                    },
                    {
                        field: 'account_approver', width: 100, align: 'center', title: '财务审批人',
                        //formatter: function (data) {
                        //    return data.replace("[", "").replace("]", "").replace(/"/g,"");
                        //}
                    },
                    {
                        field: 'informer', width: 100, align: 'center', title: '抄送人',
                        //formatter: function (data) {
                        //    return data.replace("[", "").replace("]", "").replace(/"/g, "");
                        //}
                    },
                    {
                        field: 'ApprovalResult', width: 100, align: 'center', title: '事业部审批结果',
                        formatter: function (value, row, index) {
                            if (value == '同意') {
                                return '<span style="color: #FFFFFF; background-color: #00CC00"> ' + value + ' </span>';
                            }
                            else if (value == '不同意') {
                                return '<span style="color: #FFFFFF; background-color: #FF0000"> ' + value + ' </span>';
                            }
                            else {
                                return '<span>审批中</span>';
                            }
                        }
                    },
                    {
                        field: 'account_result', width: 100, align: 'center', title: '财务审批结果',
                        formatter: function (value, row, index) {
                            if (value == '同意') {
                                return '<span style="color: #FFFFFF; background-color: #00CC00"> ' + value + ' </span>';
                            }
                            else if (value == '拒绝') {
                                return '<span style="color: #FFFFFF; background-color: #FF0000"> ' + value + ' </span>';
                            } else {
                                return '<span>未审批</span>';
                            }
                        }
                    },
                    {
                        field: 'pay_amount', width: 120, align: 'center', title: '出纳付款金额'
                    },
                    { field: 'remark', width: 200, align: 'center', title: '备注' },
                    {
                        field: 'isOverBudget', width: 100, align: 'center', title: '预算外单据',
                        formatter: function (value, row, index) {
                            if (value !== null && value > 0) {
                                return '<span style="color: #FFFFFF; background-color: #FF0000">是</span>';
                            }
                            else {
                                return '<span style="color: #FFFFFF; background-color: #00CC00">否</span>';
                            }
                        }
                    },
                    {
                        field: 'isPrepaid', width: 100, align: 'center', title: '是否公司垫付', sortable: "true",
                        formatter: function (value, row, index) {
                            if (value !== null && value > 0) {
                                return '<span style="color: #FFFFFF; background-color: #FF0000">是</span>';
                            }
                            else {
                                return '<span style="color: #FFFFFF; background-color: #00CC00">否</span>';
                            }
                        }
                    },
                    {
                        field: 'isHasReceipt', width: 100, align: 'center', title: '是否到票', sortable: "true",
                        formatter: function (value, row, index) {
                            if (value !== null && value > 0) {
                                return '<span style="color: #FFFFFF; background-color: #00CC00">是</span>';
                            }
                            else {
                                return '<span style="color: #FFFFFF; background-color: #FF0000">否</span>';
                            }
                        }
                    },
                ]],
                idField: "code",
                // sortName: 'date',
                // sortOrder: 'asc',
                onSortColumn: function (sort, order) {
                    //dg_Load("", sort, order);
                    sortName=sort;
                    sortOrder=order;
                    dg_search();
                },
                onLoadSuccess: function (data) {
                    $('.showDetailCls').linkbutton({ text: '明细', plain: true, iconCls: 'icon-tip' });
                }  
            })
        }

        function showDetail(code) {
            $('#dgDetail').datagrid({
                url: 'FinancialReimburseManage.aspx',
                queryParams: {
                    action: 'getDetailData',
                    code: code,
                },
                autoRowHeight: false,
                fit: true,
                nowrap: false,
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    { field: 'FeeType',  align: 'center', title: '发票类型', sortable: "true", },
                    {
                        field: 'ReceiptAmount',align: 'center', title: '发票金额', sortable: "true",
                    },
                    {
                        field: 'ReceiptTax', align: 'center', title: '发票税额', sortable: "true"
                    },
                    {
                        field: 'ReceiptPlace', align: 'center', title: '发票地点', sortable: "true",
                    },
                    {
                        field: 'ReceiptDesc',  align: 'center', title: '活动内容描述', sortable: "true",
                    },
                    { field: 'ReceiptDate', align: 'center', title: '发票日期' },
                    { field: 'ReceiptPerson', align: 'center', title: '发票人姓名' },
                    { field: 'ReceiptType', align: 'center', title: '发票用途' },
                    { field: 'ReceiptCode', align: 'center', title: '发票编码' },
                    {
                        field: 'ReceiptNum', align: 'center', title: '发票号码',
                    },
                    {
                        field: 'ReceiptAttachment', align: 'center', title: '发票图片',
                        formatter: function (src) {
                            return "<img style='width:66px; height:60px;margin-left:3px;' src='" + src + "' /><a href='javascript:void(0)' onClick='showBigImage(\"" + src + "\")'>预览</a>"
                        }
                    },
                ]],
                idField: "ReceiptNum",
                onLoadSuccess: function (data) {
                    $('#dlg-Detail').dialog('open');
                }  
            });
        }

        function showBigImage(src) {
            $('#bigImgDialog').dialog({
                title: '预览',
                width: 800,
                height: 600,
                resizable: true,
                closed: true,
                cache: false,
                modal: true
            });
            $("#bigImg").attr("src", src);
            $('#bigImgDialog').dialog('open');
        }
    </script>

    </html>