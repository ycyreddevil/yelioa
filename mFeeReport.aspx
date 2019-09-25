<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mFeeReport.aspx.cs" Inherits="mFeeReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <meta name="apple-mobile-web-app-status-bar-style" content="black">
    <title>业力报表</title>
    <link href="Scripts/mui/css/mui.min.css" rel="stylesheet" />
    <link href="Scripts/mui/css/mui.picker.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <style>
        .chart {
            height: 200px;
            margin: 0px;
            padding: 0px;
        }
        h5 {
            margin-top: 30px;
            font-weight: bold;
        }
        h5:first-child {
            margin-top: 15px;
        }
        html,
		body {
			background-color: #efeff4;
		}
		.mui-views,
		.mui-view,
		.mui-pages,
		.mui-page,
		.mui-page-content {
			position: absolute;
			left: 0;
			right: 0;
			top: 0;
			bottom: 0;
			width: 100%;
			height: 100%;
			background-color: #efeff4;
		}
		.mui-pages {
			top: 46px;
			height: auto;
		}
		.mui-scroll-wrapper,
		.mui-scroll {
			background-color: #efeff4;
		}
		.mui-page.mui-transitioning {
			-webkit-transition: -webkit-transform 200ms ease;
			transition: transform 200ms ease;
		}
		.mui-page-left {
			-webkit-transform: translate3d(0, 0, 0);
			transform: translate3d(0, 0, 0);
		}
		.mui-ios .mui-page-left {
			-webkit-transform: translate3d(-20%, 0, 0);
			transform: translate3d(-20%, 0, 0);
		}
		.mui-navbar {
			position: fixed;
			right: 0;
			left: 0;
			z-index: 10;
			height: 44px;
			background-color: #f7f7f8;
		}
		.mui-navbar .mui-bar {
			position: absolute;
			background: transparent;
			text-align: center;
		}
		.mui-android .mui-navbar-inner.mui-navbar-left {
			opacity: 0;
		}
		.mui-ios .mui-navbar-left .mui-left,
		.mui-ios .mui-navbar-left .mui-center,
		.mui-ios .mui-navbar-left .mui-right {
			opacity: 0;
		}
		.mui-navbar .mui-btn-nav {
			-webkit-transition: none;
			transition: none;
			-webkit-transition-duration: .0s;
			transition-duration: .0s;
		}
		.mui-navbar .mui-bar .mui-title {
			display: inline-block;
			position: static;
			width: auto;
		}
		.mui-page-shadow {
			position: absolute;
			right: 100%;
			top: 0;
			width: 16px;
			height: 100%;
			z-index: -1;
			content: '';
		}
		.mui-page-shadow {
			background: -webkit-linear-gradient(left, rgba(0, 0, 0, 0) 0, rgba(0, 0, 0, 0) 10%, rgba(0, 0, 0, .01) 50%, rgba(0, 0, 0, .2) 100%);
			background: linear-gradient(to right, rgba(0, 0, 0, 0) 0, rgba(0, 0, 0, 0) 10%, rgba(0, 0, 0, .01) 50%, rgba(0, 0, 0, .2) 100%);
		}
		.mui-navbar-inner.mui-transitioning,
		.mui-navbar-inner .mui-transitioning {
			-webkit-transition: opacity 200ms ease, -webkit-transform 200ms ease;
			transition: opacity 200ms ease, transform 200ms ease;
		}
		.mui-page {
			display: none;
		}
		.mui-pages .mui-page {
			display: block;
		}
		.mui-page .mui-table-view:first-child {
			margin-top: 15px;
		}
		.mui-page .mui-table-view:last-child {
			margin-bottom: 30px;
		}
		.mui-table-view {
			margin-top: 20px;
		}
		.mui-table-view:after {
			height: 0;
		}
		.mui-table-view span.mui-pull-right {
			color: #999;
		}
		.mui-table-view-divider {
			background-color: #efeff4;
			font-size: 14px;
		}
		.mui-table-view-divider:before,
		.mui-table-view-divider:after {
			height: 0;
		}
		.mui-content-padded {
			margin: 10px 0px;
		}
		.mui-locker {
			margin: 35px auto;
			display: none;
		}
    </style>
