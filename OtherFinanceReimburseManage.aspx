<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OtherFinanceReimburseManage.aspx.cs" Inherits="OtherFinanceReimburseManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>其他费用管理</title>
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
    <div id="loading"
        style="background-position: center center; width: 110px; height: 110px;background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;"
        class="easyui-dialog" border="false" noheader="true" closed="true" modal="true">
    </div>
    <table id="dg" class="easyui-datagrid" title="其他费用管理">

    </table>
    <div id="tb" style="padding: 2px 5px;">
        年:<input id="year" class="easyui-textbox" style="width: 110px">
        月:<input id="month" class="easyui-textbox" style="width: 110px">
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search',"
            onclick="dg_search();">查询</a>&nbsp;
       <%-- <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-add'"
            onclick="openDialogAdd()">新增</a>--%>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'"
            onclick="openDialogEdit()">编辑</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-remove'"
            onclick="exportExcel();">删除</a>
        <a href="javascript:void(0)" class="easyui-linkbutton"
            onclick="$('#dlg-budget-Import').dialog('open');">导入Excel</a>
    </div>
    <div id="dlg-budget-Import" class="easyui-dialog" title="工资导入" style="width: 800px; height: 200px"
        data-options="iconCls:'icon-import',modal:true,closed:true">
        <form id="fmFile" method="post" enctype="multipart/form-data">
            <input id="fbx" class="easyui-filebox" label="非移动报销文件:" labelposition="left"
                data-options="prompt:'请选择一个xls文件...'" style="width: 100%" buttontext="请选择文件"
                accept='application/vnd.ms-excel' name="file">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <input type="hidden" name="act" id="actFbx" />
            &nbsp;&nbsp;&nbsp;<select id='wage-year' name="wage-year"></select>
            &nbsp;&nbsp;&nbsp;<select id='wage-month' name="wage-month"></select>
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="ImportExcel()">提交</a>
            <a href="javascript:void(0)" onclick="location.href='Template/非移动报销导入模板.xlsx'">下载非移动报销导入模板</a>
        </form>
        <form id="fmFile1" method="post" enctype="multipart/form-data">
            <input id="fbx1" class="easyui-filebox" label="流向、纯销、毛利文件:" labelposition="left"
                data-options="prompt:'请选择一个xls文件...'" style="width: 100%" buttontext="请选择文件"
                accept='application/vnd.ms-excel' name="file">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <input type="hidden" name="act" id="actFbx1" />
            &nbsp;&nbsp;&nbsp;<select id='wage-year1' name="wage-year"></select>
            &nbsp;&nbsp;&nbsp;<select id='wage-month1' name="wage-month"></select>
            <a href="javascript:void(0)" class="easyui-linkbutton" onclick="ImportExcel1()">提交</a>
            <a href="javascript:void(0)" onclick="location.href='Template/流向纯销毛利导入模板.xlsx'">下载流向、纯销、毛利导入模板</a>
        </form>
    </div>
    <div id="dlg" class="easyui-dialog" title="新增" data-options="iconCls:'icon-save',modal:true,closed:true,buttons: [{
                    text:'确定',
                    iconCls:'icon-ok',
                    handler:function(){
                        addOrUpdate();
                    }
                },{
                    text:'取消',
                    handler:function(){
                        $('#dlg').dialog('close');
                    }
                }]"
        style="width:600px;height:650px;display: none">
        <form id="form" method="post">
            <div style="margin-bottom:20px;display:none">
                <input class="easyui-textbox" id="dataId" name="Id" style="width:70%" data-options="label:'Id:'">
            </div>
            <div style="margin-bottom:20px">
                <input name="ReportDepartmentName" id="parentDepart" class="easyui-textbox" style="width: 70%;"
                    data-options="label:'部门:',required:true" />
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-textbox" id="feeName" name="FeeName" style="width:70%"
                    data-options="label:'费用名称:',required:true">
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-numberbox" id="feeAmount" name="FeeAmount" style="width:70%"
                    data-options="label:'费用金额:',required:true">
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-numberbox" id="budget" name="Budget" style="width:70%"
                    data-options="label:'预算:',required:true">
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-textbox" id="startDate" name="Year" style="width:70%;"
                    data-options="label:'年:',multiline:true,required:true">
            </div>
            <div style="margin-bottom:20px">
                <input class="easyui-textbox" id="endDate" name="Month" style="width:70%;"
                    data-options="label:'月:',multiline:true,required:true">
            </div>
        </form>
    </div>
