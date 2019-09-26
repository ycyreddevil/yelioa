<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mApprovalReimburseDetail.aspx.cs" Inherits="mApprovalReimburseDetail" %>

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
            title="待我审批"
            left-text="刷新"
            right-text="日期查询"
            @click-left="refresh"
            @click-right="query">
            <van-icon name="filter-o" slot="right" size="20px"/>
        </van-nav-bar>
        <van-loading v-show="loading" size="24px" style="padding-top:100px" vertical>数据加载中，请稍等</van-loading>
        <div v-if="list.length > 0">
            <mu-list dense v-for="temp in list">
                <mu-list-item avatar :ripple="false" button @click="showDetail(temp.BatchNo, temp.Code)">
                <mu-list-item-action>
                  <mu-avatar>
                    <img :src="temp.avatar">
                  </mu-avatar>
                </mu-list-item-action>
                <mu-list-item-content>
                  <mu-list-item-title>发票用途：{{temp.CreateTime}}</mu-list-item-title>
                  <mu-list-item-sub-title>
                    金额：{{temp.ReceiptAmount}}
                  </mu-list-item-sub-title>
                </mu-list-item-content>
                <mu-list-item-action>
                  <mu-list-item-after-text>{{temp.Status}}</mu-list-item-after-text>
                </mu-list-item-action>
              </mu-list-item>
            </mu-list>
        </div>
        <div v-else>
            <img style="width: 100%;" src="resources/norecord.png" />
        </div>
        <!--日期选择弹出框-->
        <van-popup v-model="isShowDate" position="bottom">
            <van-datetime-picker v-model="chooseDate" @confirm="confirmDate" type="date" :formatter="formatter"></van-datetime-picker>
        </van-popup>
        <!--图片预览-->
        <van-image-preview
          v-model="isImagePreviewShow"
          :images="imageList">
          <template v-slot:index>第{{ imageIndex+1 }}页</template>
        </van-image-preview>
        <!--单据详情弹出框-->
        <van-popup v-model="isShowDetail" style="width:100%;height:100%" position="right">
            <van-nav-bar
                title="明细"
                left-text="返回"
                left-arrow
                @click-left="back">
            </van-nav-bar>
            <van-collapse v-model="activeName" accordion>
              <van-collapse-item title="消费记录" name="1">
                  <mu-paper class="demo-paper" :z-depth="1" style="margin:10px" v-for="(temp,index) in reimburseList">
                    <van-cell-group>
                        <van-field v-model="temp.approval_time" readonly label="审批日期"></van-field>
                        <van-field v-model="temp.fee_department" readonly label="费用归属部门"></van-field>
                        <van-field v-model="temp.fee_company" readonly label="费用归属公司"></van-field>
                        <van-field v-model="temp.fee_detail"  readonly label="费用明细"></van-field>
                        <van-field v-model="temp.product" readonly label="产品"></van-field>
                        <van-field v-model="temp.branch" readonly label="网点"></van-field>
                        <van-field v-model="temp.project" readonly label="项目"></van-field>
                        <van-field v-model="temp.fee_amount" readonly label="金额"></van-field>
                        <van-field v-model="temp.remark" type="textarea" readonly label="备注"></van-field>
                    </van-cell-group>
                </mu-paper>
              </van-collapse-item>
              <%--<van-collapse-item title="发票图片" name="2" v-show="imageList.length > 0">
                  <van-image v-for="(temp,index) in receiptList"
                    width="100"
                    height="100"
                    fit="cover"
                    :src="temp.ReceiptAttachment" 
                    style="margin:5px"
                    @click="isImagePreviewShow = true">
                    <template v-slot:error>加载失败</template>
                </van-image>
              </van-collapse-item>--%>
              <van-collapse-item title="发票明细" name="3">
                  <mu-paper class="demo-paper" :z-depth="1" style="margin:10px" v-for="(temp,index) in receiptList">
                    <van-cell-group>
                        <van-image
                            width="100"
                            height="100"
                            fit="cover"
                            :src="temp.ReceiptAttachment" 
                            style="margin:10px"
                            @click="isImagePreviewShow = true;imageList = [];imageList.push(temp.ReceiptAttachment)">
                            <template v-slot:error>加载失败</template>
                        </van-image>
                        <van-field label="发票用途" v-model="temp.ReceiptType"></van-field>
                        <van-field v-show="temp.ReceiptType != '出差补贴'" v-model="temp.ReceiptDate" label="发票日期"></van-field>
                        <van-field v-model="temp.ActivityDate" label="费用产生日期"></van-field>
                        <van-field v-model="temp.ActivityEndDate" label="费用结束日期"></van-field>
                        <van-field v-show="temp.ReceiptType != '出差补贴'" v-model="temp.ReceiptCode"  label="发票编码"></van-field>
                        <van-field v-show="temp.ReceiptType != '出差补贴'" v-model="temp.ReceiptNum" label="发票号码"></van-field>
                        <van-field type="number" v-model="temp.ReceiptAmount" label="发票金额"></van-field>
                        <van-field v-model="temp.ReceiptPerson" label="发票人"></van-field>
                        <van-field v-model="temp.RelativePerson" label="同行人"></van-field>
                        <van-field v-show="temp.ReceiptType != '出差补贴'" type="number" v-model="temp.ReceiptTax" readonly label="发票税额"></van-field>
                        <van-field v-model="temp.ReceiptPlace" label="发票地点"></van-field>
                        <van-field v-model="temp.ReceiptDesc" type="textarea" label="活动内容描述"></van-field>
                    </van-cell-group>
                </mu-paper>
              </van-collapse-item>
            </van-collapse>
            <van-row type="flex" justify="space-around" style="padding-top:10px">
                <van-col span="10"><van-button size="large" type="primary" @click="agree">同意</van-button></van-col>
                <van-col span="10"><van-button size="large" type="danger" @click="disagreeDialog = true">拒绝</van-button></van-col>
            </van-row>
            <%--<van-divider>消费记录</van-divider>
            <mu-paper class="demo-paper" :z-depth="1" style="margin:10px" v-for="(temp,index) in reimburseList">
                <van-cell-group>
                    <van-field v-model="temp.approval_time" readonly label="审批日期"></van-field>
                    <van-field v-model="temp.fee_department" readonly label="费用归属部门"></van-field>
                    <van-field v-model="temp.fee_company" readonly label="费用归属公司"></van-field>
                    <van-field v-model="temp.fee_detail"  readonly label="费用明细"></van-field>
                    <van-field v-model="temp.product" readonly label="产品"></van-field>
                    <van-field v-model="temp.branch" readonly label="网点"></van-field>
                    <van-field v-model="temp.project" readonly label="项目"></van-field>
                    <van-field v-model="temp.fee_amount" readonly label="金额"></van-field>
                    <van-field v-model="temp.remark" type="textarea" readonly label="备注"></van-field>
                </van-cell-group>
            </mu-paper>
            <van-divider>发票图片</van-divider>
            <van-image v-for="(temp,index) in receiptList"
                width="100"
                height="100"
                fit="cover"
                :src="temp.ReceiptAttachment" 
                style="margin:5px"
                @click="isImagePreviewShow = true">
                <template v-slot:error>加载失败</template>
            </van-image>
            <van-divider>发票明细</van-divider>
            <mu-paper class="demo-paper" :z-depth="1" style="margin:10px" v-for="(temp,index) in receiptList">
                <van-cell-group>
                    <van-field readonly label="发票用途" v-model="temp.ReceiptType"></van-field>
                    <van-field v-model="temp.ReceiptDate" readonly label="发票日期"></van-field>
                    <van-field v-model="temp.ActivityDate" readonly label="活动日期"></van-field>
                    <van-field v-model="temp.ReceiptCode"  readonly label="发票编码"></van-field>
                    <van-field v-model="temp.ReceiptNum" readonly label="发票号码"></van-field>
                    <van-field type="number" v-model="temp.ReceiptAmount" readonly label="发票金额"></van-field>
                    <van-field v-model="temp.ReceiptPerson" readonly label="发票人"></van-field>
                    <van-field type="number" v-model="temp.ReceiptTax" readonly label="发票税额"></van-field>
                    <van-field v-model="temp.ReceiptPlace" readonly label="发票地点"></van-field>
                    <van-field v-model="temp.ReceiptDesc" type="textarea" readonly label="活动内容描述"></van-field>
                </van-cell-group>
            </mu-paper>--%>
        </van-popup>
        <!--审批拒绝弹出框-->
        <van-dialog
          v-model="disagreeDialog"
          title="审批拒绝"
          show-cancel-button
          @confirm="disagree">
          <van-field border type="textarea" label="拒绝理由" v-model="opinion"></van-field>
        </van-dialog>
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
            url: 'mApprovalReimburseDetail.aspx',
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
            isShowDate: false,
            chooseDate: new Date(),
            receiptList: [],
            reimburseList: [],
            isImagePreviewShow: false,
            imageList: [],
            imageIndex: 0,
            isShowDetail: false,
            activeName: '1',
            chooseCode: '',
            chooseBatchNo: '',
            disagreeDialog: false,
            opinion: ''
        },
        methods: {
            refresh() {
                location.reload()
            },
            query() {
                this.isShowDate = true
            },
            confirmDate(date) {
                this.isShowDate = false
                this.chooseDate = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate()
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
            showDetail(batchNo,code) {
                this.chooseCode = code
                this.chooseBatchNo = batchNo
                //this.imageList = []
                this.isShowDetail = true
                web({ action: 'getDetail', batchNo: batchNo, code: code }).then(res => {
                    this.receiptList = JSON.parse(res.data.result1)
                    this.reimburseList = JSON.parse(res.data.result2)
                    //this.receiptList.forEach((v, i) => {
                    //    if (v.ReceiptAttachment !== '')
                    //        this.imageList.push(v.ReceiptAttachment)
                    //})
                })
            },
            onChange(index) {
                this.imageIndex = index
            },
            back() {
                this.isShowDetail = false
            },
            agree() {
                this.$dialog.confirm({
                    title: '提示',
                    message: '确认审批同意此单据吗'
                }).then(() => {
                    // 审批同意
                    web({ action: 'agree', batchNo: this.chooseBatchNo, code: this.chooseCode, receiptList: JSON.stringify(this.receiptList) }).then(res => {
                        if (res.data.code == 200) {
                            this.$dialog.alert({
                                title: '提示',
                                message: '审批成功'
                            }).then(() => {
                                location.reload()
                            });
                        } else if (res.data.code === 400) {
                            this.$toast(res.data.msg)
                        } else {
                            this.$toast('审批失败，请稍后重试')
                        }
                    })
                }).catch(() => {})
            },
            disagree() {
                if (this.opinion === '') {
                    this.$toast('拒绝理由不能为空')
                    return
                }
                web({ action: 'disagree', batchNo: this.chooseBatchNo, code: this.chooseCode, opinion: this.opinion }).then(res => {
                    if (res.data.code == 200) {
                        this.$dialog.alert({
                            title: '提示',
                            message: '审批成功'
                        }).then(() => {
                            location.reload()
                        });
                    } else if (res.data.code === 400) {
                        this.$toast(res.data.msg)
                    } else {
                        this.$toast('审批失败，请稍后重试')
                    }
                })
            },
            goBack(){
                this.isShowDetail = false
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
            web({ action: 'getList' }).then(res => {
                this.list = res.data
                this.loading = false
            })
        }
    })
</script>
</html>