</head>
<body>
<div id="total">
    <div id="index" class="mui-views">
        <div class="mui-view">
            <div class="mui-navbar">
            </div>
            <div class="mui-pages">

            </div>
        </div>
    </div>

    <!-- 主界面不动、菜单移动 -->
    <!-- 侧滑导航根容器 -->
    <div id="main" class="mui-page">
        <div class="mui-navbar-inner mui-bar mui-bar-nav">
            <button id="search" type="button" class="mui-left mui-action-back mui-btn mui-btn-link mui-btn-nav mui-pull-left">
                <span class="mui-icon mui-icon-search"></span>查询
            </button>
            <button id="toTable" type="button" class="mui-right mui-action-back mui-btn mui-btn-link mui-btn-nav mui-pull-right">
                <span class="mui-icon mui-icon-loop"></span>切换表格
            </button>
            <h1 class="mui-center mui-title">业力报表</h1>
        </div>
        <div class="mui-page-content">
            <div class="mui-off-canvas-wrap mui-draggable mui-slide-in">
                <div class="mui-content mui-scroll-wrapper">
                    <div class="mui-scroll">
                        <!-- 主界面具体展示内容 -->
                        <div class="mui-content-padded">
                            <p style="text-indent: 22px;">
                                这是mui集成百度ECharts的图表示例，ECharts的详细用法及 API 请参考其官方网站: 
                            </p>
                        </div>
                        <div class="mui-content-padded">
                            <h5>柱图示例</h5>
                            <div class="chart" id="barChart"></div>
                            <h5>线图示例</h5>
                            <div class="chart" id="lineChart"></div>
                            <h5>饼图示例</h5>
                            <div class="chart" id="pieChart"></div>
                            <div class="chart" id="feeDetailChart"></div>
                        </div>
                    </div>
                </div>  
            </div>
        </div>
    </div>
    
    <div id="showTable" class="mui-page">
        <div class="mui-navbar-inner mui-bar mui-bar-nav">
            <button type="button" class="mui-left mui-action-back mui-btn mui-btn-link mui-btn-nav mui-pull-left">
                <span class="mui-icon mui-icon-left-nav"></span>返回
            </button>
            
            <h1 class="mui-center mui-title">表格展示</h1>
        </div>
        <div class="mui-page-content">
            <div class="mui-off-canvas-wrap mui-draggable mui-slide-in">
                <div class="mui-content mui-scroll-wrapper">
                    <div class="mui-scroll">
                        <!-- 主界面具体展示内容 -->
                        <div class="mui-content-padded">
                            <p style="text-indent: 22px;">
                                这是mui集成百度ECharts的图表示例，ECharts的详细用法及 API 请参考其官方网站: 
                            </p>
                        </div>
                        <div class="mui-content-padded">
                            <h5>柱图示例</h5>
                            <div>
                                <mu-data-table stripe :columns="columns" :sort.sync="sort" @sort-change="handleSortChange" :data="list">
                                    <template slot-scope="scope">
                                        <td>{{scope.row.name}}</td>
                                        <td class="is-right">{{scope.row.calories}}</td>
                                    </template>
                                </mu-data-table>
                            </div>
                        </div>
                    </div>
                </div>  
            </div>
        </div>
    </div>
    
    <div id="showSearch" class="mui-page">
        <div class="mui-navbar-inner mui-bar mui-bar-nav">
            <button type="button" class="mui-left mui-action-back mui-btn  mui-btn-link mui-btn-nav mui-pull-left">
                <span class="mui-icon mui-icon-left-nav"></span>返回
            </button>
            <h1 class="mui-center mui-title">搜索</h1>
        </div>
        <div class="mui-page-content">
            <div class="mui-content">
                <div class="mui-button-row">
                    <ul class="mui-table-view">
                        <li class="mui-table-view-cell">
                            <a class="mui-navigate-right" href="#showDepartment" id="departmentHref">{{chooseDepartmentName}}</a>
                        </li>
                        <li class="mui-table-view-cell">
                            <a class="mui-navigate-right" href="javascript:void(0)" id="timeHref">{{chooseTime}}</a>
                        </li>
                    </ul>
                </div>
                <div class="mui-button-row">
                    <button type="button" class="mui-btn mui-btn-primary mui-btn-block" @click="search()">确定</button>
                </div>
            </div>
        </div>
    </div>

    <div id="showDepartment" class="mui-page">
        <div class="mui-navbar-inner mui-bar mui-bar-nav">
            <button type="button" class="mui-left mui-action-back mui-btn  mui-btn-link mui-btn-nav mui-pull-left">
                <span class="mui-icon mui-icon-left-nav"></span>返回
            </button>
            <h1 class="mui-center mui-title">搜索结果</h1>
        </div>
        <div class="mui-page-content">
            <div class="mui-input-row mui-search">
                <input id="searchInput" v-model="keyword" type="search" class="mui-input-clear" placeholder="请根据关键字进行搜索" @keyup="searchDepartment('department', $event)">
            </div>
            <div class="mui-input-row">
                <ul class="mui-table-view" id="departmentList">
                    <li v-for="department in departmentList" class="mui-table-view-cell mui-media">
                        <a href="javascript:;">
                            <img class="mui-media-object mui-pull-left" src="Scripts/images/document.png">
                            <div class="mui-media-body">
                                {{department.name | substringDepartmentTitle}}
                                <p class='mui-ellipsis'>{{department.name | substringDepartmentContent}}</p>
                            </div>
                        </a>
                        
                    </li>
                </ul>
            </div>
        </div>
        <p>此示例基于环信 “WebIM SDK” + 环信 “移动客服” 实现，在环信 “移动客服面板” 能够查阅反馈信息。</p>
    </div>
