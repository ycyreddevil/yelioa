<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mMySubmittedReimburseDetail.aspx.cs" Inherits="mMySubmittedReimburseDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta http-equiv="Access-Control-Allow-Origin" content="*" />
    <meta name="description" content="" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/vant@2.0/lib/index.css">
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <style>
        [v-cloak] {
            display: none;
        }
    </style>
</head>
<body>
    <div id="title" v-cloak>
        <van-nav-bar
            title="我已提交"
            left-text="刷新"
            right-text="日期查询"
            @click-left="refresh"
            @click-right="query">
            <van-icon name="filter-o" slot="right" size="20px"/>
        </van-nav-bar>
        <van-loading v-show="loading" size="24px" style="padding-top:100px" vertical>数据加载中，请稍等</van-loading>
        <div v-if="list.length > 0">
            <mu-list dense v-for="temp in list">
                <mu-list-item avatar :ripple="false" button @click="showDetail(temp.BatchNo)">
                <mu-list-item-action>
                  <mu-avatar>
                    <img :src="temp.avatar">
                  </mu-avatar>
                </mu-list-item-action>
                <mu-list-item-content>
                  <mu-list-item-title>提交时间：{{temp.CreateTime}}</mu-list-item-title>
                  <mu-list-item-sub-title>
                    金额：{{temp.ReceiptAmount}}
                  </mu-list-item-sub-title>
                </mu-list-item-content>
                <mu-list-item-action>
                    <mu-list-item-after-text v-if="temp.Status == '拒绝'" style="color: orangered">{{temp.Status}}</mu-list-item-after-text>
                    <mu-list-item-after-text v-else-if="temp.Status == '同意'" style="color: limegreen">{{temp.Status}}</mu-list-item-after-text>
                    <mu-list-item-after-text v-else>{{temp.Status}}</mu-list-item-after-text>
                </mu-list-item-action>
              </mu-list-item>
            </mu-list>
        </div>
        <div v-else>
            <img style="width: 100%;" src="resources/norecord.png" />
        </div>
        <!--单据详情弹出框-->
        <van-popup v-model="isShowDetail" style="width:100%;height:100%" position="right">
            <van-nav-bar
                title="明细"
                left-text="返回"
                left-arrow
                @click-left="back">
            </van-nav-bar>
            <van-divider v-show="imageList.length > 0">发票图片</van-divider>
            <van-image v-show="imageList.length > 0" v-for="(temp,index) in receiptList"
                width="100"
                height="100"
                fit="cover"
                :src="temp.ReceiptAttachment" 
                style="margin:5px"
                @click="isImagePreviewShow = true; imageList = []; imageList.push(temp.ReceiptAttachment)">
                <template v-slot:error>加载失败</template>
            </van-image>
            <van-divider>发票明细</van-divider>
            <mu-paper class="demo-paper" :z-depth="1" style="margin:10px" v-for="(temp,index) in receiptList">
                <van-cell-group>
                    <van-field readonly label="发票用途" v-model="temp.ReceiptType"></van-field>
                    <van-field v-show="temp.ReceiptType != '出差补贴'" v-model="temp.ReceiptDate" readonly label="发票日期"></van-field>
                    <van-field v-model="temp.ActivityDate" readonly label="费用发生日期"></van-field>
                    <van-field v-model="temp.ActivityEndDate" readonly label="费用结束日期"></van-field>
                    <van-field v-show="temp.ReceiptType != '出差补贴'" v-model="temp.ReceiptCode"  readonly label="发票编码"></van-field>
                    <van-field v-show="temp.ReceiptType != '出差补贴'" v-model="temp.ReceiptNum" readonly label="发票号码"></van-field>
                    <van-field type="number" v-model="temp.ReceiptAmount" readonly label="发票金额"></van-field>
                    <van-field v-model="temp.ReceiptPerson" readonly label="发票人"></van-field>
                    <van-field v-model="temp.RelativePerson" readonly label="同行人"></van-field>
                    <van-field v-show="temp.ReceiptType != '出差补贴'" type="number" v-model="temp.ReceiptTax" readonly label="发票税额"></van-field>
                    <van-field v-model="temp.ReceiptPlace" readonly label="发票地点"></van-field>
                    <van-field v-model="temp.ReceiptDesc" type="textarea" readonly label="活动内容描述"></van-field>
                </van-cell-group>
            </mu-paper>
            <van-row type="flex" justify="space-around" style="padding-top:10px" v-show="isReSubmit">
                <van-col span="10"><van-button size="large" type="info" @click="reSubmit">重新提交</van-button></van-col>
            </van-row>
        </van-popup>
        <!--图片预览-->
        <van-image-preview
          v-model="isImagePreviewShow"
          :images="imageList"
          :start-position ="imageIndex"
          @change="onChange">
          <template v-slot:index>第{{ imageIndex+1 }}页</template>
        </van-image-preview>
        <!--日期选择弹出框-->
        <van-popup v-model="isShowDate" position="bottom">
            <van-datetime-picker v-model="chooseDate" @confirm="confirmDate" type="date" :formatter="formatter"></van-datetime-picker>
        </van-popup>
    </div>
