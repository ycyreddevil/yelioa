<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mPrepaid.aspx.cs" Inherits="mPrepaid" %>

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
    <link href="Scripts/themes/mobile.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/mobileCommon.js"></script>
</head>
<body>
    <table id="dg" data-options="header:'#hh2',onClickRow: onClickRow,scrollbarSize: 0"><thead></thead></table>
    <div id="hh2">
        <div class="m-toolbar">
            <div class="m-title">新增预付款</div>
            <div class="m-right">
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:true" onclick="accept()"></a>
                <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-undo',plain:true" onclick="reject()"></a>
            </div>
        </div>
    </div>
</body>
<script>
    $(function () {
        loadSumData();
    })

    var loadSumData = function () {
        $('#dg').datagrid({
            url: 'mPrepaid.aspx',
            queryParams: {
                act: 'getPrepaidData',
            },
            singleSelect: true,
            fit: true,
            toolbar: '#tb',
            striped: true,
            rownumbers: true,
            collapsible: false,
            fitColumns: true,
            columns: [[
                {
                    field: 'sector',
                    title: '盈利中心',
                    align: "center",
                    width: 80,
                },
                {
                    field: 'year',
                    title: '年',
                    align: "center",
                    width: 80,
                },
                {
                    field: 'month',
                    title: '月',
                    align: "center",
                    width: 80,
                },
                {
                    field: 'prepaidMoney',
                    title: '当月预付款值',
                    align: "center",
                    width: 100,
                    editor: 'numberbox'
                },
                {
                    field: 'sumPrepaidMoney',
                    title: '累计预付款值',
                    align: "center",
                    width: 100,
                    editor: 'numberbox'
                }
            ]],
        })
    }

    var editIndex = undefined;
    function endEditing() {
        if (editIndex == undefined) { return true }
        if ($('#dg').datagrid('validateRow', editIndex)) {
            $('#dg').datagrid('endEdit', editIndex);
            editIndex = undefined;
            return true;
        } else {
            return false;
        }
    }
    function onClickRow(index) {
        if (editIndex != index) {
            if (endEditing()) {
                $('#dg').datagrid('selectRow', index)
                    .datagrid('beginEdit', index);
                editIndex = index;
            } else {
                $('#dg').datagrid('selectRow', editIndex);
            }
        }
    }
    function accept() {
        if (endEditing()) {
            //$('#dg').datagrid('acceptChanges');
            var rows = $("#dg").datagrid('getChanges');
            
            //后台更新
            $.ajax({
                url: 'mPrepaid.aspx',
                data: {
                    act: 'updatePrepaidData',
                    newRows: JSON.stringify(rows)
                },
                dataType: 'json',
                type: 'post',
                success: function () {
                    alert(22)
                }
            })
        }
    }
    function reject() {
        $('#dg').datagrid('rejectChanges');
        editIndex = undefined;
    }
</script>
</html>
