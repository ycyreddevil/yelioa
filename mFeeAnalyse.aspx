<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mFeeAnalyse.aspx.cs" Inherits="mFeeAnalyse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>费用分析</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link href="Scripts/themes/default/easyui.css" rel="stylesheet" />
    <link rel="stylesheet" href="Scripts/themes/mobile.css">
    <link href="Scripts/themes/icon.css" rel="stylesheet" />
    <link href="Scripts/themes/color.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/vant@2.0/lib/index.css">
    <link rel="stylesheet" href="https://cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.css">
    <link rel="stylesheet" href="https://gw.alipayobjects.com/os/rmsportal/YmDAMEQVbLJpVbKiRQVX.css" />
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
</head>
<body>
    <div id="vm">
        <van-nav-bar :title="titleName" right-text="月份查询" left-text="类型查询" @click-right="timeDialog = true" @click-left="typeDialog = true"></van-nav-bar>
        <H5 v-show="type === '0' || type === '1'" style="text-align:center;margin-bottom:0px">{{startTm}} 费用总计:{{totalAmount.toFixed(3)}}(万元)</H5>
        <canvas id="mountNode" v-if="type === '0' || type === '1' || type === '3' || type === '4'"></canvas>
        <el-table :data="tableData" stripe style="width: 100%; font-size:10px" height="750" v-else>
            <el-table-column
                fixed
                prop="name"
                label="部门"
                width="110">
            </el-table-column>
            <el-table-column sortable
                prop="budget"
                label="预算"
                width="110">
            </el-table-column>
            <el-table-column sortable
                prop="real_fee"
                label="实际"
                width="110">
            </el-table-column>
            <el-table-column sortable
                prop="diff"
                label="差异数"
                width="110">
            </el-table-column>
        </el-table>
        <mu-dialog title="选择月份" width="360" :open.sync="timeDialog">
            <mu-date-input value-format="YYYY-MM" v-model="startTm" label="月份" type="month" label-float full-width></mu-date-input>
            <mu-button slot="actions" flat color="primary" @click="queryByDate">确定</mu-button>
            <mu-button slot="actions" flat color="danger" @click="timeDialog = false">取消</mu-button>
        </mu-dialog>
        <van-action-sheet v-model="typeDialog" :actions="allAnalyseType" @select="onSelect" cancel-text="取消" @cancel="typeDialog = false"/>
    </div>
</body>
<script src="Scripts/jquery.min.js"></script>
<script src="Scripts/mobileCommon.js"></script>
<script src="Scripts/vue.js"></script>
<script src="https://cdn.jsdelivr.net/npm/vant@2.0/lib/vant.min.js"></script>
<script src="https://unpkg.com/muse-ui/dist/muse-ui.js"></script>
<script src="https://gw.alipayobjects.com/os/antv/assets/f2/3.3.8/f2.min.js"></script>
<script src="https://gw.alipayobjects.com/os/antv/assets/lib/jquery-3.2.1.min.js"></script>
<script src="https://gw.alipayobjects.com/os/rmsportal/NjNldKHIVQRozfbAOJUW.js"></script>
<script src="https://unpkg.com/element-ui/lib/index.js"></script>
<script>
    var vm = new Vue({
        el: '#vm',
        data: {
            timeDialog: false,
            typeDialog: false,
            allAnalyseType: [
                {
                    name: '部门费用占比'
                },
                {
                    name: '费用科目占比'
                },
                {
                    name: '预算执行差异'
                },
                {
                    name: '总费用率分析'
                },
                {
                    name: '总毛利率分析'
                }
            ],
            feeAnalyseData: [],
            startTm: '',
            endTm: '',
            toast: '',
            totalAmount: 0,
            type: '3',
            titleName: '部门费用占比',
            tableData: [],
        },
        methods: {
            onSelect(item) {
                this.typeDialog = false;
                this.titleName = item.name
                if (item.name == '部门费用占比') {
                    this.type = '0'
                } else if (item.name == '费用科目占比') {
                    this.type = '1'
                } else if (item.name == '预算执行差异') {
                    this.type = '2'
                } else if (item.name == '总费用率分析') {
                    this.type = '3'
                } else if (item.name == '总毛利率分析') {
                    this.type = '4'
                }
                this.toast = this.$toast({
                    type: 'loading',
                    mask: true,
                    message: '加载中...',
                    duration: 0
                })
                this.totalAmount = 0.0
                getFeeAnalyseData(this.startTm, this.type)
            },
            queryByDate() {
                this.totalAmount = 0;
                this.timeDialog = false;
                this.toast = this.$toast({
                    type: 'loading',
                    mask: true,
                    message: '加载中...',
                    duration: 0
                })
                getFeeAnalyseData(this.startTm, this.type)
            }
        },
        mounted: function () {
            this.toast = this.$toast({
                type: 'loading',
                mask: true,
                message: '加载中...',
                duration: 0
            })
            getFeeAnalyseData(this.startTm, this.type)
        }
    })

    function getFeeAnalyseData(startTm, type) {
        const now = new Date();
        if (!startTm || startTm == '') {
            let year = now.getFullYear()
            let month = now.getMonth() + 1
            month = month < 10 ? ('0' + month) : month
            startTm = year + '-' + month
        }
        $.ajax({
            url: 'mFeeAnalyse.aspx',
            data: {
                act: 'getFeeAnalyse',
                startTm: startTm,
                type: type
            },
            type: 'post',
            success: function (res) {
                vm.toast.clear();
                let data = JSON.parse(res);
                if (data.length == 0) {
                    alert('暂无数据')
                    return;
                }
                
                if (type === '2') {
                    vm.tableData = data
                    initFeeTable()
                }
                //else if (type === '3' || type === '4') {
                //    vm.feeAnalyseData = data
                //    initType3AndType4()
                //}
                else {
                    vm.feeAnalyseData = data.sort((a, b) => {
                        return a.value - b.value;
                    })
                    vm.feeAnalyseData.forEach((v, i) => {
                        vm.totalAmount += v.value
                    })
                    initType1AndType2()
                }
            }
        })
    }

    function initType1AndType2() {
        const chart = new F2.Chart({
            id: 'mountNode',
            pixelRatio: window.devicePixelRatio,
            padding: [20, 20, 5],
            height: 700
        });
        chart.source(vm.feeAnalyseData);
        chart.coord({
            transposed: true
        });
        chart.axis(false);
        chart.tooltip(false);
        chart.interval().position('name*const').color('#d9e4eb').size(10).animate(false);
        chart.interval().position('name*value').size(10);
        // 绘制文本
        vm.feeAnalyseData.map(function (obj) {
            chart.guide().text({
                position: [obj.name, 'min'],
                content: obj.name,
                style: {
                    textAlign: 'start',
                    textBaseline: 'bottom'
                },
                offsetY: -8
            });
            chart.guide().text({
                position: [obj.name, 'max'],
                content: numberAndPerfect(obj.value),
                style: {
                    textAlign: 'end',
                    textBaseline: 'bottom'
                },
                offsetY: -8
            });
        });
        chart.render();
    }

    function numberAndPerfect(n) {
        return n + '(' + ((n / vm.totalAmount) * 100).toFixed(3) + '%)';
    }
</script>
</html>
