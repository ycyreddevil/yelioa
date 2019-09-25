<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mPersonReport.aspx.cs" Inherits="mPersonReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>业务员达成率报表</title>
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
    <script src="Scripts/weui.min.js"></script>
    <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    <style>
        input[type="month"]::-webkit-calendar-picker-indicator {
           display: none;
        }

        /*----------用来移除叉叉按钮----------*/
        input[type="month"]::-webkit-clear-button{
           display:none;
        }
    </style>
</head>
<body>
    <div id="tb" style="padding: 2px 5px;">
        年：<input class="easyui-textbox" readonly  id="year" value="" style="width: 60px"/><a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search'" id="showYear">年份选择</a>
        &nbsp;月：<input class="easyui-textbox" readonly  id="month" value="" style="width: 60px"/><a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search'" id="showMonth">月份选择</a>
        <%--<input type="text" id="date" class="easyui-datebox" style="width: 100px" data-options="onSelect:function(date){dateBoxOnselectEvent(date);}">   --%>
        <br/>部门：<select id="sector" class="easyui-combobox" style="width:80px;" data-options="editable:false,valueField:'sector',
                                    textField:'sector',
                                    url:'mPersonReport.aspx?act=getAllSector',
                                    method:'post',value:'全部'">
        </select>
        
        <input id="dg-searchbox" class="easyui-searchbox" data-options="prompt:'人员的拼音或文字'" style="width: 90px">
        <a class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-search'," onclick="searchName($('#dg-searchbox').searchbox('getValue'))">查询</a>&nbsp; 
    </div>
    <table id="dg" data-options="header:'#hh1'"><thead data-options="frozen:true"></thead></table>
    <div id="hh1">
        <div class="m-toolbar">
            <div class="m-title">业务员达成率报表</div>
        </div>
    </div>
</body>
<script>
    var year = new Date().getFullYear();
    var month = new Date().getMonth() + 1;
    //var day = new Date().getDate();
    //var dateStr;
    //if (month.toString().length == 1) {
    //    dateStr = year + "-" + "0" + month;
    //} else {
    //    dateStr = year + "-" + month;
    //}
    var name = "";
    var sector = "";

    $(function () {
        $('#year').textbox("setValue", year);
        $('#month').textbox("setValue", month);
        //loadSumData();
        changeSector();
    })

    $("#showYear").click(function () {
        weui.picker([
            {
                label: '2017',
                value: 2017
            },
            {
                label: '2018',
                value: 2018,
            },
            {
                label: '2019',
                value: 2019,
            },
            {
                label: '2020',
                value: 2020,
            }
        ],
            {
                className: 'custom-classname',
                container: 'body',
                defaultValue: [1],
                onChange: function (result) {
                },
                onConfirm: function (result) {
                    year = result[0].value;
                    $('#year').textbox("setValue", result[0].value);
                },
                id: 'showYear'
            });
    })

    $("#showMonth").click(function () {
        var dayArray = new Array();
        for (i = 1; i <= 12; i++) {
            var days = {};
            days["label"] = '' + i;
            days["value"] = i;
            dayArray.push(days);
        }
        weui.picker(dayArray,
            {
                className: 'custom-classname',
                container: 'body',
                defaultValue: [month],
                onChange: function (result) {
                },
                onConfirm: function (result) {
                    month = result[0].value;
                    $('#month').textbox("setValue", result[0].value);
                },
                id: 'showMonth'
            });
    })

    //$("#date").on("change", function () {
    //    dateBoxOnselectEvent();
    //})

    //var dateBoxOnselectEvent = function() {
    //    var this_time = $('#date').val();
    //    var this_datetime = new Date(this_time);

    //    year = this_datetime.getFullYear();
    //    month = this_datetime.getMonth() + 1; 
        
    //    loadSumData();
    //}

    var searchName = function (str) {
        name = str;
        loadSumData();
    }

    var loadSumData = function () {
        if (sector == '全部') {
            sector = '';
        }
        $('#dg').datagrid({
            url: 'mPersonReport.aspx',
            queryParams: {
                act: 'getPersonReport',
                year: year,
                month: month,
                name: name,
                sort: 'completeRate',
                order: 'desc',
                sector: sector
            },
            singleSelect: true,
            fit: true,
            toolbar: '#tb',
            striped: true,
            rownumbers: true,
            collapsible: false,
            fitColumns: true,
            frozenColumns: [[
                {
                    field: 'Sales',
                    title: '业务员',
                    align: "center",
                    
                    fixed: true
                },
            ]],
            columns: [[
                {
                    field: 'Sector',
                    title: '盈利中心',
                    align: "center",
                    
                    fixed:true
                },
                {
                    field: 'monthTask',
                    title: '月指标金额',
                    align: "center",
                    
                    sortable: true,
                    fixed: true,
                    formatter: function (val, row, index) {
                        if (row.monthTask != "" && row.monthTask != null) {
                            return parseFloat(row.monthTask).toFixed(2);
                        }
                    }
                },
                {
                    field: 'netSalesMoney',
                    title: '月纯销金额',
                    align: "center",
                    sortable: true,
                    fixed: true,
                    formatter: function (val, row, index) {
                        if (row.netSalesMoney != "" && row.netSalesMoney != null) {
                            return parseFloat(row.netSalesMoney).toFixed(2);
                        }
                    }
                },
                {
                    field: 'completeRate',
                    title: '月完成率',
                    sortable: true,
                    fixed: true,
                    align: "center",
                    formatter: function (val, row, index) {
                        if (row.completeRate != "" && row.completeRate != null) {
                            var completeRate = parseFloat(row.completeRate) * 100
                            return completeRate.toFixed(2) + "%";
                        }
                    }
                }
            ]],
            sortName: 'completeRate',
            sortOrder: 'desc',
            onSortColumn: function (sort, order) {
                dg_sort(sort, order);
            },
            onLoadSuccess: function (data) {
                if (data.total == 0) {
                    $(this).datagrid('appendRow', { Sector: '<div style="text-align:center;color:red">没有相关记录！</div>' }).datagrid('mergeCells', { index: 0, field: 'Sector', colspan: 4 });
                }
            }
        })
    }

    var dg_sort = function (sort, order) {
        if (sort == undefined) {
            sort = 'completeRate'; order = 'desc';
        }
        $.ajax({
            url: 'mPersonReport.aspx',
            data: {
                act: 'dataGridSort',
                data: JSON.stringify($('#dg').datagrid('getData').rows),
                sort: sort,
                order: order
            },
            dataType: 'json',
            type: 'post',
            success: function (res) {
                var datasource = $.parseJSON(res);
                $('#dg').datagrid("loadData", datasource);
            }
        })
    }

    var changeSector = function () {
        $("#sector").combobox({
            onSelect: function (n, o) {
                sector = n.sector;
                loadSumData();
            }
        })
    }
</script>
</html>
