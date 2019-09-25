<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CostSharingApproval.aspx.cs" Inherits="CostSharingApproval" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>网点备案审批管理</title>
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

<body class="easyui-layout">
    <div id="loading"
        style="background-position: center center; width: 110px; height: 110px; background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;"
        class="easyui-dialog" border="false" noheader="true" closed="true" modal="true">
    </div>
    <div data-options="region:'west',split:true" title="表单列表" style="width: 200px;">
        <ul class="easyui-datalist" id="tree">
            <li value="0">网点详情</li>
            <li value="107">网点备案新增申请表</li>
            <li value="108">网点备案变更申请表</li>
            <li value="109">网点价格费用及指标新增申请表</li>
            <li value="110">网点价格费用及指标变更申请表</li>
        </ul>
    </div>
    <div id="div-formDetail" data-options="region:'center',title:'表单详细信息'">
        <table id="dg" class="easyui-datagrid"></table>
        <div id="dg-tb" style="padding: 2px 5px;">
            提交日期:
            <input id="dateFrom" class="easyui-datebox" style="width: 110px">
            -
            <input id="dateTo" class="easyui-datebox" style="width: 110px">
            网点:
            <input id="client" class="easyui-textbox" style="width: 110px" />
            产品:
            <input id="product" class="easyui-textbox" style="width: 110px" />
            <!-- 部门:
            <input id="depart" class="easyui-textbox" style="width: 110px" />审批结果:
            <select id="status" class="easyui-combobox" style="width: 110px;"
                data-options="editable:false,panelHeight:'auto'">
                <option>全部</option>
                <option>已审批</option>
                <option>审批中</option>
                <option>已拒绝</option>
            </select> -->

            <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search',"
                onclick="GetFormInfo($('#tree').datalist('getSelected'));">查询</a>&nbsp;
            <!-- <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-print'"
                onclick="exportExcel();">导出</a> -->
            <a id='btn-edit' href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-edit'"
                onclick="openDlg();">编辑</a>
        </div>
    </div>
    <div id="dlg" class="easyui-dialog" title="批量信息导入" data-options="iconCls:'icon-edit',modal:true,closed:true">
        <form id="fm" class="easyui-form" method="post" data-options="novalidate:true">
            <table id="tb" style="width: 100%;">
                <!-- <tr>
                    <td style="width: 5%;">&nbsp;</td>
                    <td style="width: 40%;">&nbsp;</td>
                    <td style="width: 40%;">
                        <input id="act" name="act" type="hidden" />
                        <input  name="Id" type="hidden" />
                    </td>
                    <td style="width: 5%;">&nbsp;</td>
                </tr> -->

            </table>
        </form>
    </div>
