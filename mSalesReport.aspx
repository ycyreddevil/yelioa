<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mSalesReport.aspx.cs" Inherits="mSalesReport"%>

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
    <script src="Scripts/echarts/echarts.min.js"></script>
    <script src="Scripts/mobileCommon.js"></script>
    <script src="Scripts/weui.min.js"></script>
    <link href="Scripts/themes/weui.min.css" rel="stylesheet" />
    <style>
        html, body{
            height: 100%;
        }
        .panel-header, .panel-body {
            border-width: 0px;
        }
        .datagrid,.combo-p{
            border:solid 1px #D4D4D4;
        }
        .datagrid *{
            -webkit-box-sizing: content-box;
            -moz-box-sizing: content-box;
            box-sizing: content-box;
        }
    </style>
</head>
<body>
    <div id="loading" style="background-position: center center; width: 110px; height: 110px; 
        background-image: url('resources/5-121204194037-50.gif'); background-repeat: no-repeat;" class="easyui-dialog" border="false"
        noheader="true" closed="true" modal="true">
    </div>
    <div class="weui-tab" id="tab">
        
        <div class="weui-tab__panel">
            <!-- 公司图div-->
            <div class="weui-tab__content">
                 <div class="weui-cell">
                    <div class="weui-cell__hd"><label for="" class="weui-label" style="font-style:italic">日期:</label></div>
                    <div class="weui-cell__bd" style="margin-left:-52px;font-size: 15px;font-style:italic">
                        <input class="weui-input" type="date"  id="showDatePicker1" value="" />
                    </div>
                </div>
                <div id="main1" style="width: 400px;height:360px;"></div>
                <div id="main2" style="width: 400px;height:360px;margin-top:-40%"></div>
                <div id="main3" style="width: 400px;height:360px;margin-top:-40%"></div>
            </div>
            <!-- 公司表div-->
            <div class="weui-tab__content">
                <div class="weui-cell">
                    <div class="weui-cell__hd"><label for="" class="weui-label" style="font-style:italic">日期:</label></div>
                    <div class="weui-cell__bd" style="margin-left:-52px;font-size: 15px;font-style:italic">
                        <input class="weui-input" type="date" id="showDatePicker2" value="" />
                    </div>
                </div>
                <table id="dg1" data-options="header:'#hh1'"><thead data-options="frozen:true"></thead></table>
                <div id="hh1">
                    <div class="m-toolbar">
                        <div class="m-title">公司销售报表</div>
                    </div>
                </div>
            </div>
            <!-- 盈利中心图div-->
            <div class="weui-tab__content">
                <div class="weui-cell">
                    <div class="weui-cell__hd"><label for="" class="weui-label" style="font-style:italic">日期:</label></div>
                    <div class="weui-cell__bd" style="margin-left:-52px;font-size: 15px;font-style:italic">
                        <input class="weui-input" type="date" id="showDatePicker3" value="" />
                    </div>
                </div>
                <div id="main4" style="width:360px;height:500px"></div>
            </div>
            <!-- 盈利中心表div-->
            <div class="weui-tab__content">
                <div class="weui-cell">
                    <div class="weui-cell__hd"><label for="" class="weui-label" style="font-style:italic">日期:</label></div>
                    <div class="weui-cell__bd" style="margin-left:-52px;font-size: 15px;font-style:italic">
                        <input class="weui-input" type="date" id="showDatePicker4" value="" />
                    </div>
                </div>
                <table id="dg2" data-options="header:'#hh2'"><thead data-options="frozen:true"></thead></table>
                <div id="hh2">
                    <div class="m-toolbar">
                        <div class="m-title">盈利中心销售报表</div>
                    </div>
                </div>
            </div>
        </div>
        <div class="weui-tabbar">
            <a href="javascript:;" class="weui-tabbar__item weui-bar__item_on">
                <img src="Scripts/images/图表.png" alt="" class="weui-tabbar__icon">
                <p class="weui-tabbar__label">公司图</p>
            </a>
            <a href="javascript:;" class="weui-tabbar__item">
                <img src="Scripts/images/表格.png" alt="" class="weui-tabbar__icon">
                <p class="weui-tabbar__label">公司表</p>
            </a>
            <a href="javascript:;" class="weui-tabbar__item">
                <img src="Scripts/images/图表1.png" alt="" class="weui-tabbar__icon">
                <p class="weui-tabbar__label">盈利中心图</p>
            </a>
            <a href="javascript:;" class="weui-tabbar__item">
                <img src="Scripts/images/表格3.png" alt="" class="weui-tabbar__icon">
                <p class="weui-tabbar__label">盈利中心表</p>
            </a>
        </div>
    </div>
    <div style="display: none;" id="dialog11">
        <div class="weui-mask"></div>
        <div class="weui-dialog">
            <div class="weui-dialog__hd"><strong class="weui-dialog__title"></strong></div>
            <div class="weui-dialog__bd" id="dialogContent">弹窗内容，告知当前页面信息等</div>
            <div class="weui-dialog__ft">
                <a href="javascript:;" class="weui-dialog__btn weui-dialog__btn_primary">确定</a>
            </div>
        </div>
    </div>
    
    <%--<input type="text" id="date" class="easyui-datebox" style="width: 110px" data-options="onSelect:function(date){changeDate(date)}">--%>   
    <%--<div class="weui-cell">
        <div class="weui-cell__hd"><label for="" class="weui-label">日期:</label></div>
        <div class="weui-cell__bd">
            <input class="weui-input" type="date" id="showDatePicker" value="" />
        </div>
    </div>--%>
    <%--<a href="javascript:;" class="weui-btn weui-btn_default" id="showDatePicker">日期选择器</a>--%>
   <%-- <a id="changeTable" class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-reload'," onclick="changeData()">切换为详表</a>&nbsp;
    <a id="showChartsOrTables" class="easyui-linkbutton" href="javascript:void(0)" data-options="iconCls:'icon-tip'," onclick="showTables()">展示表格</a>&nbsp;
    --%>
    
    <%--<table id="dg" data-options="header:'#hh'"><thead data-options="frozen:true"></thead></table>
    <div id="hh">
        <div class="m-toolbar">
            <div class="m-title">公司销售报表</div>
        </div>
    </div>--%>
    
