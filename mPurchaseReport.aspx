<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mPurchaseReport.aspx.cs" Inherits="mPurchaseReport" %>
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
    <script src="Scripts/jquery.easyui.mobile.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/echarts/echarts.min.js"></script>
    <script src="Scripts/mobileCommon.js"></script>
    <script src="Scripts/weui.min.js"></script>
    <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    <script src="Scripts/jquery.edatagrid.js"></script>
</head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
                background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog"
            border="false" noheader="true" closed="true" modal="true">
    </div>
    <div id="p1" class="easyui-navpanel">
        <header>
            <div class="m-toolbar">
                <span class="m-title">采购报表</span>
            </div>
        </header>
        
        <div id="tt" class="easyui-tabs" data-options="tabHeight:40,fit:true,tabPosition:'bottom',border:false,pill:true,narrow:true,justified:true">
            <div id="preview1">
                <div class="panel-header tt-inner">
                    采购日报
                </div>
                <table id="dg" class="easyui-datagrid"></table>
                <div id="tb" style="padding: 2px 5px;">
                    请选择日期:<input type="date" id="date" style="width: 110px">   
                </div>
            </div>
            <div id="preview2">
                <div class="panel-header tt-inner">
                    采购月报
                </div>
                <table id="dg2" class="easyui-datagrid"></table>
                <div id="tb2" style="padding: 2px 5px;">
                    请选择日期:<input type="month" id="date2" style="width: 120px">   
                </div>
            </div>
        </div>         
    </div>
