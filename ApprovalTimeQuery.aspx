<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ApprovalTimeQuery.aspx.cs" Inherits="ApprovalTimeQuery" %>

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
    <script src="Scripts/base-loading.js"></script>
    <script src="Scripts/pcCommon.js"></script>
    <script src="Scripts/jquery.json-2.4.min.js"></script>
</head>

<body>
    <table id="dg" class="easyui-datagrid"></table>
    <div id="dg-tb">
        <select id="cbx-type" class="easyui-combobox" style="width:200px;">
            <option value="1">移动报销及发货货需申请</option>
            <option value="2">业力表单</option>
        </select>
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search',"
            onclick="dgLoad()">查询</a>
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-leaveStock'"
            onclick="ExportToExcel('单据审批时间', '单据审批时间', 'dg');">导出Excel</a>

        <!-- <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-edit',"
            onclick="dlgOpen('edit')">编辑</a>
        <input id="dg-searchbox" class="easyui-searchbox" data-options="prompt:'人员或部门或医院或产品的拼音或文字',searcher:doSearch"
            style="width: 300px"> -->

    </div>

    <script type="text/javascript">
        var Url = 'ApprovalTimeQuery.aspx';
        $(document).ready(function () {
            InitTable();
            // dgLoad();
        });

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


        function InitTable() {
            $('#dg').datagrid({
                singleSelect: true, fit: true,
                toolbar: '#dg-tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                columns: [[
                    { field: 'DocId', width: 10, align: 'center', title: '单据编号', sortable: "true" },
                    { field: 'DocName', width: 10, align: 'center', title: '单据名称', sortable: "true" },
                    { field: 'Approver', width: 10, align: 'center', title: '单据提交人', sortable: "true" },
                    { field: 'PreviousOperationTime', width: 30, align: 'center', title: '上一操作人操作时间', sortable: "true" },
                    { field: 'CurrentOperator', width: 10, align: 'center', title: '当前操作人', sortable: "true" },
                    { field: 'Days', width: 10, align: 'center', title: '上一操作距今天数', sortable: "true" }
                ]]
                // onDblClickRow: function () { dlgOpen('edit'); },
                // sortName: 'Hospital',
                // sortOrder: 'asc',
                // onSortColumn: function (sort, order) {
                //     dgLoad($('#dg-searchbox').searchbox('getValue'), sort, order);
                // }
            });
        }

        function dgLoad() {
            var data = {
                act: 'getInfos',
                type: $('#cbx-type').combobox('getValue')
            };
            parent.Loading(true);
            $.post(Url, data, function (res) {
                parent.Loading(false);
                try {
                    var datasource = $.parseJSON(res);
                    $('#dg').datagrid("loadData", datasource);
                } catch (error) {
                    $.messager.alert('错误提示', res + '\r\n' + error);
                }
            });
            // var res = AjaxSync(Url, data);


        }

    </script>
</body>

</html>