</body>
<script>
    var year = new Date().getFullYear();
    var month = new Date().getMonth() + 1;
    var day = new Date().getDate();
    var dateStr;
    if (month.toString().length == 1) {
        dateStr = year + "-" + "0" + month;
    } else {
        dateStr = year + "-" + month;
    }
    if (day.toString().length == 1) {
        dateStr += ("-" + "0" + day);
    } else {
        dateStr += ("-" + day);
    }

    $(function () {
        $('#dg1').datagrid(null)
        $('#dg2').datagrid(null)
        
        $("#showDatePicker1").val(dateStr)
        $("#showDatePicker2").val(dateStr)
        $("#showDatePicker3").val(dateStr)
        $("#showDatePicker4").val(dateStr)

        generateCenterChart();
        
        $('.weui-tabbar__item').on('click', function () {
            $(this).addClass('weui-bar__item_on').siblings('.weui-bar__item_on').removeClass('weui-bar__item_on');
        });
        $(".weui-dialog__btn").click(function () {
            $("#dialog11").fadeOut(200)
        })
        weui.tab('#tab', {
            defaultIndex: 0,
            onChange: function (index) {
                $("#showDatePicker1").val(dateStr)
                $("#showDatePicker2").val(dateStr)
                $("#showDatePicker3").val(dateStr)
                $("#showDatePicker4").val(dateStr)
                if (index == 0) {
                    generateCenterChart();
                }
                else if (index == 1) {
                    loadSumData();
                }
                else if (index == 2) {
                    generateSectorChart();
                }
                else if (index == 3) {
                    loadDetailData();
                }
            }
        });
    })

    $("#showDatePicker1").on("change", function () {
        dateStr = $("#showDatePicker1").val();
        $("#showDatePicker2").val(dateStr)
        $("#showDatePicker3").val(dateStr)
        $("#showDatePicker4").val(dateStr)

        var datesArray = dateStr.split("-");
        year = datesArray[0];
        month = datesArray[1];
        day = datesArray[2];
        generateCenterChart()
    });

    $("#showDatePicker2").on("change", function () {
        dateStr = $("#showDatePicker2").val();
        $("#showDatePicker1").val(dateStr)
        $("#showDatePicker3").val(dateStr)
        $("#showDatePicker4").val(dateStr)

        var datesArray = dateStr.split("-");
        year = datesArray[0];
        month = datesArray[1];
        day = datesArray[2];
        loadSumData();
    });

    $("#showDatePicker3").on("change", function () {
        dateStr = $("#showDatePicker3").val();
        $("#showDatePicker1").val(dateStr)
        $("#showDatePicker2").val(dateStr)
        $("#showDatePicker4").val(dateStr)

        var datesArray = dateStr.split("-");
        year = datesArray[0];
        month = datesArray[1];
        day = datesArray[2];
        generateSectorChart();
    });

    $("#showDatePicker4").on("change", function () {
        dateStr = $("#showDatePicker4").val();
        $("#showDatePicker1").val(dateStr)
        $("#showDatePicker3").val(dateStr)
        $("#showDatePicker2").val(dateStr)

        var datesArray = dateStr.split("-");
        year = datesArray[0];
        month = datesArray[1];
        day = datesArray[2];
        loadDetailData();
    });

    var loadSumData = function () {
        $('#dg1').datagrid({
            url: 'mSalesReport.aspx',
            queryParams: {
                act: 'getCenterDataList',
                year: year,
                month: month,
                dateStr: dateStr,
            },
            singleSelect: true,
            fit: true,
            toolbar: '#tb',
            striped: true,
            rownumbers: true,
            collapsible: false,
            //fitColumns: true,
            showFooter: true,
            frozenColumns: [[
                {
                    field: 'center',
                    title: '业务中心',
                    align: "center",
                    width: 60
                },
            ]],
            columns: [[
                {
                    field: 'todayFlow',
                    title: month + '月' + day + '日',
                    align: "center",
                    width: 90,
                    sortable: "true",
                    formatter: function (val, row, index) {
                        if (row.todayFlow != "" && row.todayFlow != null) {
                            return Number(row.todayFlow);
                        }
                    }
                },
                {
                    field: 'monthFlow',
                    title: month + '月累计进货金额',
                    align: "center",
                    width: 150,
                    sortable: "true",
                    formatter: function (val, row, index) {
                        if (row.monthFlow != "" && row.monthFlow != null) {
                            return Number(row.monthFlow);
                        }
                    }
                },
                {
                    field: 'sumSales',
                    title: '1-'+month+'月累计销售金额',
                    align: "center",
                    sortable: "true",
                    width: 150,
                    formatter: function (val, row, index) {
                        if (row.sumSales != "" && row.sumSales != null) {
                            return Number(row.sumSales);
                        }
                    }
                },
                {
                    field: 'monthCompleteRate',
                    title: month + '月完成率',
                    sortable: "true",
                    align: "center",
                    width: 120,
                    formatter: function (val, row, index) {
                        if (row.monthCompleteRate == "" || row.monthCompleteRate == null) {
                            return "-";
                        }
                        else {
                            var monthCompleteRate = parseFloat(row.monthCompleteRate) * 100
                            return monthCompleteRate.toFixed(2) + "%";
                        }
                    }
                },
                {
                    field: 'sumCompleteRate',
                    title: '1-'+month+'月累计完成率',
                    sortable: "true",
                    align: "center",
                    formatter: function (val, row, index) {
                        if (row.sumCompleteRate == "" || row.sumCompleteRate == null) {
                            return "-";
                        } else {
                            var sumCompleteRate = parseFloat(row.sumCompleteRate) * 100;
                            return sumCompleteRate.toFixed(2) + "%";
                        }
                    },
                    width: 150
                }
            ]],
            sortName: 'sumSales',
            sortOrder: 'desc',
            onSortColumn: function (sort, order) {
                dg_sort(sort, order);
            },
            rowStyler: function (index, row) {
                if (row.IsFooter) {
                    return 'background-color:#6293BB;color:#fff;font-weight:bold;';
                }
            },
        })
    }

    var loadDetailData = function () {
        $('#dg2').datagrid({
            url: 'mSalesReport.aspx',
            queryParams: {
                act: 'getDataList',
                year: year,
                month: month,
                dateStr: dateStr
            },
            singleSelect: true,
            fit: true,
            toolbar: '#tb',
            striped: true,
            rownumbers: true,
            collapsible: false,
            //fitColumns: true,
            showFooter: true,
            frozenColumns: [[
                {
                    field: 'Sector',
                    title: '盈利中心',
                    align: "center",
                    width: 60
                },
            ]],
            columns: [[
                {
                    field: 'todayFlow',
                    title: month + '月' + day + '日',
                    align: "center",
                    width: 150,
                    sortable: "true",
                    formatter: function (val, row, index) {
                        if (row.todayFlow != "" && row.todayFlow != null) {
                            return Number(row.todayFlow);
                        }
                    }
                },
                {
                    field: 'monthFlow',
                    title: month + '月累计进货金额',
                    align: "center",
                    width: 150,
                    sortable: "true",
                    formatter: function (val, row, index) {
                        if (row.monthFlow != "" && row.monthFlow != null) {
                            return Number(row.monthFlow);
                        }
                    }
                },
                {
                    field: 'sumSales',
                    title: '1-'+month+'月累计销售金额',
                    align: "center",
                    width: 150,
                    sortable: "true",
                    formatter: function (val, row, index) {
                        if (row.sumSales != "" && row.sumSales != null) {
                            return Number(row.sumSales);
                        }
                    }
                },
                {
                    field: 'monthCompleteRate',
                    title: month + '月完成率',
                    align: "center",
                    width: 150,
                    sortable: "true",
                    formatter: function (val, row, index) {
                        if (row.monthCompleteRate == "" || row.monthCompleteRate == null) {
                            return "-";
                        }
                        else {
                            var monthCompleteRate = parseFloat(row.monthCompleteRate) * 100
                            return monthCompleteRate.toFixed(2) + "%";
                        }
                    }
                },
                {
                    field: 'sumCompleteRate',
                    title: '1-'+month+'月累计完成率',
                    align: "center",
                    sortable: "true",
                    formatter: function (val, row, index) {
                        if (row.sumCompleteRate == "" || row.sumCompleteRate == null) {
                            return "-";
                        } else {
                            var sumCompleteRate = parseFloat(row.sumCompleteRate) * 100;
                            return sumCompleteRate.toFixed(2) + "%";
                        }
                    },
                    width: 150
                }
            ]],
            rowStyler: function (index, row) {
                if (row.IsFooter) {
                    return 'background-color:#6293BB;color:#fff;font-weight:bold;';
                }
            },
            sortName: 'sumSales',
            sortOrder: 'desc',
            onSortColumn: function (sort, order) {
                dg_sort2(sort, order);
            },
            onLoadSuccess: function (data) {
            }
        })
    }

    var generateCenterChart = function () {
        Loading(true);
        echarts.init(document.getElementById('main1')).clear();
        echarts.init(document.getElementById('main2')).clear();
        echarts.init(document.getElementById('main3')).clear();
        $.ajax({
            url: "mSalesReport.aspx",
            data: {
                act: 'generateCenterChart',
                year: year,
                month: month,
                dateStr: dateStr
            },
            dataType: 'json',
            type: 'post',
            success: function (data) {
                var array1 = data.jArray1;
                var array2 = data.jArray2;
                for (i = 0; i < 3; i++) {
                    var title;
                    if (i == 0) {
                        title='东森'
                    } else if (i == 1) {
                        title='业力'
                    } else {
                        title='中申'
                    }
                    var arrObj1 = array1[i];
                    var arrObj2 = array2[i];
                    option = {
                        title: {
                            text: title,
                            left: '40%',
                            top: '1%'
                        },
                        series: [
                            {
                                name: '年度达成率',
                                type: 'gauge',
                                center: ['60%', '35%'],
                                z: 3,
                                min: 0,
                                max: 100,
                                splitNumber: 10,
                                pointer: {
                                    width: 5
                                },
                                radius: '50%',
                                axisLine: {            // 坐标轴线
                                    lineStyle: {       // 属性lineStyle控制线条样式
                                        width: 7
                                    }
                                },
                                axisTick: {            // 坐标轴小标记
                                    length: 10,        // 属性length控制线长
                                    lineStyle: {       // 属性lineStyle控制线条样式
                                        color: 'auto'
                                    }
                                },
                                splitLine: {           // 分隔线
                                    length: 10,         // 属性length控制线长
                                    lineStyle: {       // 属性lineStyle（详见lineStyle）控制线条样式
                                        color: 'auto'
                                    }
                                },
                                axisLabel: {
                                    backgroundColor: 'auto',
                                    borderRadius: 2,
                                    color: '#eee',
                                    padding: 3,
                                    textShadowBlur: 2,
                                    textShadowOffsetX: 1,
                                    textShadowOffsetY: 1,
                                    textShadowColor: '#222'
                                },
                                title: {
                                    // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                                    fontWeight: 'bolder',
                                    fontStyle: 'italic'
                                },
                                detail: {
                                    // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                                    formatter: function (value) {
                                        return value.toFixed(2)+"%"
                                    },
                                    fontSize: 16,
                                },
                                data: arrObj1
                            },
                            {
                                name: '当月达成率',
                                type: 'gauge',
                                center: ['30%', '35%'],    // 默认全局居中
                                radius: '50%',
                                min: 0,
                                max: 100,
                                endAngle: 45,
                                splitNumber: 10,
                                axisLine: {            // 坐标轴线
                                    lineStyle: {       // 属性lineStyle控制线条样式
                                        width: 8
                                    }
                                },
                                axisTick: {            // 坐标轴小标记
                                    length: 12,        // 属性length控制线长
                                    lineStyle: {       // 属性lineStyle控制线条样式
                                        color: 'auto'
                                    }
                                },
                                splitLine: {           // 分隔线
                                    length: 20,         // 属性length控制线长
                                    lineStyle: {       // 属性lineStyle（详见lineStyle）控制线条样式
                                        color: 'auto'
                                    }
                                },
                                pointer: {
                                    width: 5
                                },
                                title: {
                                    offsetCenter: [0, '-30%'],       // x, y，单位px
                                },
                                detail: {
                                    // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                                    formatter: function (value) {
                                        return value.toFixed(2) + "%"
                                    },
                                    fontWeight: 'bolder',
                                    fontSize: 16,
                                },
                                data: arrObj2
                            }
                        ]
                    };
                    
                    echarts.init(document.getElementById('main' + (i + 1))).setOption(option);

                    
                }
                Loading(false);
            },
            error: function (data) {
                console.log(data)
            }
        })
    }

    var generateSectorChart = function () {
        Loading(true);
        var echart = echarts.init(document.getElementById('main4'))
        echart.clear();
        $.ajax({
            url: 'mSalesReport.aspx',
            data: {
                act: 'generateSectorChart',
                year: year,
                month: month,
                dateStr: dateStr
            },
            dataType: 'json',
            type: 'post',
            success: function (data) {
                var data1 = JSON.parse(data.sectorList);
                var data2 = JSON.parse(data.monthCompleteRateList);
                var data3 = JSON.parse(data.sumCompleteRateList);

                option = {
                    tooltip: {
                        trigger: 'axis',
                        axisPointer: {
                            type: 'shadow'
                        },
                    },
                    legend: {
                        data: ['年度达成率', '月度达成率']
                    },
                    grid: {
                        left: '3%',
                        right: '4%',
                        bottom: '3%',
                        containLabel: true
                    },
                    xAxis: {
                        type: 'value',
                        boundaryGap: [0, 0.01],
                        splitNumber: 3
                    },
                    yAxis: {
                        type: 'category',
                        data: data1,
                        axisLabel: {
                            fontStyle: 'oblique'
                        },
                        triggerEvent:true
                    },
                    series: [
                        {
                            name: '年度达成率',
                            type: 'bar',
                            data: data3,
                            barGap: '0',
                            
                        },
                        {
                            name: '月度达成率',
                            type: 'bar',
                            data: data2,
                        }
                    ]
                };
                echart.setOption(option);

                echart.on('click',
                    function (params) {
                        if (params.componentType != 'yAxis') {
                            return;
                        }
                        Loading(true);
                        $(".weui-dialog").css("height", "85%");
                        $("#dialogContent").css("height", "85%");
                        console.log(params)
                        var clickSector = params.value;
                        // 显示每个盈利中心1-12月的流向
                        $("#dialog11").fadeIn(200);
                        var echart33 = echarts.init(document.getElementById('dialogContent'))
                        echart33.clear();
                        $.ajax({
                            url: 'mSalesReport.aspx',
                            data: {
                                act: 'generateCenterMonthChart',
                                year: year,
                                sector: clickSector
                            },
                            dataType: 'json',
                            type: 'post',
                            success: function (data) {
                                var result = JSON.parse(data.list);
                                option = {
                                    title: {
                                        text: clickSector+'逐月销售额',
                                    },
                                    tooltip: {
                                        trigger: 'axis',
                                        axisPointer: {
                                            type: 'shadow'
                                        }
                                    },
                                    grid: {
                                        left: '0%',
                                        right: '4%',
                                        bottom: '3%',
                                        containLabel: true
                                    },
                                    xAxis: {
                                        type: 'value',
                                        boundaryGap: [0, 0.01],
                                        splitNumber: 3
                                    },
                                    yAxis: {
                                        type: 'category',
                                        data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
                                        axisLabel: {
                                            fontStyle: 'italic',
                                            fontSize: 6
                                        }
                                    },
                                    series: [
                                        {
                                            name: '2011年',
                                            type: 'bar',
                                            data: result,
                                        }
                                    ]
                                };

                                echart33.setOption(option);
                                Loading(false);
                            }
                        })
                    });
                Loading(false);
            },
            error: function (data) {

            }
        })   
    }

    var dg_sort = function (sort, order) {
        if (sort == undefined) {
            sort = 'monthFlow'; order = 'asc';
        }
        
        $.ajax({
            url: 'mSalesReport.aspx',
            data: {
                act: 'dataGridSort',
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

    var dg_sort2 = function (sort, order) {
        if (sort == undefined) {
            sort = 'monthFlow'; order = 'asc';
        }

        $.ajax({
            url: 'mSalesReport.aspx',
            data: {
                act: 'dataGridSort',
                data: JSON.stringify($('#dg2').datagrid('getData').rows),
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
</script>
</html>
