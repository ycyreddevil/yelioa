<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mFinanceReimburseDetail.aspx.cs" Inherits="mFinanceReimburseDetail" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta http-equiv="Access-Control-Allow-Origin" content="*" />
    <meta name="description" content="" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/vant@2.2/lib/index.css">
    <link rel="stylesheet" href="https://unpkg.com/muse-ui/dist/muse-ui.css">
    <link rel="stylesheet" href="https://cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.css">
    <style>
        .remainFeeAmount{
            margin: 10px auto
        }
        [v-cloak] {
            display: none;
        }
    </style>
</head>
<body>
    <div id="title" v-cloak>
        <van-nav-bar
            :title="`${current_month}月发票上传`"
            left-text="刷新"
            right-text="提交"
            @click-left="refresh"
            @click-right="submit">
        </van-nav-bar>
        <%--<van-cell-group>
            <van-cell title="选择发票用途" :value="reimburseType" is-link @click="showReimburseType"/>
        </van-cell-group>--%>
        <mu-paper class="demo-paper" :z-depth="1" style="margin:10px 0">
            <van-cell-group>
                <van-cell title="选择报销单据" :value="`已勾选${chooseReimburseCode.length}张报销单`" is-link @click="showReimburseList"/>
            </van-cell-group>
            <van-cell-group>
                <van-field v-model="receiptDesc" type="textarea" required label="活动描述" clearable autosize
                    placeholder="差旅费活动描述填出差即可；活动的活动描述填对应的活动类型；会议培训的活动描述填poa会议、小组会、战区会等类型；"></van-field>
            </van-cell-group>
            <van-cell-group>
                <van-cell v-if="allowanceMap === null" title="选择发票上传" value="新增出差补贴" @click="appendAllowance">
                    <van-icon
                        slot="right-icon"
                        name="add-o"
                        style="line-height: inherit;padding-left:10px">
                    </van-icon>
                </van-cell>
                <van-cell v-else title="选择发票上传" value="删除出差补贴" @click="removeAllowance">
                    <van-icon
                        slot="right-icon"
                        name="close"
                        style="line-height: inherit;padding-left:10px">
                    </van-icon>
                </van-cell>
                <van-uploader style="margin-left:10px" multiple :before-read="beforeRead" :after-read="afterRead" :preview-image="previewImage"/>
            </van-cell-group>
        </mu-paper>
        <van-divider>发票明细</van-divider>
        <!-- 发票明细-->
        <mu-paper class="demo-paper" :z-depth="1" style="margin:15px" v-for="(temp,index) in receiptList">
            <van-collapse v-model="activeName" accordion>
                <van-collapse-item :title="`${temp.feeType} ${temp.receiptAmount}元`" :name="index">
                    <van-cell-group>
                        <van-row v-show="temp.receiptType != '实报实销'">
                            <van-col span="8">
                                <van-image @click="isImagePreviewShow = true; photoList = [];photoList.push(temp.receiptAttachment);location.hash='xxx'" style="margin-left:10px" width="100" height="100" fit="contain" :src="temp.receiptAttachment">
                                    <template v-slot:loading>
                                        <van-loading type="spinner" size="20" />
                                    </template>
                                </van-image>
                            </van-col>
                            <van-col span="8" offset="8"><mu-button @click="isDeleteDetail = true; clickIndex = index" style="float:right" icon color="red"><mu-icon value="delete"></mu-icon></mu-button></van-col>
                        </van-row>
                        <van-field v-show="temp.receiptType != '实报实销'" required readonly label="发票用途" v-model="temp.receiptType" @click="showReimburseType(index)"></van-field>
                
                        <van-field v-show="temp.receiptType != '实报实销'" v-model="temp.receiptDate" clearable required label="发票日期"></van-field>
                
                        <van-field required v-model="temp.activityDate" readonly label="费用发生日期" @click="showDate(index)" 
                            right-icon="question-o" @click-right-icon="$toast('出差补贴必须精确到小时分钟，其他精确到日期即可')"></van-field>
                
                        <van-field required v-show="temp.receiptType == '实报实销' || temp.receiptType == '住宿费'" v-model="temp.activityEndDate" readonly label="费用结束日期" @click="showEndDate(index)" 
                            right-icon="question-o" @click-right-icon="$toast('出差补贴必须精确到小时分钟，其他精确到日期即可')"></van-field>
                
                        <van-field v-show="temp.receiptType != '实报实销'" v-model="temp.receiptCode" clearable required label="发票代码" 
                            right-icon="question-o" @click-right-icon="$toast('请核对识别的发票代码是否正确，若不正确，请修改，否则无法正常提交')"></van-field>

                        <van-field v-show="temp.receiptType != '实报实销'" v-model="temp.receiptNum" clearable required label="发票号码" 
                            right-icon="question-o" @click-right-icon="$toast('请核对识别的发票号码是否正确，若不正确，请修改，否则无法正常提交')"></van-field>
                
                        <van-field v-show="temp.feeType && temp.feeType.indexOf('增值税') > -1" v-model="temp.sellerRegisterNum" clearable required label="纳税人识别号" 
                            right-icon="question-o" @click-right-icon="$toast('请确定纳税人识别号与所选公司匹配，若不正确，请修改，否则无法正常提交')"></van-field>
                
                        <van-field type="number" v-model="temp.receiptAmount" required :label="temp.receiptType != '实报实销' ? '发票金额' : '补贴金额'"></van-field>

                        <van-field v-model="temp.receiptPerson" v-show="temp.receiptType != '实报实销'" clearable required label="发票人" 
                            right-icon="question-o" @click-right-icon="$toast('除实名制火车票、飞机票、汽车票，其余都填无即可')" placeholder="除实名制火车飞机汽车票，其余填无"></van-field>
                
                        <van-field v-show="temp.receiptType != '实报实销'" v-model="temp.relativePerson" clearable required label="同行人"
                            right-icon="question-o" @click-right-icon="$toast('没有填无')" placeholder="没有填无"></van-field>
                        <%--<van-field v-show="temp.receiptType != '出差补贴'" type="number" readonly v-model="temp.receiptTax" required label="发票税额" 
                            right-icon="question-o" @click-right-icon="$toast('税额不允许修改')"></van-field>--%>
                        <van-field v-model="temp.receiptPlace" placeholder="请输入省份和城市" required :label="temp.receiptType == '实报实销' ? '出差地' : '发票地'" right-icon="question-o" 
                            @click-right-icon="$toast('住宿费必须输入XX省XX市或XX省XX市XX县，否则无法提交')"></van-field>
                    </van-cell-group>
                </van-collapse-item>
            </van-collapse>
        </mu-paper>
        <!--勾选报销单据弹出框 -->
        <van-popup v-model="isShowReimburseList" position="bottom" style="height:300px">
            <van-checkbox-group v-model="chooseReimburseCode">
                <van-cell-group>
                    <van-cell
                      v-for="(selfReimburse,index) in selfReimburseList"
                      clickable
                      value-class="remainFeeAmount"
                      :key="selfReimburse.id"
                      :value="`剩余金额:${selfReimburse.remain_fee_amount}`"
                      :title="`${selfReimburse.LMT.split(' ')[0]} ${selfReimburse.remark}`"
                      @click="toggle(index, selfReimburse.remain_fee_amount)">
                      <van-checkbox
                        :name="selfReimburse.code"
                        ref="checkboxes"
                        slot="right-icon"
                      />
                    </van-cell>
                </van-cell-group>
            </van-checkbox-group>
        </van-popup>
        <!--选择发票用途弹出框-->
        <van-popup v-model="isShowReimburseType" position="right">
            <van-list
              :finished="finished"
              finished-text="没有更多了">
              <van-cell
                v-for="temp in reimburseTypeList"
                clickable
                :key="temp"
                :title="temp"
                @click="chooseReimburseType(temp)"
              />
            </van-list>
        </van-popup>
        <!-- 日期选择弹出框-->
        <van-popup v-model="isShowDate" position="bottom">
            <van-datetime-picker v-model="nowDate" @cancel="cancelDate" @confirm="confirmDate" type="datetime" :formatter="formatter"></van-datetime-picker>
        </van-popup>
        <van-popup v-model="isShowEndDate" position="bottom">
            <van-datetime-picker v-model="nowDate" @cancel="cancelEndDate" @confirm="confirmEndDate" type="datetime" :formatter="formatter"></van-datetime-picker>
        </van-popup>
        <!--图片预览-->
        <van-image-preview close-on-popstate 
          v-model="isImagePreviewShow"
          :images="photoList">
          <template v-slot:index>第1页</template>
        </van-image-preview>
        <!--明细删除 -->
        <van-dialog
          v-model="isDeleteDetail"
          title="提示"
          message="确定是否删除该明细吗？"
          show-cancel-button
          @confirm="deleteDetail" @cancel="isDeleteDetail = false"
        >
        </van-dialog>
    </div>
