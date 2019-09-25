<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FormApprovalManager.aspx.cs" Inherits="FormApprovalManager" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/jquery.edatagrid.js"></script>
    <script src="Scripts/base-loading.js"></script>
    <script src="Scripts/pcCommon.js"></script>
    <style type="text/css">
        table input,
        table select {
            width: 260px;
        }
    </style>
</head>

<body class="easyui-layout">
    <div data-options="region:'west',split:true" title="表单列表" style="width: 300px;">
        <ul class="easyui-datalist" id="tree"></ul>
    </div>
    <div id="div-formDetail" data-options="region:'center',title:'表单详细信息'">
        <table id="dg" class="easyui-datagrid"></table>
        <div id="dg-tb" style="padding: 2px 5px;">
            提交日期:
            <input id="dateFrom" class="easyui-datebox" style="width: 110px"> -
            <input id="dateTo" class="easyui-datebox" style="width: 110px">提交人:
            <input id="applyName" class="easyui-textbox" style="width: 110px" /> 部门:
            <input id="depart" class="easyui-textbox" style="width: 110px" />审批结果:
            <select id="status" class="easyui-combobox" style="width:110px;"
                data-options="editable:false,panelHeight:'auto'">
                <option>全部</option>
                <option>已审批</option>
                <option>审批中</option>
                <option>已拒绝</option>
            </select>

            <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search',"
                onclick="dg_search();">查询</a>&nbsp;
            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-print'"
                onclick="exportExcel();">导出</a>
        </div>
    </div>
    <script>
        var Url = 'FormApprovalManager.aspx';
        var SelectedId, SelectedName, SelectedManageRange;
        $(document).ready(function () {
            InitTree();
            InitDg();
            EditTree();
        });

        function dg_search(){
            var data = {
                act: 'SearchFormInfo',
                name: SelectedName,
                start:$('#dateFrom').textbox('getValue'),
                end:$('#dateTo').textbox('getValue'),
                submitter:$('#applyName').textbox('getValue'),
                department:$('#depart').textbox('getValue'),
                result:$('#status').textbox('getValue')
            };

            parent.Loading(true);
            $.post(Url, data, function (res) {
                parent.Loading(false);
                var datasource = $.parseJSON(res);
                DgLoad(datasource);
            });
        }

        function InitTree() {
            $("#tree").datalist({
                fit: true, plain: true, valueField: 'valueField', textField: 'textField', singleSelect: true,
                onClickRow: function (index, row) {
                    GetFormInfo(row);
                }
            });
        }

        function SetStartAndEndDate() {
            var now = new Date();
            var month = now.getMonth() + 1;//js获取到的是月份是 0-11 所以要加1
            var year = now.getFullYear();
            var nextMonthFirstDay = new Date([year, month + 1, 1].join('-')).getTime();
            var oneDay = 1000 * 24 * 60 * 60;
            var monthLast = new Date(nextMonthFirstDay - oneDay).getDate();
            $('#dateFrom').datebox('setValue', [year, month, 1].join('-'));
            $('#dateTo').datebox('setValue', [year, month, monthLast].join('-'));
        }

        function GetFormInfo(row) {
            SetStartAndEndDate();
            var data = {
                act: 'GetFormInfo',
                id: row['valueField'],
                name: row['textField']

            };
            parent.Loading(true);
            $.post(Url, data, function (res) {
                parent.Loading(false);
                var datasource = $.parseJSON(res);
                if (datasource.hasRight == 1) {
                    SelectedId = row['valueField'];
                    SelectedName = row['textField'];
                    $("#div-formDetail").panel({ title: SelectedName });
                    DgLoad($.parseJSON(datasource.data));
                }
                else {
                    $.messager.alert('提示', '当前用户无权限查询' + row['textField'] + '!', 'info');
                    $('#dg').datagrid({ columns: [] });
                    $("#div-formDetail").panel({ title: '表单详细信息' });
                    DgLoad($.parseJSON(datasource.data));
                }
                // $('#dg').datagrid("loadData", datasource);

            });

        }

        function EditTree() {
            $.ajax({
                url: 'FormApprovalManager.aspx',
                data: { act: 'GetFormApprovalList' },
                type: 'get',
                dataType: 'json',
                success: function (json) {
                    $("#tree").datalist("loadData", json);
                    // var datasource = $.parseJSON(json);
                    // $('#tree').tree("loadData", json);
                }
            })
        }

        function isContainChinese(s) {
            if (escape(s).indexOf("%u") < 0) {
                // alert("没有包含中文");
                return false;
            } else {
                // alert("包含中文");
                return true;
            }
        }


        function InitDg() {
            $('#dg').datagrid({
                singleSelect: true, fit: true,
                toolbar: '#dg-tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true
            });
        }

        

        function DgLoad(data) {
            if (data.total == 0) {
                $('#dg').datagrid({ columns: [] }).datagrid("loadData", data);
                return;
            }

            var columns = new Array();
            var obj = data.rows[0];
            // DgImportLength = data.total;
            //columns.push();
            $.each(obj, function (key, value) {
                if (!isContainChinese(key))
                    return true; //Continue
                if (key == '状态') {
                    columns.push({
                        field: '状态', title: '状态', width: 150, align: 'center', frozen: true,
                        formatter: function (value, row, index) {
                            if (value == '已导入') {
                                return '<span style="color: #FFFFFF; background-color: #00CC00">已导入</span>';
                            }
                            else {
                                return '<span style="color: #FFFFFF; background-color: #FF0000">' + value + '</span>';
                            }
                        }
                    });
                }
                else {
                    var column = {};
                    column["field"] = key;
                    column["title"] = key;
                    column["width"] = 100;
                    column["align"] = 'center';
                    column["sortable"] = true;
                    columns.push(column);
                }
            });
            $('#dg').datagrid({
                columns: [columns],
                fit: true,
                fitColumns: false,
                nowrap: false
            }).datagrid("loadData", data);
        }

        function exportExcel() {
            ExportToExcel(SelectedName+'信息', SelectedName+'信息', 'dg');
        }

    </script>
</body>

</html>