</body>
<script>
    const Url = 'OtherFinanceReimburseManage.aspx';
    let TreeDataJson;

    $(function () {
        InitWageDate();
        initDatagrid();
        TreeLoad();
    })

    ///初始化工资录入年月
    function InitWageDate() {
        var date = new Date;
        var year = date.getFullYear()-1;
        var month = date.getMonth();

        $("#wage-year").append("<option value="+year+">"+year+"</option>"); 
        year++;
        $("#wage-year").append("<option value="+year+">"+year+"</option>"); 
        year++;
        $("#wage-year").append("<option value="+year+">"+year+"</option>"); 
        year--;
        $("#wage-year").val(year);

        for(var i=1;i<=12;i++){
            $("#wage-month").append("<option value="+i+">"+i+"</option>"); 
        }
        $("#wage-month").val(month);

        year = date.getFullYear() - 1;
        $("#wage-year1").append("<option value=" + year + ">" + year + "</option>");
        year++;
        $("#wage-year1").append("<option value=" + year + ">" + year + "</option>");
        year++;
        $("#wage-year1").append("<option value=" + year + ">" + year + "</option>");
        year--;
        $("#wage-year1").val(year);

        for (var i = 1; i <= 12; i++) {
            $("#wage-month1").append("<option value=" + i + ">" + i + "</option>");
        }
        $("#wage-month1").val(month);
    }

    function AjaxSync(url, data) {
        parent.Loading(true);
        var res = $.ajax({
            async: false,
            cache: false,
            type: 'post',
            url: url,
            data: data
        }).responseText;
        parent.Loading(false);
        return res;
    }

    function TreeLoad(selectedId) {
        var data = {
            act: 'getTree'
        };
        parent.Loading(true);
        $.post(Url, data, function (res) {
            parent.Loading(false);
            if (res != "F") {
                TreeDataJson = res;
            }
        });
    }

    function openDialogAdd() {
        $('#dlg').dialog('open')
        $("#form").form('reset');
        $('#dlg').dialog({//组织架构对话框
            buttons: [{
                text: '确定',
                iconCls: 'icon-ok',
                handler: function () {
                    addOrUpdate();
                }
            }, {
                text: '取消',
                iconCls: 'icon-cancel',
                handler: function () {
                    $('#dlg').dialog('close');
                }
            }]
        });
    }

    function openDialogEdit() {
        // 只能选择一个进行编辑
        var data = $("#dg").datagrid("getChecked");
        if (data.length == 0) {
            $.messager.alert('提示', '请选择要编辑的单据', 'info');
        } else if (data.lengtj > 1) {
            $.messager.alert('提示', '只能选择一项进行编辑', 'info');
        } else {
            $('#dlg').dialog('open')
            $("#form").form('load', data[0]);
        }
    }

    function addOrUpdate() {
        var data = {
            act: 'insertOrUpdateOtherFee',
            id: $("#dataId").textbox('getValue'),
            Year: $("#startDate").textbox('getValue'),
            Month: $("#endDate").textbox('getValue'),
            FeeAmount: $("#feeAmount").numberbox('getValue'),
            Budget: $("#budget").numberbox('getValue'),
            FeeName: $("#feeName").textbox('getValue'),
            ReportDepartmentName: $("#parentDepart").textbox('getValue')
        }

        var res = AjaxSync(Url, data)
        res = JSON.parse(res)

        $('#dlg').dialog('close')
        if (res.code == 200) {
            $.messager.alert('提示', '操作成功', 'info');

            if ($("#dataId").textbox('getValue') != null && $("#dataId").textbox('getValue') != '') {
                $('#dg').datagrid('updateRow', {
                    index: $('#dg').datagrid('getRowIndex', $('#dg').datagrid('getSelected')),
                    row: data
                });
            } else {
                $('#dg').datagrid('insertRow', {
                    row: data
                });
            }

        } else {
            $.messager.alert('提示', '操作失败', 'info');
        }
    }

    function initDatagrid() {
        // 查询当月已设置的其他费用
        $('#dg').datagrid({
            url: Url,
            queryParams: {
                act: 'initDatagrid',
                year: $("#year").val(),
                month: $("#month").val(),
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
            fitColumns: true,
            columns: [[
                { field: 'code11', checkbox: true, width: 12, align: 'center', title: '复选框' },
                { field: 'ReportDepartmentName', width: 14, align: 'center', title: '部门' },
                {
                    field: 'FeeName', width: 10, align: 'center', title: '费用名称',
                },
                {
                    field: 'FeeAmount', width: 8, align: 'center', title: '费用金额', sortable: "true"
                },
                {
                    field: 'Budget', width: 8, align: 'center', title: '预算', sortable: "true"
                },
                {
                    field: 'Year', width: 8, align: 'center', title: '年',
                },
                {
                    field: 'Month', width: 8, align: 'center', title: '月',
                },
                { field: 'UserName', width: 8, align: 'center', title: '提交人' },
                { field: 'CreateTime', width: 8, align: 'center', title: '创建时间' }
            ]],
        })
    }

    function dg_search() {
        initDatagrid()
    }

    function ImportExcel() {
        var fileName = $('#fbx').filebox('getValue');
        if (fileName == "") {
            return;
        }
        if (fileName.indexOf(".xls") == -1) {
            $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
        } else {
            $('#actFbx').val('importWage');
            parent.Loading(true);
            $('#fmFile').form('submit', {
                url: Url,
                success: function (res) {
                    parent.Loading(false);
                    $.messager.alert('提示', res);
                }
            });
        }
    }

    function ImportExcel1() {
        var fileName = $('#fbx1').filebox('getValue');
        if (fileName == "") {
            return;
        }
        if (fileName.indexOf(".xls") == -1) {
            $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
        } else {
            $('#actFbx1').val('importFlow');
            parent.Loading(true);
            $('#fmFile1').form('submit', {
                url: Url,
                success: function (res) {
                    parent.Loading(false);
                    $.messager.alert('提示', res);
                }
            });
        }
    }
</script>

</html>