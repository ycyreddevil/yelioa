<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mDailyProfit.aspx.cs" Inherits="mDailyProfit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>日利润</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link rel="stylesheet" href="Scripts/themes/mobile.css">
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/vant@2.0/lib/index.css">
    <link rel="stylesheet" href="https://cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.css">
    <link rel="stylesheet" href="https://gw.alipayobjects.com/os/rmsportal/YmDAMEQVbLJpVbKiRQVX.css" />
</head>
<body>
    <div id="vm">
        <van-nav-bar title="日利润" right-text="日期查询" left-text="部门查询" @click-right="timeDialog = true" @click-left="openDepartmentDialog"></van-nav-bar>
        <van-panel title="利润（万元）" :desc="profit.toFixed(3)" ></van-panel>
        <mu-container>
            <mu-tabs :value.sync="active1" @change="changeType" inverse color="secondary" text-color="rgba(0, 0, 0, .54)" center full-width>
                <mu-tab>数据表</mu-tab>
                <mu-tab>柱状图</mu-tab>
                <%--<mu-tab>费用分析</mu-tab>--%>
            </mu-tabs>
            <div class="demo-text" v-show="active1 === 0">
                <H4 style="text-align:center">{{chooseDepartmentName}} 日利润报表</H4>
                <el-table :data="tableData" stripe style="width: 100%">
                    <el-table-column
                      fixed
                      prop="name"
                      label="部门"
                      width="100">
                    </el-table-column>
                    <el-table-column sortable
                      prop="overcome"
                      label="收入"
                      width="100">
                    </el-table-column>
                    <el-table-column sortable
                      prop="cost"
                      label="成本"
                      width="100">
                    </el-table-column>
                    <el-table-column sortable
                      prop="reimburse"
                      label="费用"
                      width="100">
                    </el-table-column>
                    <el-table-column sortable
                      prop="other_fee"
                      label="其他"
                      width="100">
                    </el-table-column>
                    <el-table-column sortable
                      prop="profit"
                      label="利润"
                      width="100">
                    </el-table-column>
                </el-table>
            </div>
            <div class="demo-text" v-show="active1 === 1">
                <canvas id="mountNode"></canvas>
            </div>
           <%-- <div class="demo-text" v-show="active1 === 2">
                <canvas id="mountNode"></canvas>
            </div>--%>
        </mu-container>
        <mu-dialog title="日期查询" width="360" :open.sync="timeDialog">
            <mu-date-input value-format="YYYY-MM-DD" v-model="startTm" label="开始日期" type="date" label-float full-width></mu-date-input>
            <mu-date-input value-format="YYYY-MM-DD" v-model="endTm" label="结束日期" type="date" label-float full-width></mu-date-input>
            <mu-button slot="actions" flat color="primary" @click="queryByDate">确定</mu-button>
            <mu-button slot="actions" flat color="danger" @click="closeTimeDialog">取消</mu-button>
        </mu-dialog>
        <mu-dialog ref="departmentDialog" width="360" fullscreen transition="slide-right" :open.sync="departmentDialog">
            <mu-appbar color="primary" :title="chooseDepartmentName" style="text-align: center">
                <mu-button slot="left" flat @click="findParentDepartment(chooseDepartmentId)">
                返回
                </mu-button>
                <mu-button slot="right" flat @click="queryByDepartment">
                确定
                </mu-button>
            </mu-appbar>
            <mu-list v-for="department in departmentList">
                <mu-list-item button :ripple="false" @click="findDepartment(department.id, this.startTm)">
                    <mu-list-item-action>
                         <mu-icon value="work" color="blue"></mu-icon>
                    </mu-list-item-action>
                    <mu-list-item-title>{{department.name}}</mu-list-item-title>
                </mu-list-item>
            </mu-list>
        </mu-dialog>
    </div>
</body>
<script src="Scripts/jquery.min.js"></script>
<script src="Scripts/mobileCommon.js"></script>
<script src="Scripts/vue.js"></script>
<script src="Scripts/echarts/echarts.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/vant@2.0/lib/vant.min.js"></script>
<script src="https://unpkg.com/muse-ui/dist/muse-ui.js"></script>
<script src="https://unpkg.com/element-ui/lib/index.js"></script>
<script src="https://gw.alipayobjects.com/os/antv/assets/f2/3.3.8/f2.min.js"></script>
<script src="https://gw.alipayobjects.com/os/antv/assets/lib/jquery-3.2.1.min.js"></script>
<script src="https://gw.alipayobjects.com/os/rmsportal/NjNldKHIVQRozfbAOJUW.js"></script>

