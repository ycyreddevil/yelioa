<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SalesReport.aspx.cs" Inherits="SalesReport" %>
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>销售报表</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <link href="Scripts/themes/mobile.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
    <script src="Scripts/jquery.easyui.min.js"></script>
    <script src="Scripts/locale/easyui-lang-zh_CN.js"></script>
    <script src="Scripts/echarts/echarts.min.js"></script>
    <script src="Scripts/mobileCommon.js"></script>
</head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
        background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog" border="false"
        noheader="true" closed="true" modal="true">
    </div>
    <table id="dg1" title="流向信息"><thead data-options="frozen:true"></thead></table>
    <div id="tb" style="padding: 2px 5px;">
        请选择月份:
        <input type="text" id="date" class="easyui-datebox" style="width: 110px" data-options="onSelect:function(date){changeDate(date);}">   
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search'," onclick="insertPrepaid()">填写预付款</a>&nbsp; 
        <a id="switchTable" class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-reload'" onclick="loadDetailData()">切换为公司报表</a>&nbsp; 
    </div>
    <div id="prepaid" class="easyui-dialog" title="新增预付款" data-options="modal:true,closed:true">
        <table id="dg" data-options="onClickRow: onClickRow,scrollbarSize: 0"><thead></thead></table>
    </div>

    <script>
        var year = new Date().getFullYear();
        var month = new Date().getMonth() + 1;
        var day = new Date().getDate();
        var dateStr = year + "-" + month + "-" + day;
        var count = 0;

        $(function () {
            loadDetailData();
            //loadPrepaid();
            $('#date').datebox('setValue', new Date().toString());

            $('#prepaid').dialog({
                buttons: [{
                    text: '关闭',
                    iconCls: 'icon-cancel', size: 'large',
                    handler: function () {
                        $('#prepaid').dialog('close');
                    }
                }],
                onOpen: function () {
                    var w = $(window).width() - 100;
                    var h = $(window).height() - 100;
                    $('#prepaid').dialog('resize', { width: w, height: h });
                    $('#prepaid').dialog('move', { left: 40, top: 40 });
                    loadPrepaid();

                },
                onClose: function () {
                    
                }
            });
        })

        var toCompany = function () {
            count = 1;
            $('#dg1').datagrid('loadData', { total: 0, rows: [] })
            $('#dg1').datagrid({
                url: 'SalesReport.aspx',
                queryParams: {
                    act: 'getCenterDataList',
                    year: year,
                    month: month,
                    dateStr: dateStr,
                    sort: 'monthFlow',
                    order: 'desc',
                },
                singleSelect: true,
                fit: true,
                toolbar: '#tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                showFooter: true,
                frozenColumns: [[
                    {
                        field: 'center',
                        title: '公司',
                        align: "center",
                    },
                ]],
                columns: [[
                    {
                        field: 'monthTask',
                        title: month + '月任务金额',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.monthTask == "" || row.monthTask == null) {
                                return "-";
                            } else {
                                return Number(row.monthTask);
                            }
                        }
                    },
                    {
                        field: 'todayFlow',
                        title: month + '月' + day + '日',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            return Number(row.todayFlow);
                        }
                    },
                    {
                        field: 'monthFlow',
                        title: month + '月累计进货金额',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            return Number(row.monthFlow);
                        }
                    },
                    {
                        field: 'monthPrepaid',
                        title: month + '月预付款',
                        halign: "center",
                        sortable: true,
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.monthPrepaid == "" || row.monthPrepaid == null) {
                                return "-";
                            } else {
                                return Number(row.monthPrepaid);
                            }
                        }
                    },
                    {
                        field: 'monthFlowAndPrepaid',
                        title: month + '月累计进货金额+' + month + '月预付款合计',
                        halign: "center",
                        sortable: true,
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.monthFlowAndPrepaid == "" || row.monthFlowAndPrepaid == null) {
                                return "-";
                            } else {
                                return Number(row.monthFlowAndPrepaid);
                            }
                        }
                    },
                    {
                        field: 'monthCompleteRate',
                        title: month + '月完成率',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.monthCompleteRate == "" || row.monthCompleteRate == null) {
                                return "-";
                            } else {
                                var monthCompleteRate = parseFloat(row.monthCompleteRate) * 100
                                return monthCompleteRate.toFixed(2) + "%";
                            }
                        }
                    },
                    {
                        field: 'monthDvalue',
                        title: month + '月差距金额',
                        halign: "center",
                        sortable: true,
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.monthDvalue == "" || row.monthDvalue == null) {
                                return "-";
                            } else {
                                return Number(row.monthDvalue);
                            }
                        }
                    },
                    {
                        field: 'yearTask',
                        title: '年度考核指标',
                        halign: "center",
                        align: "center",
                        sortable: true,
                        formatter: function (val, row, index) {
                            if (row.yearTask == "" || row.yearTask == null) {
                                return "-";
                            } else {
                                return Number(row.yearTask);
                            }
                        }
                    },
                    {
                        field: 'sumSales',
                        title: '1-12月累计销售金额',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.sumSales == "" || row.sumSales == null) {
                                return "-";
                            } else {
                                return Number(row.sumSales);
                            }
                        }
                    },
                    {
                        field: 'sumPrepaid',
                        title: '1-12月累计预付款',
                        halign: "center",
                        align: "center",
                        sortable: true,
                        formatter: function (val, row, index) {
                            if (row.sumPrepaid == "" || row.sumPrepaid == null) {
                                return "-";
                            } else {
                                return Number(row.sumPrepaid);
                            }
                        }
                    },
                    {
                        field: 'sumSalesAndPrepaid',
                        title: '1-12月累计销售金额+累计预付款合计',
                        halign: "center",
                        align: "center",
                        sortable: true,
                        formatter: function (val, row, index) {
                            if (row.sumSalesAndPrepaid == "" || row.sumSalesAndPrepaid == null) {
                                return "-";
                            } else {
                                return Number(row.sumSalesAndPrepaid);
                            }
                        }
                    },
                    {
                        field: 'sumCompleteRate',
                        title: '1-12月累计完成率',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.sumCompleteRate == "" || row.sumCompleteRate == null) {
                                return "-";
                            } else {
                                var sumCompleteRate = parseFloat(row.sumCompleteRate) * 100;
                                return sumCompleteRate.toFixed(2) + "%";
                            }
                        }
                    },
                ]],
                rowStyler: function (index, row) {
                    if (row.IsFooter) {
                        return 'background-color:#6293BB;color:#fff;font-weight:bold;';
                    }
                },
                sortName: 'monthFlow',
                sortOrder: 'desc',
                onSortColumn: function (sort, order) {
                    dg_sort2(sort, order);
                },
                onLoadSuccess: function (data) {
                    $('#switchTable').linkbutton({ text: '切换为盈利中心报表' });
                    $('#switchTable').attr("onclick", "loadDetailData()");
                }
            })
        }

        var insertPrepaid = function () {
            //$('#dg').datagrid('loadData', { total: 0, rows: [] });
            loadPrepaid();
            $('#prepaid').dialog('open');
        }

        var changeDate = function (date) {
            var date = new Date(date);
            year = date.getFullYear();
            month = date.getMonth() + 1;
            day = date.getDate();
            dateStr = year + "-" + month + "-" + day;
            if ($("#switchTable").linkbutton("options").text == "切换为公司报表") {
                toCompany();
            } else {
                loadDetailData();
            }
            
        }

        var loadDetailData = function () {
            
            if (count == 1) {
                $('#dg1').datagrid('loadData', { total: 0, rows: [] })
            }
            
            $('#dg1').datagrid({
                url: 'SalesReport.aspx',
                queryParams: {
                    act: 'getDataList',
                    year: year,
                    month: month,
                    dateStr: dateStr,
                    sort: 'monthFlow',
                    order: 'desc',
                },
                singleSelect: true,
                fit: true,
                toolbar: '#tb',
                striped: true,
                rownumbers: true,
                collapsible: false,
                fitColumns: true,
                showFooter: true,
                frozenColumns: [[
                    {
                        field: 'Sector',
                        title: '盈利中心',
                        align: "center",
                    },
                ]],
                columns: [[
                    {
                        field: 'monthTask',
                        title: month + '月任务金额',
                        sortable:true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.monthTask == "" || row.monthTask == null) {
                                return "-";
                            } else {
                                return Number(row.monthTask);
                            }
                        }
                    },
                    {
                        field: 'todayFlow',
                        title: month + '月' + day + '日',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            return Number(row.todayFlow);
                        }
                    },
                    {
                        field: 'monthFlow',
                        title: month + '月累计进货金额',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            return Number(row.monthFlow);
                        }
                    },
                    {
                        field: 'monthPrepaid',
                        title: month + '月预付款',
                        halign: "center",
                        sortable: true,
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.monthPrepaid == "" || row.monthPrepaid == null) {
                                return "-";
                            } else {
                                return Number(row.monthPrepaid);
                            }
                        }
                    },
                    {
                        field: 'monthFlowAndPrepaid',
                        title: month + '月累计进货金额+' + month + '月预付款合计',
                        halign: "center",
                        sortable: true,
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.monthFlowAndPrepaid == "" || row.monthFlowAndPrepaid == null) {
                                return "-";
                            } else {
                                return Number(row.monthFlowAndPrepaid);
                            }
                        }
                    },
                    {
                        field: 'monthCompleteRate',
                        title: month + '月完成率',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.monthCompleteRate == "" || row.monthCompleteRate == null) {
                                return "-";
                            } else {
                                var monthCompleteRate = parseFloat(row.monthCompleteRate) * 100
                                return monthCompleteRate.toFixed(2) + "%";
                            }
                        }
                    },
                    {
                        field: 'monthDvalue',
                        title: month + '月差距金额',
                        halign: "center",
                        sortable: true,
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.monthDvalue == "" || row.monthDvalue == null) {
                                return "-";
                            } else {
                                return Number(row.monthDvalue);
                            }
                        }
                    },
                    {
                        field: 'yearTask',
                        title: '年度考核指标',
                        halign: "center",
                        align: "center",
                        sortable: true,
                        formatter: function (val, row, index) {
                            if (row.yearTask == "" || row.yearTask == null) {
                                return "-";
                            } else {
                                return Number(row.yearTask);
                            }
                        }
                    },
                    {
                        field: 'sumSales',
                        title: '1-12月累计销售金额',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.sumSales == "" || row.sumSales == null) {
                                return "-";
                            } else {
                                return Number(row.sumSales);
                            }
                        }
                    },
                    {
                        field: 'sumPrepaid',
                        title: '1-12月累计预付款',
                        halign: "center",
                        align: "center",
                        sortable: true,
                        formatter: function (val, row, index) {
                            if (row.sumPrepaid == "" || row.sumPrepaid == null) {
                                return "-";
                            } else {
                                return Number(row.sumPrepaid);
                            }
                        }
                    },
                    {
                        field: 'sumSalesAndPrepaid',
                        title: '1-12月累计销售金额+累计预付款合计',
                        halign: "center",
                        align: "center",
                        sortable: true,
                        formatter: function (val, row, index) {
                            if (row.sumSalesAndPrepaid == "" || row.sumSalesAndPrepaid == null) {
                                return "-";
                            } else {
                                return Number(row.sumSalesAndPrepaid);
                            }
                        }
                    },
                    {
                        field: 'sumCompleteRate',
                        title: '1-12月累计完成率',
                        sortable: true,
                        halign: "center",
                        align: "center",
                        formatter: function (val, row, index) {
                            if (row.sumCompleteRate == "" || row.sumCompleteRate == null) {
                                return "-";
                            } else {
                                var sumCompleteRate = parseFloat(row.sumCompleteRate) * 100;
                                return sumCompleteRate.toFixed(2) + "%";
                            }
                        }
                    },
                ]],
                rowStyler: function (index, row) {
                    if (row.IsFooter) {
                        return 'background-color:#6293BB;color:#fff;font-weight:bold;';
                    }
                },
                sortName: 'monthFlow',
                sortOrder: 'desc',
                onSortColumn: function (sort, order) {
                    dg_sort2(sort, order);
                },
                onLoadSuccess: function (data) {
                    $('#switchTable').linkbutton({ text: '切换为公司报表' });
                    $('#switchTable').attr("onclick", "toCompany()");
                }
            })
        }

        var dg_sort = function (sort, order) {
            if (sort == undefined) {
                sort = 'monthFlow'; order = 'desc';
            }

            $.ajax({
                url: 'SalesReport.aspx',
                data: {
                    act: 'getDataList',
                    data: JSON.stringify($('#dg1').datagrid('getData').rows),
                    sort: sort,
                    order: order
                },
                dataType: 'json',
                type: 'post',
                success: function (res) {
                    var datasource = $.parseJSON(res);
                    $('#dg1').datagrid("loadData", datasource);
                }
            })
        }

        var loadPrepaid = function () {
            $('#dg').datagrid({
                url: 'mPrepaid.aspx',
                queryParams: {
                    act: 'getPrepaidData',
                },
                singleSelect: true,
                fit: true,
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
                toolbar: [
                    {
                        text: '保存', iconCls: 'icon-save', handler: function () {
                            accept();
                        }
                    },
                    {
                        text: '回退', iconCls: 'icon-undo', handler: function () {
                            reject();
                        }
                    },
                ]
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
</body>
</html>