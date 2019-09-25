<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImportPurchase.aspx.cs" Inherits="ImportPurchase" %>

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
        <a class="easyui-linkbutton" style="margin-top:1%" href="javascript:void(0)" data-options="iconCls:'icon-leaveStock'," onclick="OpenDialog(1)">金蝶入库数据导入</a> 
        <a class="easyui-linkbutton" style="margin-top:1%" href="javascript:void(0)" data-options="iconCls:'icon-leaveStock'," onclick="OpenDialog(2)">金博入库数据导入</a> 
    </div>
    <div data-options="region:'center',title:'入库基础数据'" style="background:#eee;">
        <div id="tb2">
            <input type="text" id="date" class="easyui-datebox" style="width: 110px">
            <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-save" onclick="save()">保存</a>
            <a href="javascript:void(0)" class="easyui-linkbutton" iconcls="icon-undo" onclick="cancel()">删除</a>
        </div>
        <table id="dg" class="easyui-datagrid"></table>
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
    var type = "";
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
                    $.post('importPurchase.aspx', data, function (res) {
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

        $('#date').datebox({
            onShowPanel: function () {// 显示日趋选择对象后再触发弹出月份层的事件，初始化时没有生成月份层  
                span.trigger('click'); // 触发click事件弹出月份层  
                if (!tds)
                    setTimeout(function () {// 延时触发获取月份对象，因为上面的事件触发和对象生成有时间间隔  
                        tds = p.find('div.calendar-menu-month-inner td');
                        tds.click(function (e) {
                            e.stopPropagation(); // 禁止冒泡执行easyui给月份绑定的事件  
                            var year = /\d{4}/.exec(span.html())[0]// 得到年份  
                                , month = parseInt($(this).attr('abbr'), 10); // 月份  
                            $('#date').datebox('hidePanel')// 隐藏日期对象  
                                .datebox('setValue', year + '-' + month); // 设置日期的值  
                            initDataGrid();
                        });
                    }, 0);
            },
            parser: function (s) {// 配置parser，返回选择的日期  
                if (!s)
                    return new Date();
                var arr = s.split('-');
                return new Date(parseInt(arr[0], 10), parseInt(arr[1], 10) - 1, 1);
            },
            formatter: function (d) {
                if (d.getMonth() == 0) {
                    return d.getFullYear() - 1 + '-' + 12;
                } else {
                    return d.getFullYear() + '-' + (d.getMonth() + 1);
                }
            }// 配置formatter，只返回年月  
        });
        var p = $('#date').datebox('panel'), // 日期选择对象  
            tds = false, // 日期选择对象中月份  
            span = p.find('span.calendar-text'); // 显示月份层的触发控件  

        var date = new Date();
        var y = date.getFullYear();
        var m = date.getMonth() + 1;

        var temp_date = y + '-' + (m < 10 ? ('0' + m) : m);  
        $("#date").datebox("setValue", temp_date)

        initDataGrid();
    })

    function save() {
        $('#dg').data('isSave', true)
        $('#dg').edatagrid('saveRow');
    }

    function cancel() {
        $('#dg').edatagrid('cancelRow');
    }

    function initDataGrid() {
        var chooseDate = $("#date").datebox("getValue");
        //var year = chooseDate.split("-")[0];
        //var month = chooseDate.split("-")[1];

        $("#dg").edatagrid({
            url: 'importPurchase.aspx',
            queryParams: {
                act: 'getCommonPurchaseData',
                date: chooseDate
            },
            singleSelect: true,
            fit: true,
            iconCls: 'icon-edit',
            striped: true,
            rownumbers: true,
            //collapsible: false,
            toolbar: '#tb2',
            columns: [[
                {
                    field: 'name',
                    title: '商品',
                    align: "center",
                    fixed: true,
                    rowspan:2
                },
                {
                    field: 'specification',
                    title: '规格',
                    align: "center",
                    fixed: true,
                    rowspan: 2
                },
                {
                    field: 'unit',
                    title: '单位',
                    align: "center",
                    fixed: true,
                    rowspan: 2
                },
                {
                    field: 'reportnumber',
                    title: '上报数量',
                    align: "center",
                    fixed: true,
                    //editor: 'numberbox',
                    rowspan: 2
                },
                {
                    field: 'approvalnumber',
                    title: '审批数量',
                    fixed: true,
                    align: "center",
                    //editor: 'numberbox',
                    rowspan: 2
                },
                {
                    field: 'actualnumber', width: 60, align: 'center', title: '到货数量', colspan: 5, rowspan: 1
                },
                {
                    field: 'differ', width: 60, align: 'center', title: '审批与到货数量差异', sortable: "true", rowspan: 2,
                    formatter: function (value, rowData, rowIndex) {
                        var value1 = rowData.approvalnumber == "" ? 0 : parseInt(rowData.approvalnumber);
                        var value2 = rowData.week1 == "" ? 0 : parseInt(rowData.week1)
                        var value3 = rowData.week2 == "" ? 0 : parseInt(rowData.week2)
                        var value4 = rowData.week3 == "" ? 0 : parseInt(rowData.week3)
                        var value5 = rowData.week4 == "" ? 0 : parseInt(rowData.week4)
                        var value6 = rowData.week5 == "" ? 0 : parseInt(rowData.week5)
                        return value1 - value2 - value3 - value4 - value5 - value6;
                    }
                },
                { field: 'differreason', width: 60, align: 'center', title: '差异原因', rowspan: 2, editor: 'text'},
                { field: 'remark', width: 60, align: 'center', title: '备注', rowspan: 2, editor: 'text'},
            ], [
                {
                    field: 'week1', width: 60, align: 'center', title: '第一周', sortable: "true",
                    formatter: function (value, rowData, rowIndex) {
                        if (value == "" || value == null) {
                            return 0;
                        }
                        else {
                            return value;
                        }
                    }
                },
                {
                    field: 'week2', width: 60, align: 'center', title: '第二周', sortable: "true",
                    formatter: function (value, rowData, rowIndex) {
                        if (value == "" || value == null) {
                            return 0;
                        }
                        else {
                            return value;
                        }
                    }
                },
                {
                    field: 'week3', width: 60, align: 'center', title: '第三周', sortable: "true",
                    formatter: function (value, rowData, rowIndex) {
                        if (value == "" || value == null) {
                            return 0;
                        }
                        else {
                            return value;
                        }
                    }
                },
                {
                    field: 'week4', width: 60, align: 'center', title: '第四周', sortable: "true",
                    formatter: function (value, rowData, rowIndex) {
                        if (value == "" || value == null) {
                            return 0;
                        }
                        else {
                            return value;
                        }
                    }
                },
                {
                    field: 'week5', width: 60, align: 'center', title: '第五周', sortable: "true",
                    formatter: function (value, rowData, rowIndex) {
                        if (value == "" || value == null) {
                            return 0;
                        }
                        else {
                            return value;
                        }
                    }
                }
            ]],
            onSave: function (index, row) {
                var $datagrid = $('#dg');
                if ($datagrid.data('isSave')) {
                    //如果需要刷新，保存完后刷新
                    $datagrid.removeData('isSave');

                    var data = $("#dg").datagrid("getRows");
                    var date = $("#date").datebox("getValue");
                    var tempYear = date.split("-")[0];
                    var tempMonth = date.split("-")[1];
                    $.ajax({
                        url: 'importPurchase.aspx',
                        data: {
                            act: 'InsertOrUpdateCommonPurchaseData',
                            dataJson: JSON.stringify(data),
                            year: tempYear,
                            month: tempMonth
                        },
                        type: 'post',
                        dataType: 'json',
                        success: function (data) {
                            $.messager.alert('提示', '保存成功', 'info');
                            $datagrid.edatagrid('reload');
                        },
                        error: function (msg) {

                        }
                    })
                }
            },
        })
    }

    function saveDataGrid() {
        var data = $("#dg").datagrid("getRows");
        var date = $("#date").datebox("getValue");
        var tempYear = date.split("-")[0];
        var tempMonth = date.split("-")[1];
        $.ajax({
            url: 'importPurchase.aspx',
            data: {
                act: 'InsertOrUpdateCommonPurchaseData',
                dataJson: JSON.stringify(data),
                year: tempYear,
                month: tempMonth
            },
            type: 'post',
            dataType: 'json',
            success: function (data) {
                $.messager.alert('提示', '保存成功', 'info');
                initDataGrid();
            },
            error: function (msg) {

            }
        })
    }

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
                url: 'importPurchase.aspx',
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