</body>
<script src="https://cdn.jsdelivr.net/npm/vue/dist/vue.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/vant@2.2/lib/vant.min.js"></script>
<script src="https://unpkg.com/axios/dist/axios.min.js"></script>
<script src="https://unpkg.com/muse-ui/dist/muse-ui.js"></script>
<script src="https://cdn.jsdelivr.net/npm/@vant/touch-emulator"></script>
<script src="Scripts/exif.js"></script>
<script>
    var web = function (data) {
        const params = new URLSearchParams()

        Object.keys(data).forEach((v, i) => {
            params.append(v, data[v])
        })

        return axios({
            method: 'post',
            url: 'mFinanceReimburseDetail.aspx',
            data: params,
            responseType: 'json',
            withCredentials: true
        })
    }

    var GetQueryString = function(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }

    //var constPhoteList = []
    const salesReimburseDetail = ["出差车船费","住宿费","出差补贴","餐费", "市内交通费", "会议费", "培训费", "办公用品", "工作餐", "场地费", "招待餐费", "纪念品", "外协劳务", "外部人员机票/火车票", "外部人员住宿费", "外部人员交通费", "学术会费", "营销办公费"]
    const notSalesReimburseDetail = ["出差车船费","住宿费", "出差补贴","交通费", "汽车使用费","业务招待费", "培训费", "办公费", "租赁费", "劳保费", "通讯费", "福利费", "物业费", "水电费", "招聘费", "运输费", "材料费", "试验费", "检测费", "专利费", "注册费", "服务费", "其他"]

    var vue = new Vue({
        el: '#title',
        data: {
            activeName: '',
            isDeleteDetail: false,
            isImagePreviewShow: false,
            previewImage: false,
            isShowReimburseList: false,
            isShowReimburseType: false,
            isShowDate: false,
            isShowEndDate: false,
            finished: true,
            nowDate: new Date(),
            selfReimburseList: [],
            chooseReimburseCode: [],
            reimburseTypeList: [],
            reimburseAmount: 0,
            photoList: [],
            clickIndex: 0,
            receiptList: [],
            chooseBatchNo: '',
            allowanceMap: null,
            orientation: null,
            chooseReimburseCompany: '',
            receiptDesc: '',
            current_month: new Date().getDate() < 6 ? new Date().getMonth() : new Date().getMonth() + 1,
        },
        methods: {
            refresh() {
                location.reload()
            },
            submit() {
                if (this.chooseReimburseCode.length === 0) {
                    this.$toast('请先选择关联的单据号')
                    return
                } else if (this.receiptDesc === '') {
                    this.$toast('请填写描述')
                    return
                }

                let totalAmount = 0
                for (let temp of this.receiptList) {
                    let flag = true
                    let msg = ''
                    /**Object.keys(temp).forEach((v, i) => {
                        if (temp[v] === '') {
                            msg = v+'不合法,请重新确认！'
                            flag = false
                            return
                        }
                    })
                    if (!flag) {
                        this.$toast(msg)
                        return
                    }*/
                    if (temp['feeType'] != '火车票' && temp['feeType'] != '飞机票') {
                        if (temp['receiptPlace'].indexOf('省') === -1 && (temp['receiptPlace'] !== '北京市'
                            && temp['receiptPlace'] !== '上海市' && temp['receiptPlace'] !== '天津市' && temp['receiptPlace'] !== '重庆市')) {
                            msg = '发票地址必须包含省份'
                            flag = false
                        } else if (temp['receiptPlace'].indexOf('市') === -1 && temp['receiptType'] === '住宿费') {
                            msg = '住宿费发票地址必须包含省份和城市'
                            flag = false
                        }
                    }
                    if (!flag) {
                        this.$toast(msg)
                        return
                    }
                        
                    totalAmount += parseFloat(temp["receiptAmount"])
                }
                if (totalAmount > this.reimburseAmount) {
                    this.$toast('发票金额不能高于单据金额')
                    return
                }
                this.$toast.loading({
                    mask: true,
                    message: '加载中...',
                    duration: 0
                });
                web({
                    action: 'submit',
                    batchNo: this.chooseBatchNo,
                    code: JSON.stringify(this.chooseReimburseCode),
                    receipt: JSON.stringify(this.receiptList),
                    receiptDesc: this.receiptDesc
                }).then(res => {
                    this.$toast.clear();
                    if (res.data.code === 200) {
                        this.$dialog.alert({
                            title: '提示',
                            message: '提交成功'
                        }).then(() => {
                            location.href = "mMySubmittedReimburseDetail.aspx"
                        });
                    }
                    else if (res.data.code === 400) {
                        this.$toast(res.data.msg)
                    }
                    else {
                        this.$toast('提交失败，请稍后重试')
                    }
                })
            },
            toggle(index, amount) {
                if (this.chooseReimburseCompany === '')
                    this.chooseReimburseCompany = this.selfReimburseList[index].fee_company

                if (!this.$refs.checkboxes[index].checked) {
                    // 判断已勾选的报销单是否开票单位一致
                    if (this.selfReimburseList[index].fee_company != this.chooseReimburseCompany) {
                        this.$toast('请关联开票单位一致的报销单！')
                        return
                    }
                    this.reimburseAmount += parseFloat(amount)
                }
                else {
                    this.reimburseAmount -= parseFloat(amount)
                    this.chooseReimburseCompany = ''
                }

                this.$refs.checkboxes[index].toggle();
            },
            showReimburseList(temp) {
                web({ action: 'getSelfReimburseList' }).then(res => {
                    if (res.data.length === 0)
                        this.$toast('暂无可用报销单据');
                    else
                        this.isShowReimburseList = true
                    this.selfReimburseList = res.data
                })
            },
            showReimburseType(index) {
                this.clickIndex = index
                this.isShowReimburseType = true
                web({action: 'getSalesOrNotSales'}).then(res => {
                    if (res.data.name.indexOf('营销中心') > -1)
                        this.reimburseTypeList = salesReimburseDetail
                    else
                        this.reimburseTypeList = notSalesReimburseDetail
                })
            },
            chooseReimburseType(data) {
                this.isShowReimburseType = false
                this.receiptList[this.clickIndex]["receiptType"] = data
            },
            showDate(index) {
                this.clickIndex = index
                this.isShowDate = true
            },
            showEndDate(index) {
                this.clickIndex = index
                this.isShowEndDate = true
            },
            confirmDate(date) {
                this.isShowDate = false
                this.receiptList[this.clickIndex]["activityDate"] = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate() + ' '
                    + date.getHours() + ":" + date.getMinutes() + ":" + "00"
            },
            confirmEndDate(date) {
                this.isShowEndDate = false
                this.receiptList[this.clickIndex]["activityEndDate"] = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate() + ' '
                    + date.getHours() + ":" + date.getMinutes() + ":" + "00"
            },
            cancelDate() {
                this.isShowDate = false
            },
            cancelEndDate() {
                this.isShowEndDate = false
            },
            formatter(type, value) {
                if (type === 'year') {
                    return `${value}年`;
                } else if (type === 'month') {
                    return `${value}月`
                } else if (type === 'day') {
                    return `${value}日`
                } else if (type === 'hour') {
                    return `${value}时`
                } else {
                    return `${value}分`
                }
            },
            //deleteImage(file) {
            //    if (this.photoList.length === 0) {
            //        this.receiptList = []
            //    } else {
            //        for (let i = 0; i < constPhoteList.length; i ++) {
            //            if (constPhoteList[i] == file) {
            //                this.receiptList.splice(i, 1)
            //                break
            //            }
            //        }
            //    }

            //    constPhoteList = this.photoList
            //},
            beforeRead(file) {
                this.$toast.loading({
                    mask: true,
                    message: '发票识别中...',
                    duration: 0
                });

                if (file.type && file.type.indexOf('image') == -1) {
                    this.$toast('请勿上传非图片文件');
                    return false;
                } else if (file.length > 1) {
                    for (let temp of file) {
                        if (temp.type.indexOf('image') == -1) {
                            this.$toast('请勿上传非图片文件');
                            return false
                        }
                    }
                }

                return true;
            },
            afterRead(file) {
                let fileList = []
                let fileNameList = []

                // 图片上传
                if (file.content) {
                    fileList.push(file.content)
                    fileNameList.push(file.file.name)
                }
                else {
                    for (let temp of file) {
                        fileList.push(temp.content)
                        fileNameList.push(temp.file.name)
                    }
                }
                //constPhoteList = this.photoList
                web({ action: 'uploadFile', files: JSON.stringify(fileList), fileNames: JSON.stringify(fileNameList) }).then(res => {
                    const result = res.data

                    if (result.message) {
                        this.$toast.clear();
                        this.$toast(result.message)
                        return
                    }

                    for (let temp of result) {
                        const url = temp.url
                        const info = temp.details

                        const tempData = {}
                        const templateType = temp.type
                        //let array = {}
                        //info.data.ret.forEach((value, index) => {
                        //    array[value["word_name"]] = value["word"]
                        //})
                        tempData['relativePerson'] = '无'
                        tempData["receiptAttachment"] = url
                        if (templateType == "10100" || templateType == "10101" || templateType == "10102" || templateType == "10103") {  // 增值税发票
                            if (templateType == "10100")
                                tempData['feeType'] = '增值税专用发票'
                            else if (templateType == "10101")
                                tempData['feeType'] = '增值税普通发票'
                            else if (templateType == "10102")
                                tempData['feeType'] = '增值税电子普通发票'
                            else if (templateType == "10103")
                                tempData['feeType'] = '增值税普通发票(卷票)'

                            tempData['receiptDate'] = info.date;
                            tempData['receiptCode'] = info.code;
                            tempData['receiptAmount'] = info.total;
                            if (templateType == "10102" && (info.item_names.indexOf('客运服务费') > -1 || info.item_names.indexOf('通行费') > -1))
                                tempData['receiptTax'] = info.tax;
                            else if (templateType == "10100")
                                tempData['receiptTax'] = info.tax;
                            else
                                tempData['receiptTax'] = 0;
                            tempData['receiptPlace'] = info.province+info.city;
                            tempData['receiptNum'] = info.number;
                            tempData['receiptPerson'] = '<%=userInfo.userName%>'
                            tempData['activityDate'] = ''
                            tempData['activityEndDate'] = ''
                            tempData['sellerRegisterNum'] = info.buyer_tax_id
                        } else if (templateType == "10503") {
                            tempData['feeType'] = '火车票'
                            tempData['receiptDate'] = info.date;
                            tempData['receiptCode'] = info.number;
                            tempData['receiptAmount'] = info.total
                            tempData['receiptTax'] = 0;
                            tempData['receiptPlace'] = info.station_geton + '-' + info.station_getoff;
                            tempData['receiptNum'] = info.number;
                            tempData['receiptPerson'] = info.name
                            tempData['activityDate'] = ''
                            tempData['activityEndDate'] = ''
                            tempData['sellerRegisterNum'] = ''
                        } else if (templateType == "10500") {
                            tempData['feeType'] = '出租车票'
                            tempData['receiptDate'] = info.date;
                            tempData['receiptCode'] = info.code;
                            tempData['receiptAmount'] = info.total;
                            tempData['receiptTax'] = 0;
                            tempData['receiptPlace'] = info.place
                            tempData['receiptNum'] = info.number;
                            tempData['receiptPerson'] = '<%=userInfo.userName%>'
                            tempData['activityDate'] = ''
                            tempData['activityEndDate'] = ''
                            tempData['sellerRegisterNum'] = ''
                        } else if (templateType == "10400") {
                            tempData['feeType'] = '通用机打发票'
                            tempData['receiptDate'] = info.date;
                            tempData['receiptCode'] = info.code;
                            tempData['receiptAmount'] = info.total;
                            tempData['receiptTax'] = 0;
                            tempData['receiptPlace'] = info.province + info.city
                            tempData['receiptNum'] = info.number;
                            tempData['receiptPerson'] = '<%=userInfo.userName%>'
                            tempData['activityDate'] = ''
                            tempData['activityEndDate'] = ''
                            tempData['sellerRegisterNum'] = ''
                        } else if (templateType == "10506") {
                            tempData['feeType'] = '飞机票'
                            tempData['receiptDate'] = info.date;
                            tempData['receiptCode'] = info.number;
                            tempData['receiptAmount'] = parseFloat(info.fare) + parseFloat(info.fuel_surcharge);
                            tempData['receiptTax'] = 0;
                            tempData['receiptPlace'] = info.flights[0].from + '-' + info.flights[0].to
                            tempData['receiptNum'] = info.number;
                            tempData['receiptPerson'] = info.user_name
                            tempData['activityDate'] = ''
                            tempData['activityEndDate'] = ''
                            tempData['sellerRegisterNum'] = ''
                        } else if (templateType == "10200") {
                            tempData['feeType'] = '定额发票'
                            tempData['receiptDate'] = ''
                            tempData['receiptCode'] = info.code;
                            tempData['receiptAmount'] = info.total;
                            tempData['receiptTax'] = 0;
                            tempData['receiptPlace'] = info.province + info.city
                            tempData['receiptNum'] = info.number;
                            tempData['receiptPerson'] = '<%=userInfo.userName%>'
                            tempData['activityDate'] = ''
                            tempData['activityEndDate'] = ''
                            tempData['sellerRegisterNum'] = ''
                        } else if (templateType == "10505") {
                            // 汽车票
                            tempData['feeType'] = '汽车票'
                            tempData['receiptDate'] = info.date;
                            tempData['receiptCode'] = info.code;
                            tempData['receiptAmount'] = info.total;
                            tempData['receiptTax'] = 0;
                            tempData['receiptPlace'] = info.station_geton + '-' + info.station_getoff;
                            tempData['receiptNum'] = info.number;
                            tempData['receiptPerson'] = info.name;
                            tempData['activityDate'] = ''
                            tempData['activityEndDate'] = ''
                            tempData['sellerRegisterNum'] = ''
                        } else if (templateType == "10507") {
                            // 汽车票
                            tempData['feeType'] = '过桥过路票'
                            tempData['receiptDate'] = info.date
                            tempData['receiptCode'] = info.code;
                            tempData['receiptAmount'] = info.total;
                            tempData['receiptTax'] = 0;
                            tempData['receiptPlace'] = ''
                            tempData['receiptNum'] = info.number;
                            tempData['receiptPerson'] = '<%=userInfo.userName%>'
                            tempData['activityDate'] = ''
                            tempData['activityEndDate'] = ''
                            tempData['sellerRegisterNum'] = ''
                        } else {
                            tempData['feeType'] = '其他票'
                            tempData['receiptDate'] = ''
                            tempData['receiptCode'] = ''
                            tempData['receiptAmount'] = ''
                            tempData['receiptTax'] = ''
                            tempData['receiptPlace'] = ''
                            tempData['receiptNum'] = ''
                            tempData['receiptPerson'] = '<%=userInfo.userName%>'
                            tempData['activityDate'] = ''
                            tempData['activityEndDate'] = ''
                            tempData['sellerRegisterNum'] = ''
                        }

                        this.receiptList.push(tempData)
                        //else {
                        //    this.$toast('识别失败，发票照片不清晰，请上传更为清晰的发票或手工填写相关信息后提交！')
                        //    tempData['relativePerson'] = '无'
                        //    tempData["receiptAttachment"] = url
                        //    tempData['feeType'] = '识别错误票'
                        //    tempData['receiptDate'] = '';
                        //    tempData['receiptCode'] = '';
                        //    tempData['receiptAmount'] = 0;
                        //    tempData['receiptTax'] = 0;
                        //    tempData['receiptPlace'] = '';
                        //    tempData['receiptNum'] = '';
                        //    tempData['receiptPerson'] = '无'
                        //    tempData['activityDate'] = ''
                        //    tempData['activityEndDate'] = ''
                            
                        //    this.receiptList.push(tempData)
                        //}
                    }
                    this.$toast.clear();
                })
            },
            appendAllowance() {
                this.$dialog.confirm({
                  title: '提示',
                  message: '确认新增无票出差补贴吗'
                }).then(() => {
                    this.allowanceMap = {}
                    this.allowanceMap['relativePerson'] = ''
                    this.allowanceMap["receiptAttachment"] = ''
                    this.allowanceMap['feeType'] = '实报实销'
                    this.allowanceMap['receiptDate'] = ''
                    this.allowanceMap['receiptCode'] = ''
                    this.allowanceMap['receiptAmount'] = 0
                    this.allowanceMap['receiptTax'] = ''
                    this.allowanceMap['receiptPlace'] = ''
                    this.allowanceMap['receiptNum'] = '';
                    this.allowanceMap['receiptPerson'] = ''
                    this.allowanceMap['activityDate'] = ''
                    this.allowanceMap['activityEndDate'] = ''
                    this.allowanceMap['sellerRegisterNum'] = ''
                    this.allowanceMap['receiptType'] = '实报实销'
                    this.receiptList.unshift(this.allowanceMap)
                }).catch(() => {
                  // on cancel
                });
                
            },
            removeAllowance() {
                this.receiptList.splice(this.receiptList.indexOf(this.allowanceMap), 1)
                this.allowanceMap = null
            },
            deleteDetail() {
                this.receiptList.splice(this.clickIndex, 1)
            },
            overSize() {
                this.$toast('无法上传超过8m的图片！');
                return
            }
        },
        mounted: function () {
            //history.pushState(null, null, document.URL);
            //window.addEventListener('popstate', function () {
            //    history.pushState(null, null, document.URL);
            //});
            const _this = this

            let isIOS = false

            var userAgentInfo = navigator.userAgent;
            var Agents = ["iPhone", "iPad"];
            for (var v = 0; v < Agents.length; v++) {
                if (userAgentInfo.indexOf(Agents[v]) > 0) {
                    isIOS = true;
                    break;
                }
            }

            if (isIOS) {
                window.addEventListener('pagehide', function () {
                    web({
                        action: 'draft',
                        batchNo: _this.chooseBatchNo,
                        code: JSON.stringify(_this.chooseReimburseCode),
                        receipt: JSON.stringify(_this.receiptList),
                        receiptDesc: _this.receiptDesc
                    }).then(res => {

                    })
                })
            } else {
                window.onbeforeunload = function () {
                    web({
                        action: 'draft',
                        batchNo: _this.chooseBatchNo,
                        code: JSON.stringify(_this.chooseReimburseCode),
                        receipt: JSON.stringify(_this.receiptList),
                        receiptDesc: _this.receiptDesc
                    }).then(res => {

                    })
                }
            }
            
            if (GetQueryString('batchNo')) {
                // 重新提交 把原始数据带回
                const batchNo = GetQueryString('batchNo')
                this.chooseBatchNo = batchNo
                web({ action: 'getReSubmitData', batchNo: batchNo }).then(res => {
                    const data = res.data
                    this.receiptList = data
                    data.forEach((v, i) => {
                        //if (v['receiptAttachment'] !== '')
                        //    this.photoList.push({ url: v['receiptAttachment'] })
                        if (v['receiptType'] === '实报实销')
                            this.allowanceMap = v
                    })
                    //constPhoteList = this.photoList
                })
            } else {
                // 找回草稿
                web({ action: 'getDraftData'}).then(res => {
                    const data = res.data
                    if (data.length > 0) {
                        const batchNo = data[0].batchNo
                        this.chooseBatchNo = batchNo
                        web({ action: 'getReSubmitData', batchNo: batchNo }).then(res => {
                            const data = res.data
                            this.receiptList = data
                            data.forEach((v, i) => {
                                //if (v['receiptAttachment'] !== '')
                                //    this.photoList.push({ url: v['receiptAttachment'] })
                                if (v['receiptType'] === '实报实销')
                                    this.allowanceMap = v
                            })
                            //constPhoteList = this.photoList
                        })
                    }
                })
            }
        },
        destroyed() {
            //window.removeEventListener('popstate', this.goBack, false);
        },
    })
</script>
</html>