</body>
<script src="https://cdn.jsdelivr.net/npm/vue/dist/vue.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/vant@2.0/lib/vant.min.js"></script>
<script src="https://unpkg.com/axios/dist/axios.min.js"></script>
<script src="https://unpkg.com/muse-ui/dist/muse-ui.js"></script>
<script src="https://cdn.jsdelivr.net/npm/@vant/touch-emulator"></script>
<script>
    var web = function (data) {
        const params = new URLSearchParams()

        Object.keys(data).forEach((v, i) => {
            params.append(v, data[v])
        })

        return axios({
            method: 'post',
            url: 'mMySubmittedReimburseDetail.aspx',
            data: params,
            responseType: 'json',
            withCredentials: true
        })
    }

    var vue = new Vue({
        el: '#title',
        data: {
            list: [],
            loading: true,
            isShowDetail: false,
            receiptList: [],
            isImagePreviewShow: false,
            imageList: [],
            imageIndex: 0,
            isShowDate: false,
            chooseDate: new Date(),
            isReSubmit: false,
            chooseBatchNo: ''
        },
        methods: {
            showDetail(temp) {
                //this.imageList = []
                this.isShowDetail = true
                this.chooseBatchNo = temp
                web({ action: 'getDetail', batchNo: temp }).then(res => {
                    this.receiptList = res.data
                    if (res.data[0].Status === '拒绝')
                        this.isReSubmit = true
                    else
                        this.isReSubmit = false
                    //res.data.forEach((v, i) => {
                    //    if (v.ReceiptAttachment !== '')
                    //        this.imageList.push(v.ReceiptAttachment)
                    //})
                })
            },
            refresh() {
                location.reload()
            },
            query() {
                this.isShowDate = true
            },
            back() {
                this.isShowDetail = false
            },
            onChange(index) {
                this.imageIndex = index
            },
            confirmDate(date) {
                this.isShowDate = false
                this.chooseDate = date.getFullYear() + '-' + (date.getMonth()+1) + '-' + date.getDate()
                this.loading = true
                web({ action: 'getList', date: this.chooseDate }).then(res => {
                    this.list = res.data
                    this.loading = false
                    this.chooseDate = new Date()
                })
            },
            formatter(type, value) {
                if (type === 'year') {
                    return `${value}年`;
                } else if (type === 'month') {
                    return `${value}月`
                } else {
                    return `${value}日`
                }
            },
            goBack() {
                this.isShowDetail = false
            },
            reSubmit() {
                location.href = "mFinanceReimburseDetail.aspx?batchNo=" + this.chooseBatchNo
            }
        },
        destroyed(){
            window.removeEventListener('popstate', this.goBack, false);
        },
        mounted: function () {
            history.pushState(null, null, document.URL);
            window.addEventListener('popstate', function () {
                history.pushState(null, null, document.URL);
            });
            web({action: 'getList'}).then(res => {
                this.list = res.data
                this.loading = false
            })
        }
    })
</script>
</html>