</div>
</body>
</html>
<script src="Scripts/mui/js/mui.min.js"></script>
<script src="Scripts/mui/js/mui.enterfocus.js"></script>
<script src="Scripts/mui/js/mui.view.js"></script>
<script src="Scripts/mui/js/mui.picker.min.js"></script>
<script src="Scripts/echarts/echarts.min.js"></script>
<script src="Scripts/jquery.min.js"></script>
<script src="Scripts/vue.js"></script>
<script src="https://unpkg.com/muse-ui/dist/muse-ui.js"></script>
<script>
    var viewApi;
    var vue = new Vue({
        el: '#total',
        data: {
            departmentList: [],
            keyword: '',
            chooseDepartmentName: '部门查询',
            chooseTime: '日期查询',
            sort: {
                name: '',
                order: 'asc'
            },
            columns: [
                { title: 'Dessert (100g serving)', name: 'name' },
                { title: 'Calories', name: 'calories', align: 'center', sortable: true },
            ],
            list: [
                {
                    name: 'Frozen Yogurt',
                    calories: 159,
                    
                },
                {
                    name: 'Ice cream sandwich',
                    calories: 237,
                   
                },
                {
                    name: 'Eclair',
                    calories: 262,
                    
                },
                {
                    name: 'Cupcake',
                    calories: 305,
                    
                }
            ]
        },
        methods: {
            searchDepartment: function (type, e) {
                if (e.keyCode == 13) {
                    this.chooseDepartmentName = '部门查询';
                    setDepartmentList(type,this.keyword);
                }
            },
            search: function() {
                viewApi.back();
            },
            handleSortChange ({name, order}) {
                this.list = this.list.sort((a, b) => order === 'asc' ? a[name] - b[name] : b[name] - a[name]);
            }
        },
        mounted: function() {
            mui.init();
            viewApi = mui('#index').view({
                defaultPage: '#main'
            });

            (function ($) {
                // 绑定搜索框跳转
                document.getElementById("search").addEventListener('tap',function() {
                    viewApi.go("#showSearch");
                });
                // 绑定时间选择 显示datetimepicker
                document.getElementById("timeHref").addEventListener('tap', function () {
                    var optionsJson = {
                        type: "month",//设置日历初始视图模式 
                        labels: ['年', '月'],//设置默认标签区域提示语 
                        customData: { 
                            h: [
                                { value: 'AM', text: 'AM' },
                                { value: 'PM', text: 'PM' }
                            ] 
                        }
                    }
                    var dtpicker = new mui.DtPicker(optionsJson);
                    dtpicker.show(function(e) {
                        vue.chooseTime = e.value;
                    });
                });

                // 绑定切换表格跳转
                document.getElementById("toTable").addEventListener('tap', function() {
                    viewApi.go("#showTable");
                });

                //处理view的后退与webview后退
                var oldBack = $.back;
                $.back = function() {
                    if (viewApi.canBack()) { //如果view可以后退，则执行view的后退
                        viewApi.back();
                    } else { //执行webview后退
                        //oldBack();
                    }
                };
            })(mui);

            var byId = function(id) {
                return document.getElementById(id);
            };
            var barChart = echarts.init(byId('barChart'));
            barChart.setOption(getOption('bar'));
            var lineChart = echarts.init(byId('lineChart'));
            lineChart.setOption(getOption('line'));
            var pieChart = echarts.init(byId('pieChart'));
            pieChart.setOption(getOption('pie'));

            mui('#departmentList').on('tap','li',
                function() {
                    vue.chooseDepartmentName = this.innerText;
                    viewApi.back();
                }
            );
        }
    });

    // 截取部门来显示
    Vue.filter("substringDepartmentTitle", function(value) {
        return value.substring(value.lastIndexOf("/")+1, value.length);
    });
    Vue.filter("substringDepartmentContent", function(value) {
        return value.substring(value.indexOf("/")+1, value.lastIndexOf("/"));
    });

    function setDepartmentList(type,keyword) {
        $.post('mFeeReport.aspx', { act: type, keyWord: keyword},function (data) {
            vue.departmentList = JSON.parse(data);
        });
    }
    
    function getOption(chartType) {
        var chartOption = chartType == 'pie' ? {
            calculable: false,
            series: [{
                name: '访问来源',
                type: 'pie',
                radius: '70%',//管理图形大小占比的
                center: ['50%', '50%'],//管理图形水平位置的
                data: [{
                    value: 335,
                    name: '直接访问'
                }, {
                    value: 310,
                    name: '邮件营销'
                }, {
                    value: 234,
                    name: '联盟广告'
                }, {
                    value: 135,
                    name: '视频广告'
                }, {
                    value: 1548,
                    name: '搜索引擎'
                }]
            }]
        } : {
            legend: {//标题
                data: ['蒸发量', '降水量']
            },
            grid: {
                x: 35,
                x2: 10,
                y: 30,
                y2: 25
            },
            toolbox: {//工具箱
                show: false,
                feature: {
                    mark: {
                        show: true
                    },
                    dataView: {
                        show: true,
                        readOnly: false
                    },
                    magicType: {
                        show: true,
                        type: ['line', 'bar']
                    },
                    restore: {
                        show: true
                    },
                    saveAsImage: {
                        show: true
                    }
                }
            },
            calculable: false,
            //横纵轴刻度
            xAxis: [{
                type: 'category',
                data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
            }],
            yAxis: [{
                type: 'value',
                splitArea: {
                    show: true
                }
            }],
            //显示数据  此处数据名的名称还要与标题的名称相对应，否则无法显示
            series: [{
                name: '蒸发量',
                type: chartType,
                data: [2.0, 4.9, 7.0, 23.2, 25.6, 76.7, 135.6, 162.2, 32.6, 20.0, 6.4, 3.3]
            }, {
                name: '降水量',
                type: chartType,
                data: [2.6, 5.9, 9.0, 26.4, 28.7, 70.7, 175.6, 182.2, 48.7, 18.8, 6.0, 2.3]
            }]
        };
        return chartOption;
    }
</script>
