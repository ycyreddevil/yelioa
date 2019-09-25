<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ExportSales.aspx.cs" Inherits="ExportSales" %>

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
    <script src="Scripts/jquery.edatagrid.js"></script>
</head>
<body class="easyui-layout">
    <div id="tb" data-options="region:'north',title:'数据导入',split:true" style="height:100px;">
        <a class="easyui-linkbutton" style="margin-top:1%" href="javascript:void(0)" data-options="iconCls:'icon-leaveStock'," onclick="OpenDialog(1)">金蝶出库数据导入</a> 
        <a class="easyui-linkbutton" style="margin-top:1%" href="javascript:void(0)" data-options="iconCls:'icon-leaveStock'," onclick="OpenDialog(2)">金博出库数据导入</a> 
    </div>
    <div id="dlg-Import" class="easyui-dialog" title="批量信息导入" data-options="iconCls:'icon-leaveStock',modal:true,closed:true">
        <table id="dg-Import" class="easyui-datagrid"></table>
        <div id="tb-Import">
            <form id="fmFile" method="post" enctype="multipart/form-data">
                <div>
                    <input id="jd" class="easyui-filebox" label="信息文件:" labelposition="left" 
                    data-options="onChange:function(){uploadFiles('ShowFile');},prompt:'请选择一个xls文件...'"
                    style="width: 50%" buttontext="请选择文件" accept='application/vnd.ms-excel' name="file1" >
                </div>
                <input type="hidden" name="act" id="actFbx"/>
            </form>
        </div>
    </div>
</body>
<script>
    $(function () {
        $('#dlg-Import').dialog({
            buttons: [{
                text: '导入',
                iconCls: 'icon-leaveStock', size: 'large',
                handler: function () {
                     UploadIndex = 0;
                    $('#dg-Import').datagrid('selectRow', UploadIndex);
                }
            }, {
                text: '关闭',
                iconCls: 'icon-cancel', size: 'large',
                handler: function () {
                    $('#dlg-Import').dialog('close');
                }
            }],
            onOpen: function () {
                var w = $(window).width() - 80;
                var h = $(window).height() - 80;
                $('#dlg-Import').dialog('resize', { width: w, height: h });
                $('#dlg-Import').dialog('move', { left: 40, top: 40 });
                $('#dg-Import').datagrid({
                    columns: [[]]
                });
            },
            onClose: function () {
                //dg_Load();
            }
        });

        $('#dg-Import').datagrid({
            singleSelect: true, fit: true,
            toolbar: '#tb-Import',
            striped: true,
            rownumbers: true,
            collapsible: false,
            fitColumns: false,
            columns: [[]],
            onSelect: function (index, row) {
                if (UploadIndex == index) {
                    var jsonStr = JSON.stringify(row);
                    var data = { act: 'import', json: jsonStr, type: type};
                    $.post('ExportSales.aspx', data, function (res) {
                        var newRow = $.parseJSON(res);
                        $('#dg-Import').datagrid('deleteRow', index);
                        $('#dg-Import').datagrid('insertRow', {
                            index: index,	// index start with 0
                            row: newRow
                        });
                        if (index < (DgImportLength - 1)) {
                            UploadIndex = index + 1;
                            $('#dg-Import').datagrid('selectRow', UploadIndex);
                        }
                        else {//最后一行
                            UploadIndex = -1;
                            //$('#dlg-Import').dialog('close');
                            //dgLoad();
                        }
                    });
                }
            }
        });
    })

    function OpenDialog(index) {
        DatagridClear('dg-Import');
        $('#dlg-Import').dialog('open');
        $('#fmFile').form('clear');

        if (index == 1) {
            type = "jd";
        } else {
            type = "jb";
        }
    }

    function uploadFiles(type) {
        var fileName = $('#jd').filebox('getValue');
        if (fileName == "") {
            return;
        }
        if (fileName.indexOf(".xls") == -1) {
            $.messager.alert('错误提示', '上传的文件必须是xls文件!', 'error');
        } else {
            $('#actFbx').val(type);
            parent.Loading(true);
            $('#fmFile').form('submit', {
                url: 'ExportSales.aspx',
                success: function (res) {
                    parent.Loading(false);
                    try {
                        var datasource = $.parseJSON(res);
                        DgImportLoad(datasource);
                    }
                    catch (e) {
                        $.messager.alert('错误提示', res + '\r\n' + e, 'error');
                    }
                }
            });
        }
    }

    function DgImportLoad(data) {
        var columns = new Array();
        var frozenColumns = new Array();
        var obj = data.rows[0];
        DgImportLength = data.total;
        //columns.push();
        $.each(obj, function (key, value) {
            if (key == '状态') {
                frozenColumns.push({
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
        $('#dg-Import').datagrid({
            columns: [columns],
            frozenColumns: [frozenColumns]
        }).datagrid("loadData", data);
    }

    function DatagridClear(dgId) {
        $('#' + dgId).datagrid('loadData', { total: 0, rows: [] });
    }
</script>
</html>