</body>
<script>
    var Url = 'CostSharingApproval.aspx'
    $(document).ready(function () {
        InitDg();
        InitDlg();
        $('#btn-edit').linkbutton('disable');
        SetStartAndEndDate();
    });

    $('#tree').datalist({
        fit: true,
        plain: true,
        valueField: 'valueField',
        textField: 'textField',
        singleSelect: true,
        onClickRow: function (index, row) {
            if (row['valueField'] == 0) {
                $('#btn-edit').linkbutton('enable');
            }
            else {
                $('#btn-edit').linkbutton('disable');
            }
            GetFormInfo(row);
        }
    });

    var GetFormInfo = function (row) {

        var data = {
            act: 'GetFormInfo',
            id: row['valueField'],
            name: row['textField'],
            client: $('#client').textbox('getValue'),
            product: $('#product').textbox('getValue'),
            start: $('#dateFrom').datebox('getValue'),
            end: $('#dateTo').datebox('getValue')
        };
        parent.Loading(true);
        $.post(Url, data, function (res) {
            parent.Loading(false);
            var datasource = $.parseJSON(res);
            if (datasource.hasRight == 1 || row['valueField'] == "0") {
                SelectedId = row['valueField'];
                SelectedName = row['textField'];
                $("#div-formDetail").panel({ title: SelectedName });
                var ds = $.parseJSON(datasource.data);
                if (ds.total == 0) {
                    $('#dg').datagrid({ columns: [] });
                    DgLoad(ds);
                }
                else {
                    InitTable(ds.rows[0]);
                    DgLoad(ds);
                }
                // InitTable(ds.rows[0]);
                // DgLoad(ds);
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

    var SetStartAndEndDate = function () {
        var now = new Date();
        var month = now.getMonth() + 1;//js获取到的是月份是 0-11 所以要加1
        var year = now.getFullYear();
        var nextMonthFirstDay = new Date([year, month + 1, 1].join('-')).getTime();
        var oneDay = 1000 * 24 * 60 * 60;
        var monthLast = new Date(nextMonthFirstDay - oneDay).getDate();
        $('#dateFrom').datebox('setValue', [year, month, 1].join('-'));
        $('#dateTo').datebox('setValue', [year, month, monthLast].join('-'));
    }

    var InitDg = function () {
        $('#dg').datagrid({
            singleSelect: true,
            fit: true,
            toolbar: '#dg-tb',
            striped: true,
            rownumbers: true,
            collapsible: false,
            singleSelect: true,
            fitColumns: false,
            onDblClickCell: function (index, field, value) {
                openDlg();
            }
        });
    }

    var DgLoad = function (data) {
        if (data.total == 0) {
            $('#dg').datagrid({ columns: [] }).datagrid("loadData", data);
            return;
        }

        var columns = new Array();
        var obj = data.rows[0];
        // DgImportLength = data.total;
        //columns.push();
        // columns.push({ field: 'code11', checkbox: true, width: 12, align: 'center', title: '复选框', sortable: "true", });
        $.each(obj, function (key, value) {
            if (!isContainChinese(key))
                return true; //Continue
            if (key.indexOf('费用率') != -1) {
                columns.push({
                    field: key, title: key, width: 100, align: 'center',
                    formatter: function (value, row, index) {
                        return value + '%';
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
            columns: [columns]
        }).datagrid("loadData", data);
    }

    var isContainChinese = function (s) {
        if (escape(s).indexOf("%u") < 0) {
            // alert("没有包含中文");
            return false;
        } else {
            // alert("包含中文");
            return true;
        }
    }

    var InitDlg = function () {
        $('#dlg').dialog({//用户对话框
            buttons: [{
                text: '确定',
                iconCls: 'icon-ok', size: 'large',
                handler: function () {
                    Submit();
                }
            }, {
                text: '清除',
                iconCls: 'icon-clear', size: 'large',
                handler: function () {
                    $('#fm').form('clear');
                }
            }, {
                text: '取消',
                iconCls: 'icon-cancel', size: 'large',
                handler: function () {
                    $('#dlg').dialog('close');
                }
            }],
            onOpen: function () {
                var h = $(window).height() - 60;
                var w = $(window).height() - 60;
                $(this).dialog('resize', { height: h, width: w });
                $(this).dialog('move', { top: 30, left: 30 });
            }
        });
    }

    function InitTable(data) {
        var table = $('#tb');
        // $("#tb tr:not(:first)").empty(); //清空table（除了第一行以外）
        table.empty(); //清空table
        //初始化第一行
        {
            var tr = $("<tr></tr>");
            tr.append($('<td style="width: 5%;">&nbsp;</td>'));
            tr.append($('<td style="width: 20%;">&nbsp;</td>'));
            tr.append($('<td style="width: 80%;"><input id="act" name="act" type="hidden" /><input  name="Id" type="hidden" /></td>'));
            tr.append($('<td style="width: 5%;">&nbsp;</td>'));

            table.append(tr);
        }
        $.each(data, function (key, value) {
            if (!isContainChinese(key)) {
                return true;
            }
            var tr = $("<tr></tr>");
            tr.append($("<td>&nbsp;</td>"));
            tr.append($("<td>" + key + "</td>"));
            var textbox = $('<td><input class="easyui-textbox" style="width: 100%;" name="' + key + '"/></td>');
            if (key == '产品' || key == '医院' || key == '代表' || key == '区域经理' || key == '主管'
                || key == '销售负责人' || key == '部门') {
                textbox = $('<td><input class="easyui-textbox" style="width: 100%;" data-options="editable:false" name="' + key + '"/></td>');
            }
            tr.append(textbox);
            tr.append($("<td>&nbsp;</td>"));
            table.append(tr);
        });
        $.parser.parse(table);//重新渲染控件

    }

    function openDlg() {
        if ($('#dg').datagrid('getSelected') == undefined) {
            return;
        }
        $('#fm').form('clear');
        $('#dlg').dialog('open');
        var data = $('#dg').datagrid('getSelected');
        $('#fm').form('load', data);
    }

    function Submit() {
        $('#act').val('submit');
        parent.Loading(true);
        $('#fm').form('submit', {
            url: Url,
            success: function (res) {
                parent.Loading(false);
                $('#dlg').dialog('close');
                var datasource = $.parseJSON(res);
                $.messager.alert({
                    title: '提示信息',
                    msg: res,
                    fn: function () {
                        // GetFormInfo($('#tree').datalist('getSelected'));
                        var data = {};
                        var data1 = $('#fm').serializeArray();
                        $.each(data1, function () {
                            data[this.name] = this.value;
                        });

                        $('#dg').datagrid('updateRow', {
                            index: $('#dg').datagrid('getRowIndex', $('#dg').datagrid('getSelected')),
                            row: data
                        });
                    }
                });
            }
        });
    }

</script>

</html>