<script>
    var vm = new Vue({
        el: '#vm',
        data: {
            active1: 0,
            timeDialog: false,
            departmentDialog: false,
            startTm: '',
            endTm: '',
            departmentList: [],
            departmentName: '',
            overcome: 0,
            cost: 0,
            reimburse: 0,
            other_fee: 0,
            profit: 0,
            subDepartmentDictList: [],
            subDepartmentTimeDictList: [],
            timeList: [],
            toast: '',
            dialog: this.$dialog,
            sort: {
                name: '',
                order: 'asc'
            },
            columns: [
                { title: '部门', width: 100, name: 'name' },
                { title: '收入', name: 'overcome', width: 100, align: 'center', },
                { title: '成本', name: 'cost', width: 100, align: 'center'},
                { title: '费用', name: 'reimburse', width: 100, align: 'center'},
                { title: '其他', name: 'other_fee', width: 100, align: 'center'},
                { title: '利润', name: 'profit', width: 100, align: 'center'},
            ],
            tableData: [],
            barChartData: [],
            chooseDepartmentName: '业力集团',
            chooseDepartmentId: 1,
            feeAnalyseData: []
        },
        methods: {
            closeTimeDialog() {
                this.timeDialog = false
            },
            closeDepartmentDialog() {
                this.departmentDialog = false
            },
            openDepartmentDialog() {
                this.toast = this.$toast({
                    type: 'loading',
                    mask: true,
                    message: '加载中...',
                    duration: 0
                })
                this.departmentDialog = true
                findDepartment('1', this.startTm)
            },
            findDepartment(key) {
                this.toast = this.$toast({
                    type: 'loading',
                    mask: true,
                    message: '加载中...',
                    duration: 0
                })
                findDepartment(key, this.startTm)
            },
            findParentDepartment(key) {
                this.toast = this.$toast({
                    type: 'loading',
                    mask: true,
                    message: '加载中...',
                    duration: 0
                })
                if (key != 1) 
                    findParentDepartment(key)
                
            },
            changeType(value) {
                this.active1 = value
                if (value == 0) {
                    var promise = new Promise(resolve => getData(resolve, this.startTm, this.endTm, this.chooseDepartmentId));
                    promise.then(res => {
                        const data = JSON.parse(res);
                        vm.tableData = data;
                        vm.profit = 0;
                        vm.overcome = 0;
                        vm.cost = 0;
                        vm.reimburse = 0;
                        vm.other_fee = 0;

                        vm.tableData.forEach((value, index) => {
                            vm.profit += value.profit
                            vm.overcome += value.overcome
                            vm.cost += value.cost
                            vm.reimburse += value.reimburse
                            vm.other_fee += value.other_fee
                        })
                        this.toast.clear();
                    })
                } else if (value == 1) {
                    initPieCharts()
                } else if (value == 2) {
                    //getFeeAnalyseData(this.startTm, this.endTm)
                }
            },
            queryByDate() {
                this.toast = this.$toast({
                    type: 'loading',
                    mask: true,
                    message: '加载中...',
                    duration: 0
                })
                vm.closeTimeDialog()
                var promise = new Promise(resolve => getData(resolve, this.startTm, this.endTm, this.chooseDepartmentId));
                promise.then(res => {
                    const data = JSON.parse(res);
                    vm.tableData = data;
                    vm.profit = 0;
                    vm.overcome = 0;
                    vm.cost = 0;
                    vm.reimburse = 0;
                    vm.other_fee = 0;

                    vm.tableData.forEach((value, index) => {
                        vm.profit += value.profit
                        vm.overcome += value.overcome
                        vm.cost += value.cost
                        vm.reimburse += value.reimburse
                        vm.other_fee += value.other_fee
                    })
                    this.toast.clear();
                    initPieCharts();
                })
            },
            queryByDepartment() {
                this.toast = this.$toast({
                    type: 'loading',
                    mask: true,
                    message: '加载中...',
                    duration: 0
                })
                vm.closeDepartmentDialog()
                var promise = new Promise(resolve => getData(resolve, this.startTm, this.endTm, this.chooseDepartmentId));
                promise.then(res => {
                    const data = JSON.parse(res);
                    vm.tableData = data;
                    vm.profit = 0;
                    vm.overcome = 0;
                    vm.cost = 0;
                    vm.reimburse = 0;
                    vm.other_fee = 0;

                    vm.tableData.forEach((value, index) => {
                        vm.profit += value.profit
                        vm.overcome += value.overcome
                        vm.cost += value.cost
                        vm.reimburse += value.reimburse
                        vm.other_fee += value.other_fee
                    })
                    this.toast.clear();
                    initPieCharts();
                })
            },
            handleSortChange({ name, order }) {
                this.tableData = this.tableData.sort((a, b) => order === 'asc' ? a[name] - b[name] : b[name] - a[name]);
            }
        },
        mounted: function () {
            this.toast = this.$toast({
                type: 'loading',
                mask: true,
                message: '加载中...',
                duration: 0
            })
            var promise = new Promise(resolve => getData(resolve));
            promise.then(res => {
                const data = JSON.parse(res);
                vm.tableData = data;
                vm.profit = 0;
                vm.overcome = 0;
                vm.cost = 0;
                vm.reimburse = 0;
                vm.other_fee = 0;

                vm.tableData.forEach((value, index) => {
                    vm.profit += value.profit
                    vm.overcome += value.overcome
                    vm.cost += value.cost
                    vm.reimburse += value.reimburse
                    vm.other_fee += value.other_fee
                })
                this.toast.clear();
            })
        },
        computed: {
        }
    })

    function findDepartment(key, startTm) {
        if (key == '408' || key == '497' || key == '414' || key == '499') {
            alert('无下级部门')
            return
        }
        const now = new Date();
        if (!startTm || startTm == '') {
            startTm = now.getFullYear() + '-' + (now.getMonth() + 1) + '-' + now.getDate()
        }
        // 加载部门列表
        $.ajax({
            url: 'mDailyProfit.aspx',
            data: { act: 'getDepartment', key: key, startTm: startTm},
            type: 'post',
            dataType: 'json',
            success: function (res) {
                vm.toast.clear();
                if (res.length != 0) {
                    vm.departmentList = res

                    if (key != '1') {
                        vm.chooseDepartmentName = res[0].parentName;
                        vm.chooseDepartmentId = key;
                    }
                } else {
                    alert('无下级部门')
                }
            },
            error: function (res) {
                //vm.$alert('查询失败，请及时联系管理员', '提示');
            }
        })
    }

    function findParentDepartment(key) {
        // 加载部门列表
        $.ajax({
            url: 'mDailyProfit.aspx',
            data: { act: 'getParentDepartment', key: key },
            type: 'post',
            dataType: 'json',
            success: function (res) {
                vm.toast.clear();
                if (res.length != 0) {
                    vm.departmentList = res
                    vm.chooseDepartmentId = res[0].parentId
                    vm.chooseDepartmentName = res[0].parentName
                } else {
                    alert('无下级部门')
                }
            },
            error: function (res) {
                //vm.$alert('查询失败，请及时联系管理员', '提示');
            }
        })
    }

    //var myChart = echarts.init(document.getElementById('main'));
    //var myChart2 = echarts.init(document.getElementById('main2'));
    //var myChart3 = echarts.init(document.getElementById('main3'));
    //var myChart4 = echarts.init(document.getElementById('main4'));

    function getData(resolve,startTm, endTm, departmentId) {
        const now = new Date();
        if (!startTm || startTm == '') {
            startTm = now.getFullYear() + '-' + (now.getMonth()+1) + '-' + now.getDate()
        }
        if (!endTm || endTm == '') {
            endTm = now.getFullYear() + '-' + (now.getMonth()+1) + '-' + now.getDate()
        }
        if (!departmentId)
            departmentId = 1
        $.ajax({
            url: 'mDailyProfit.aspx',
            data: {
                act: 'getData',
                startTm: startTm,
                endTm: endTm,
                departmentId: departmentId
            },
            type: 'post',
            success: function (res) {
                resolve(res)
                //vm.overcome = data.overcome;
                //vm.cost = data.cost;
                //vm.reimburse = data.reimburse;
                //vm.other_fee = data.other_fee;

                //vm.subDepartmentDictList = data.subDepartmentDictList;
                //vm.subDepartmentTimeDictList = data.subDepartmentTimeDictList;
                //vm.timeList = data.timeList;
                //vm.tableData = data.subDepartmentDictList;

                //const seriesLabel = {
                //    normal: {
                //        show: true,
                //        textBorderColor: '#333',
                //        textBorderWidth: 2
                //    }
                //}

                //vm.barChartData = data.subDepartmentDictList.map(item => {
                //    return {
                //        name: item.name,
                //        type: 'bar',
                //        label: seriesLabel,
                //        data: [(item.overcome / 10000).toFixed(3), (item.cost / 10000).toFixed(3),
                //        (item.reimburse / 10000).toFixed(3), (item.other_fee / 10000).toFixed(3)]
                //    }
                //})

                //if (vm.active1 == 0) {
                //    initPieCharts();
                //} else if (vm.active1 == 1) {
                //    initLineCharts();
                //} else if (vm.active1 == 2) {
                //    initBarCharts();
                //}
            },
            error: function (res) {
                vm.$alert('查询失败，请及时联系管理员', '提示');
            }
        })
    }

    //function initLineCharts() {
    //    var option3 = {
    //        title: {
    //            text: vm.departmentName == '' ? "销售部" : vm.departmentName,
    //        },
    //        tooltip: {
    //            trigger: 'axis'
    //        },
    //        grid: {
    //            left: '3%',
    //            right: '4%',
    //            bottom: '3%',
    //            containLabel: true
    //        },
    //        toolbox: {
    //            feature: {
    //                saveAsImage: {}
    //            }
    //        },
    //        dataZoom: [
    //            {
    //                type: 'inside',
    //                realtime: true,
    //                start: 0,
    //                end: 60
    //            }
    //        ],
    //        xAxis: {
    //            type: 'category',
    //            boundaryGap: false,
    //            data: vm.timeList,
    //            axisLabel: {
    //                rotate:30
    //            }
    //        },
    //        yAxis: {
    //            type: 'value'
    //        },
    //        series: vm.subDepartmentTimeDictList
    //    };
    //    myChart3.setOption(option3);
    //}

    function initPieCharts() {
        var chart = new F2.Chart({
            id: 'mountNode',
            pixelRatio: window.devicePixelRatio,
            padding: [65, 'auto', 'auto']
        });
        chart.source(vm.tableData);
        chart.axis('value', false);
        chart.legend({
            custom: 'true',
            items: [{
                name: '净利润',
                marker: 'circle',
                fill: '#FC674D'
            }, {
                name: '负利润',
                marker: 'circle',
                fill: '#9AC2AB'
            }],
            align: 'right',
            itemWidth: null
        });
        chart.tooltip(false);
        chart.interval().position('name*profit').color('profit', function (val) {
            return val > 0 ? '#FC674D' : '#9AC2AB';
        }).size(60);

        // 辅助元素
        vm.tableData.forEach(function (obj, index) {
            // 文字部分
            var color = obj.profit > 0 ? '#FC674D' : '#9AC2AB';
            chart.guide().text({
                position: [obj.name, obj.profit > 0 ? obj.profit : 0],
                content: obj.profit,
                style: {
                    fill: color,
                    textBaseline: 'bottom'
                },
                offsetY: -5
            });
            // 背景部分
            var offset = 0.46;
            chart.guide().rect({
                start: [index - offset, 'max'],
                end: [index + offset, 'min'],
                style: {
                    fill: '#f8f8f8'
                }
            });
        });

        chart.guide().text({
            position: ['min', 'max'],
            content: '单位（万元）',
            style: {
                textBaseline: 'middle',
                textAlign: 'start'
            },
            offsetY: -50
        });

        chart.render();

        //let pieChartTable = []
        //if (vm.profit >= 0) {
        //    pieChartTable.push({ name: '成本', value: vm.cost.toFixed(3) })
        //    pieChartTable.push({ name: '费用', value: vm.reimburse.toFixed(3) })
        //    pieChartTable.push({ name: '其他', value: vm.other_fee.toFixed(3) })
        //    pieChartTable.push({ name: '利润', value: vm.profit.toFixed(3) })
        //} else {
        //    pieChartTable.push({ name: '成本', value: vm.cost.toFixed(3) })
        //    pieChartTable.push({ name: '费用', value: vm.reimburse.toFixed(3) })
        //    pieChartTable.push({ name: '其他', value: vm.other_fee.toFixed(3) })
        //}
        //var option = {
        //    title: {
        //        text: vm.chooseDepartmentName + ' 成本分析图',
        //        left: 'center',
        //        textStyle: {
        //            fontSize: 14,
        //            fontWeight: 'bold'
        //        }
        //    },
        //    tooltip: {
        //        trigger: 'item',
        //        formatter: "{b} : {c} ({d}%)"
        //    },
        //    series: [
        //        {
        //            name: '明细',
        //            type: 'pie',
        //            radius: '55%',
        //            center: ['50%', '50%'],
        //            data: pieChartTable.sort(function (a, b) { return a.value - b.value; }),
        //            itemStyle: {
        //                emphasis: {
        //                    shadowBlur: 10,
        //                    shadowOffsetX: 0,
        //                    shadowColor: 'rgba(0, 0, 0, 0.5)'
        //                }
        //            },
        //        }
        //    ]
        //};
        //myChart.setOption(option);
    }

    //function getFeeAnalyseData(startTm, endTm) {
    //    const now = new Date();
    //    if (!startTm || startTm == '') {
    //        startTm = now.getFullYear() + '-' + (now.getMonth() + 1) + '-1'
    //    }
    //    if (!endTm || endTm == '') {
    //        endTm = now.getFullYear() + '-' + (now.getMonth() + 1) + '-' + now.getDate()
    //    }
    //    $.ajax({
    //        url: 'mDailyProfit.aspx',
    //        data: {
    //            act: 'getFeeAnalyse',
    //            startTm: startTm + ' 00:00:00',
    //            endTm: endTm + ' 23:59:59',
    //            type: "0"
    //        },
    //        type: 'post',
    //        success: function (res) {
    //            vm.feeAnalyseData = JSON.parse(res)
    //            initFeeAnalyse()
    //            vm.toast.clear();
    //        }
    //    })
    //}

    //function initFeeAnalyse() {
    //    var chart = new F2.Chart({
    //        id: 'mountNode',
    //        pixelRatio: window.devicePixelRatio,
    //        padding: [20, 20, 5],
    //        height: 700
    //    });
    //    chart.source(vm.feeAnalyseData);
    //    chart.coord({
    //        transposed: true
    //    });
    //    chart.axis(false);
    //    chart.tooltip(false);
    //    chart.interval().position('name*const').color('#d9e4eb').size(10).animate(false);
    //    chart.interval().position('name*value').size(10);
    //    // 绘制文本
    //    vm.feeAnalyseData.map(function (obj) {
    //        chart.guide().text({
    //            position: [obj.name, 'min'],
    //            content: obj.name,
    //            style: {
    //                textAlign: 'start',
    //                textBaseline: 'bottom'
    //            },
    //            offsetY: -8
    //        });
    //        chart.guide().text({
    //            position: [obj.name, 'max'],
    //            content: obj.value,
    //            style: {
    //                textAlign: 'end',
    //                textBaseline: 'bottom'
    //            },
    //            offsetY: -8
    //        });
    //    });
    //    chart.render();
    //}

    //function initBarCharts() {
    //    var option4 = {
    //        title: {
    //            text: vm.departmentName == '' ? "销售部" : vm.departmentName,
    //        },
    //        tooltip: {
    //            trigger: 'axis',
    //            axisPointer: {
    //                type: 'shadow'
    //            }
    //        },
    //        legend: {
    //            data: vm.barChartData.map(item => {
    //                return item.name
    //            })
    //        },
    //        xAxis: {
    //            type: 'value',
    //            name: '利润',
    //            axisLabel: {
    //                formatter: '{value}'
    //            }
    //        },
    //        yAxis: {
    //            type: 'category',
    //            inverse: true,
    //            data: ['收入', '成本', '报销', '其他费用'],
    //        },
    //        series: vm.barChartData
    //    }
    //    myChart4.setOption(option4);
    //}
</script>
<style>
    .van-cell__label {
        font-size: 70px;
        margin-top: 40px;
        margin-bottom: 30px;
        margin-left: -5px;
        color: dodgerblue;
    }
</style>
</html>