</body>
<script>
    var year = new Date().getFullYear();
    var month = new Date().getMonth() + 1;
    var day = new Date().getDate();
    var dateStr; var monthStr;
    if (month.toString().length == 1) {
        dateStr = year + "-" + "0" + month;
        monthStr = year + "-" + "0" + month;
    } else {
        dateStr = year + "-" + month;
        monthStr = year + "-" + "0" + month;
    }
    if (day.toString().length == 1) {
        dateStr += ("-" + "0" + day);
    } else {
        dateStr += ("-" + day);
    }

    $(document).ready(function () {
        $("#date").val(dateStr);
        $("#date2").val(monthStr);
    })

    $("#date").change(function () {
        InitDailyDatagrid();
    })

    $("#date2").change(function () {
        InitMonthDatagrid();
    })

    $("#tt").tabs({
        onSelect: function (title, index) {
            $("#date").val(dateStr);
            if (index == 0) {
                //$('#dg').datagrid("loadData", { total: 0, rows: [] });
                InitDailyDatagrid();
            } else {
                // 查询和我相关的单据
                //$('#dg').datagrid("loadData", { total: 0, rows: [] });
                //DgLoadOther();
                InitMonthDatagrid();
            }
        },
    })

    function dg_load2(sort, order) {
        InitMonthDatagrid(sort, order)
    }

    function InitMonthDatagrid(sort, order) {
        if (sort == "" || sort == null) {
            sort = "week1";
        }
        if (order == "" || order == null) {
            order = "desc";
        }
        $('#dg2').datagrid({
            url: 'mPurchaseReport.aspx',
            queryParams: {
                act: 'getMonthReport',
                dateString: $("#date2").val(),
                sort: sort,
                order:order
            },
            singleSelect: true,
            fit: true,
            toolbar: '#tb2',
            striped: true,
            rownumbers: true,
            nowrap: false,
            collapsible: false,
            fitColumns: false,
            sortName: sort,
            sortOrder: order,
            onSortColumn: function (sort, order) {
                dg_load2(sort, order);
            },
            /**
            toolbar: [{
                text: '保存',
                iconCls: 'icon-save',
                handler: function () {
                    $('#dg2').data('isSave', true)
                    $('#dg2').edatagrid('saveRow');
                }
            }, {
                text: '取消',
                iconCls: 'icon-undo',
                handler: function () {
                    $('#dg2').edatagrid('cancelRow');
                }
            }],
            */
            frozenColumns: [[
                { field: 'name', width: 100, align: 'center', title: '产品' },
                { field: 'specification', width: 70, align: 'center', title: '规格型号'},
            ]],
            // showFooter: true,
            columns: [[
                { field: 'unit', width: 30, align: 'center', title: '单位', rowspan: 2 },
                {
                    field: 'targetnumber', width: 60, align: 'center', title: '指标数量', rowspan: 2,
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
                    field: 'reportnumber', width: 60, align: 'center', title: '上报数量', rowspan: 2,
                    formatter: function (value, rowData, rowIndex) {
                        if (value == "" || value == null) {
                            return 0;
                        }
                        else {
                            return value;
                        }
                    }},
                {
                    field: 'approvalnumber', width: 60, align: 'center', title: '审批数量', rowspan: 2,
                    formatter: function (value, rowData, rowIndex) {
                        if (value == "" || value == null) {
                            return 0;
                        }
                        else {
                            return value;
                        }
                    }},
                {
                    field: 'actualnumber', width: 60, align: 'center', title: '到货数量', colspan: 5, rowspan: 1
                },
                {
                    field: 'differ', width: 60, align: 'center', title: '审批与到货数量差异', rowspan: 2,
                    formatter: function (value, rowData, rowIndex) {
                        var value1 = rowData.approvalnumber == "" ? 0 : parseInt(rowData.approvalnumber);
                        var value2 = rowData.week1 == "" ? 0 : parseInt(rowData.week1)
                        var value3 = rowData.week2 == "" ? 0 : parseInt(rowData.week2)
                        var value4 = rowData.week3 == "" ? 0 : parseInt(rowData.week3)
                        var value5 = rowData.week4 == "" ? 0 : parseInt(rowData.week4)
                        var value6 = rowData.week5 == "" ? 0 : parseInt(rowData.week5)
                        return value1 - value2 - value3 - value4 - value5 - value6;
                    }},
                { field: 'differreason', width: 60, align: 'center', title: '差异原因', rowspan: 2},
                { field: 'remark', width: 60, align: 'center', title: '备注', rowspan: 2},
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
                    }},
                {
                    field: 'week2', width: 60, align: 'center', title: '第二周', sortable: "true",
                    formatter: function (value, rowData, rowIndex) {
                        if (value == "" || value == null) {
                            return 0;
                        }
                        else {
                            return value;
                        }
                    }},
                {
                    field: 'week3', width: 60, align: 'center', title: '第三周', sortable: "true",
                    formatter: function (value, rowData, rowIndex) {
                        if (value == "" || value == null) {
                            return 0;
                        }
                        else {
                            return value;
                        }
                    }},
                {
                    field: 'week4', width: 60, align: 'center', title: '第四周', sortable: "true",
                    formatter: function (value, rowData, rowIndex) {
                        if (value == "" || value == null) {
                            return 0;
                        }
                        else {
                            return value;
                        }
                    }},
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
                },
            ]],
            onLoadSuccess: function (data) {
                if (data.total == 0) {
                    $.messager.alert('提示', '暂无数据', 'info');
                }
            },
            /**
            onSave: function (index, row) {
                var $datagrid = $('#dg2');
                if ($datagrid.data('isSave')) {
                    //如果需要刷新，保存完后刷新
                    $datagrid.removeData('isSave');

                    var data = $("#dg2").datagrid("getRows");
                    var date = $("#date2").val();
                    var tempYear = date.split("-")[0];
                    var tempMonth = date.split("-")[1];
                    $.ajax({
                        url: 'mPurchaseReport.aspx',
                        data: {
                            act: 'updateDifferReason',
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
            */
            // sortName: 'Hospital',
            // sortOrder: 'asc',
            // onSortColumn: function (sort, order) {
            //     dg_sort(sort, order);
            // }
        });
    }

    function dg_load(sort, order) {
        InitDailyDatagrid(sort, order)
    }

    function InitDailyDatagrid(sort, order) {
        if (sort == "" || sort == null) {
            sort = "PurchaseNumber";
        }
        if (order == "" || order == null) {
            order = "desc";
        }
        $('#dg').datagrid({
            url: 'mPurchaseReport.aspx',
            queryParams: {
                act: 'getDailyReport',
                dateString: $("#date").val(),
                sort: sort,
                order: order
            },
            singleSelect: true,
            fit: true,
            toolbar: '#tb',
            striped: true,
            nowrap: false,
            rownumbers: true,
            collapsible: false,
            fitColumns: true,
            sortName: sort,
            sortOrder: order,
            onSortColumn: function (sort, order) {
                dg_load(sort, order);
            },
            // showFooter: true,
            columns: [[
                {
                    field: 'Date', width: 60, align: 'center', title: '到货日期',
                    formatter: function (val, row, index) {
                        return val.split(" ")[0]
                    }
                },
                { field: 'Name', width: 60, align: 'center', title: '产品' },
                { field: 'Specification', width: 60, align: 'center', title: '规格'},
                { field: 'PurchaseNumber', width: 60, align: 'center', title: '到货数量', sortable: "true" },
            ]],
            onLoadSuccess: function (data) {
                if (data.total == 0) {
                    $.messager.alert('提示', '暂无数据', 'info');
                }
            }
            // sortName: 'Hospital',
            // sortOrder: 'asc',
            // onSortColumn: function (sort, order) {
            //     dg_sort(sort, order);
            // }
        });
    }

    //function dailyLoad() {
    //    Loading(true);
    //    var data = {
    //        act: 'getDailyReport',
    //        dateString: $("#date").val(),
    //    };
    //    $.post('mPurchaseReport.aspx', data, function (res) {
    //        if (res != 'F' && res != "") {
    //            var datasource = $.parseJSON(res);
    //            $('#dg').datagrid("loadData", datasource);
    //        } else {
    //            $.messager.alert('提示', '暂无数据', 'info');
    //        }
    //        Loading(false);
    //    });
    //}
</script>
</html>
