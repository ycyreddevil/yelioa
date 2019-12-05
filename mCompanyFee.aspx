<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mCompanyFee.aspx.cs"
Inherits="mCompanyFee" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <meta
            name="viewport"
            content="width=device-width, initial-scale=1.0, user-scalable=no"
        />
        <meta name="description" content="" />
        <title>公司费用提交</title>
        <style>
            [v-cloak] {
                display: none;
            }
            body {
                background-color: #f8f8f8;
            }
        </style>
        <link
            rel="stylesheet"
            href="https://cdn.jsdelivr.net/npm/vant@2.2.13/lib/index.css"
        />
        <link
            rel="stylesheet"
            href="https://unpkg.com/element-ui/lib/theme-chalk/index.css"
        />
    </head>
    <body>
        <div id="vue" v-cloak>
            <van-nav-bar
                title="11月企业付款"
                fixed
                left-text="刷新"
                right-text="提交"
                @click-left="refresh"
                @click-right="showApprover"
            >
            </van-nav-bar>
            <div style="padding: 30px 0"></div>
            <van-grid :column-num="3" clickable :gutter="10">
                <van-grid-item text="人员工资" @click="type = 1; dialogShow = true">
                    <van-icon
                        name="manager-o"
                        slot="icon"
                        size="24"
                        color="red"
                    ></van-icon>
                </van-grid-item>
                <van-grid-item text="编外人员工资">
                    <van-icon
                        name="friends-o"
                        slot="icon"
                        size="24"
                        color="blue"
                    ></van-icon>
                </van-grid-item>
                <van-grid-item text="税金" @click="type = 3; dialogShow = true">
                    <van-icon
                        name="balance-o"
                        slot="icon"
                        size="24"
                        color="orange"
                    ></van-icon>
                </van-grid-item>
                <van-grid-item text="利息">
                    <van-icon
                        name="cashier-o"
                        slot="icon"
                        size="24"
                        color="green"
                    ></van-icon>
                </van-grid-item>
                <van-grid-item text="折旧" @click="type = 5; dialogShow = true">
                    <van-icon
                        name="hotel-o"
                        slot="icon"
                        size="24"
                        color="purple"
                    ></van-icon>
                </van-grid-item>
                <van-grid-item text="摊销" @click="type = 6; dialogShow = true">
                    <van-icon
                        name="points"
                        slot="icon"
                        size="24"
                        color="goldenrod"
                    ></van-icon>
                </van-grid-item>
            </van-grid>
            <van-divider dashed v-show="detailShow" style="font-size:10px">{{date}} {{company}}</van-divider>
            <van-uploader v-show="detailShow" style="margin-left:10px" accept="application/msexcel" :after-read="afterRead" result-type="file">
                <van-button icon="label" type="info">当月数据模板上传</van-button>
            </van-uploader>
            <van-loading v-show="loadingShow" size="24px" vertical>加载中...</van-loading>
            <div style="padding: 10px 0" v-show="detailShow"></div>
            <!-- 人员工资table -->
            <el-table stripe :data="tableData" style="width: 100%" size="mini" show-summary v-show="detailShow && type === 1">
                <el-table-column fixed prop="Name" label="姓名" width="80"></el-table-column>
                <el-table-column prop="Department" label="部门" width="100"></el-table-column>
                <el-table-column prop="Position" label="职务" width="80"></el-table-column>
                <el-table-column prop="AttendanceDay" label="考勤" width="80"></el-table-column>
                <el-table-column prop="AbsenceDay" label="缺勤天数" width="80"></el-table-column>
                <el-table-column label="固定工资">
                    <el-table-column prop="PositionSalary" label="职位工资" width="100"></el-table-column>
                    <el-table-column prop="SecretSalary" label="保密工资" width="100"></el-table-column>
                    <el-table-column prop="LevelSalary" label="档位工资" width="100"></el-table-column>
                    <el-table-column prop="TechnicalSalary" label="技衔工资" width="100"></el-table-column>
                    <el-table-column prop="OtherSalary" label="其他工资" width="100"></el-table-column>
                </el-table-column>
                <el-table-column prop="StableSalary" label="固定工资小计" width="100"></el-table-column>
                <el-table-column label="岗位津贴">
                    <el-table-column prop="EducationAllowance" label="学历津贴" width="100"></el-table-column>
                    <el-table-column prop="OtherAllowance" label="其他津贴" width="100"></el-table-column>
                </el-table-column>
                <el-table-column prop="TotalStableSalary" label="固定工资合计" width="100"></el-table-column>
                <el-table-column label="绩效及奖励">
                    <el-table-column prop="MonthlyPerformance" label="月度绩效" width="100"></el-table-column>
                    <el-table-column prop="QuarterlyPerformance" label="季度绩效" width="100"></el-table-column>
                    <el-table-column prop="YearlyPerformance" label="年度绩效" width="100"></el-table-column>
                    <el-table-column prop="ActualPerformance" label="实发绩效" width="100"></el-table-column>
                    <el-table-column prop="CSalesBonus" label="纯销代表奖金" width="100"></el-table-column>
                    <el-table-column prop="QSalesBonus" label="渠道代表奖金" width="100"></el-table-column>
                    <el-table-column prop="HeadBonus" label="主管奖金" width="100"></el-table-column>
                    <el-table-column prop="ManageBonus" label="管理层奖金" width="100"></el-table-column>
                    <el-table-column prop="UnusualBonus" label="异样考核" width="100"></el-table-column>
                    <el-table-column prop="TesterBonus" label="实验奖金" width="100"></el-table-column>
                    <el-table-column prop="OvertimeBonus" label="加班" width="100"></el-table-column>
                    <el-table-column prop="ProductBonus" label="生产产能" width="100"></el-table-column>
                    <el-table-column prop="GMBonus" label="总经理特别贡献" width="100"></el-table-column>
                    <el-table-column prop="RDBonus" label="研发项目奖金" width="100"></el-table-column>
                    <el-table-column prop="OtherBonus1" label="其他奖励1" width="100"></el-table-column>
                    <el-table-column prop="OtherBonus2" label="其他奖励2" width="100"></el-table-column>
                    <el-table-column prop="TotalBonus" label="奖励合计" width="100"></el-table-column>
                </el-table-column>
                <el-table-column prop="TotalPerformanceBonus" label="绩效及奖励小计" width="100"></el-table-column>
                <el-table-column label="工资减项">
                    <el-table-column prop="AttendaceFine" label="考勤扣款" width="100"></el-table-column>
                    <el-table-column prop="SanitaryFine" label="卫生检查罚款" width="100"></el-table-column>
                    <el-table-column prop="AbsenceFine" label="出勤扣款" width="100"></el-table-column>
                    <el-table-column prop="ExpireFine" label="产品过期罚款" width="100"></el-table-column>
                    <el-table-column prop="ViolationFine" label="违反制度罚款" width="100"></el-table-column>
                    <el-table-column prop="OtherFine1" label="其他罚款1" width="100"></el-table-column>
                    <el-table-column prop="OtherFine2" label="其他罚款2" width="100"></el-table-column>
                    <el-table-column prop="TotalFine" label="罚款合计" width="100"></el-table-column>
                    <el-table-column prop="MutualFund" label="代扣基金" width="100"></el-table-column>
                    <el-table-column prop="OtherFund" label="其它减项" width="100"></el-table-column>
                </el-table-column>
                <el-table-column prop="TotalFineAndFund" label="工资减项小计" width="100"></el-table-column>
                <el-table-column prop="TotalPay" label="应发合计" width="100"></el-table-column>
                <el-table-column prop="TotalPay1" label="应发1" width="100"></el-table-column>
                <el-table-column prop="TotalPay2" label="应发2" width="100"></el-table-column>
                <el-table-column label="代扣代缴款">
                    <el-table-column prop="SocialInsurance" label="社保" width="100"></el-table-column>
                    <el-table-column prop="PublicFund" label="公积金" width="100"></el-table-column>
                    <el-table-column prop="PersonalTax" label="个人所得税" width="100"></el-table-column>
                    <el-table-column prop="SocialInsurance1" label="社保1" width="100"></el-table-column>
                    <el-table-column prop="PublicFund1" label="公积金1" width="100"></el-table-column>
                    <el-table-column prop="PersonalTax1" label="个人所得税1" width="100"></el-table-column>
                </el-table-column>
                <el-table-column prop="Rent" label="代扣房租" width="100"></el-table-column>
                <el-table-column prop="PersonalTaxAdjust" label="个税调整项" width="100"></el-table-column>
                <el-table-column prop="PersonalTaxAdjust1" label="个税调整项1" width="100"></el-table-column>
                <el-table-column prop="ActualPay" label="实发工资" width="100"></el-table-column>
                <el-table-column prop="ActualPay1" label="实发1" width="100"></el-table-column>
                <el-table-column prop="ActualPay2" label="实发2" width="100"></el-table-column>
                <el-table-column prop="ActualPay2Decimal" label="保留两位数" width="100"></el-table-column>
                <el-table-column prop="Remark" label="备注" width="100"></el-table-column>
            </el-table>
            <!-- 外部人员工资table -->
            <el-table stripe :data="tableData" style="width: 100%" size="mini" show-summary v-show="detailShow && type === 2">
                <el-table-column fixed prop="Name" label="姓名" width="80"></el-table-column>
                <el-table-column prop="Department" label="部门" width="100"></el-table-column>
                <el-table-column prop="Position" label="职务" width="80"></el-table-column>
                <el-table-column prop="WorkingHour" label="出勤小时" width="80"></el-table-column>
                <el-table-column prop="WorkingPrice" label="计件标准（元/件）" width="80"></el-table-column>
                <el-table-column prop="WorkingAmount" label="计件数" width="100"></el-table-column>
                <el-table-column prop="TotalSalary" label="计件工资合计" width="100"></el-table-column>
                <el-table-column label="绩效及奖励">
                    <el-table-column prop="ActualPerformance" label="实发绩效" width="100"></el-table-column>
                    <el-table-column prop="Bonus" label="奖励金额" width="100"></el-table-column>
                </el-table-column>
                <el-table-column prop="TotalPerformanceBonus" label="绩效及奖励小计" width="100"></el-table-column>
                <el-table-column label="扣款">
                    <el-table-column prop="RubbishFine" label="残次品扣款" width="100"></el-table-column>
                    <el-table-column prop="ProductFine" label="产品扣罚" width="100"></el-table-column>
                    <el-table-column prop="Fine" label="罚款" width="100"></el-table-column>
                    <el-table-column prop="MutualFund" label="代扣基金" width="100"></el-table-column>
                    <el-table-column prop="OtherFund" label="其它" width="100"></el-table-column>
                </el-table-column>
                <el-table-column prop="TotalFineAndFund" label="工资减项小计" width="100"></el-table-column>
                <el-table-column prop="TotalPay" label="应发合计" width="100"></el-table-column>
                <el-table-column label="代扣代缴款">
                    <el-table-column prop="SocialInsurance" label="社保" width="100"></el-table-column>
                    <el-table-column prop="PublicFund" label="公积金" width="100"></el-table-column>
                    <el-table-column prop="PersonalTax" label="个人所得税" width="100"></el-table-column>
                </el-table-column>
                <el-table-column prop="Rent" label="代扣房租" width="100"></el-table-column>
                <el-table-column prop="ActualPay" label="实发工资" width="100"></el-table-column>
                <el-table-column prop="ActualPay2Decimal" label="保留两位数" width="100"></el-table-column>
                <el-table-column prop="Remark" label="备注" width="100"></el-table-column>
            </el-table>
            <!-- 税金table-->
            <el-table stripe :data="tableData" style="width: 100%" size="mini" show-summary v-show="detailShow && type === 3">
                <el-table-column prop="TaxType" label="税种" width="100"></el-table-column>
                <el-table-column prop="TaxAmount" label="金额" width="100"></el-table-column>
                <el-table-column prop="TaxBasis" label="计税依据" width="100"></el-table-column>
                <el-table-column prop="TaxRate" label="税率" width="100"></el-table-column>
                <el-table-column prop="Remark" label="备注" width="100"></el-table-column>
            </el-table>
            <!-- 折旧table-->
            <el-table stripe :data="tableData" style="width: 100%" size="mini" show-summary v-show="detailShow && type === 5">
                <el-table-column prop="Type" label="类型" width="100"></el-table-column>
                <el-table-column prop="Name" label="名称" width="100"></el-table-column>
                <el-table-column prop="OriginValue" label="原值" width="100"></el-table-column>
                <el-table-column prop="CurrentD" label="本期折旧" width="100"></el-table-column>
                <el-table-column prop="AccumulatedD" label="累计折旧" width="100"></el-table-column>
                <el-table-column prop="NetValue" label="净值" width="100"></el-table-column>
            </el-table>
            <!-- 摊销table-->
            <el-table stripe :data="tableData" style="width: 100%" size="mini" show-summary v-show="detailShow && type === 6">
                <el-table-column prop="Type" label="类型" width="100"></el-table-column>
                <el-table-column prop="Project" label="项目" width="100"></el-table-column>
                <el-table-column prop="OriginValue" label="原值" width="100"></el-table-column>
                <el-table-column prop="CurrentA" label="本月摊销" width="100"></el-table-column>
                <el-table-column prop="AccumulatedA" label="累计摊销" width="100"></el-table-column>
                <el-table-column prop="FinalValue" label="期末余额" width="100"></el-table-column>
                <el-table-column prop="Remark" label="备注" width="100"></el-table-column>
            </el-table>
            <!-- 选择公司和月份弹出框 -->
            <van-dialog
                v-model="dialogShow"
                show-cancel-button
                @confirm="showData"
                close-on-click-overlay
            >
                <h4 slot="title" style="margin-top:0px">确定公司和月份</h4>
                <van-cell-group>
                    <van-field
                        label-align="center"
                        readonly
                        placeholder="请选择月份"
                        v-model="date"
                        label="选择月份"
                        @click="isShowDate = true"
                    ></van-field>
                    <van-field
                        label-align="center"
                        readonly
                        placeholder="请选择公司"
                        v-model="company"
                        label="选择公司"
                        @click="isShowCompany = true"
                    ></van-field>
                </van-cell-group>
            </van-dialog>
            <!-- 月份选择框 -->
            <van-popup v-model="isShowDate" position="bottom">
                <van-datetime-picker
                    v-model="nowDate"
                    @cancel="cancelDate"
                    @confirm="confirmDate"
                    type="year-month"
                    :formatter="formatter"
                ></van-datetime-picker>
            </van-popup>
            <!-- 公司选择框 -->
            <van-popup v-model="isShowCompany" position="bottom">
                <van-picker
                    show-toolbar
                    :columns="columns"
                    @cancel="onCancelCompany"
                    @confirm="onConfirmCompany"
                />
            </van-popup>
            <!-- 审批流弹出框 -->
            <van-dialog
              v-model="processShow"
              show-cancel-button
              @confirm="submit"
            >
                <h4 slot="title" style="margin-top:0px">确定审批流</h4>
                <van-steps style="margin-left:10px" direction="vertical" :active="0">
                    <van-step v-for="(user,index) in approver" v-bind:key="index">
                        {{user.userName}}
                    </van-step>
                </van-steps>
                <%--<el-timeline>
                    <el-timeline-item v-for="(user,index) in approver" v-bind:key="index">{{user.userName}}</el-timeline-item>
                </el-timeline>--%>
            </van-dialog>
        </div>
    </body>
    <script src="Scripts/vue.js"></script>
    <script src="Scripts/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/vant@2.2.13/lib/vant.min.js"></script>
    <script src="https://unpkg.com/element-ui/lib/index.js"></script>
    <script>
        var vue = new Vue({
            el: '#vue',
            data() {
                return {
                    outerBorder: false,
                    detailShow: false,
                    dialogShow: false,
                    date: '',
                    company: '',
                    isShowDate: false,
                    isShowCompany: false,
                    nowDate: new Date(),
                    type: 0,
                    columns: ['江西东森科技发展有限公司', '江西业力医疗器械有限公司', '南昌市中申医疗器械有限公司', '南昌老康科技有限公司', '江西业力科技集团有限公司',
                        '天津吉诺泰普生物科技有限公司', '南昌业力医学检验实验室有限公司', '九江傲沐科技发展有限公司', '南昌业力生物科技有限公司', '上海会帆企业管理咨询中心', '上海恩焯企业管理咨询中心'],
                    tableData: [],
                    loadingShow: false,
                    processShow: false,
                    approver: [],
                    hasUpload: false
                }
            },
            methods: {
                refresh() {
                    location.reload()
                },
                afterRead(file) {
                    this.loadingShow = true
                    this.hasUpload = true
                    var formdata = new FormData();
                    formdata.append("file", file.file);

                    if (this.type === 1)
                        formdata.append("act", "uploadWageExcel")
                    if (this.type === 2)
                        formdata.append("act", "uploadOuterWageExcel")
                    else if (this.type === 3)
                        formdata.append("act", "uploadTaxExcel")
                    else if (this.type === 5)
                        formdata.append("act", "uploadDepreciationExcel")
                    else if (this.type === 6)
                        formdata.append("act", "uploadAmortizeExcel")

                    formdata.append("year", this.date.split('-')[0])
                    formdata.append("month", this.date.split('-')[1])
                    formdata.append("company", this.company)
                    $.ajax({
                        url: 'mCompanyFee.aspx',
                        data: formdata,
                        contentType: false,
                        cache: false,
                        processData: false,
                        type: 'post',
                        success: res => {
                            this.detailShow = true
                            this.tableData = JSON.parse(res)
                            this.loadingShow = false
                        }
                    })
                },
                formatter(type, value) {
                    if (type === 'year') {
                        return `${value}年`
                    } else if (type === 'month') {
                        return `${value}月`
                    }
                },
                cancelDate() {
                    this.isShowDate = false
                },
                confirmDate(date) {
                    this.date = date.getFullYear() + '-' + (date.getMonth() + 1)
                    this.isShowDate = false
                },
                onCancelCompany() {
                    this.isShowCompany = false
                },
                onConfirmCompany(value) {
                    this.company = value
                    this.isShowCompany = false
                },
                showData() {
                    if (this.date === '') {
                        this.$toast({
                            type: 'fail',
                            message: '请先选择月份'
                        });
                        return
                    }
                    if (this.company === '') {
                        this.$toast({
                            type: 'fail',
                            message: '请先选择公司'
                        });
                        return
                    }

                    this.loadingShow = true
                    $.ajax({
                        url: 'mCompanyFee.aspx',
                        data: {
                            act: 'getData',
                            year: this.date.split('-')[0],
                            month: this.date.split('-')[1],
                            company: this.company,
                            type: this.type
                        },
                        type: 'post',
                        dataType: 'json',
                        success: res => {
                            this.detailShow = true
                            this.tableData = res
                            this.loadingShow = false
                        }
                    })
                },
                showApprover() {
                    if (!this.detailShow) {
                        this.$toast({
                            type: 'fail',
                            message: '请先选择类型'
                        });
                        return
                    }
                    if (!this.hasUpload) {
                        this.$toast({
                            type: 'fail',
                            message: '请先上传excel'
                        });
                        return
                    }

                    $.ajax({
                        url: 'mCompanyFee.aspx',
                        data: {
                            act: 'getApprover',
                            type: this.type
                        },
                        type: 'post',
                        dataType: 'json',
                        success: res => {
                            this.processShow = true
                            this.approver = res
                            this.hasUpload = false
                        }
                    })
                },
                submit() {
                    $.ajax({
                        url: 'mCompanyFee.aspx',
                        data: {
                            act: 'submit',
                            year: this.date.split('-')[0],
                            month: this.date.split('-')[1],
                            company: this.company,
                            type: this.type,
                            tableData: JSON.stringify(this.tableData),
                            approver: JSON.stringify(this.approver)
                        },
                        type: 'post',
                        dataType: 'json',
                        success: res => {
                            this.processShow = false
                            this.detailShow = false
                            if (res.code === 200) {
                                this.$toast({
                                    type: 'success',
                                    message: '提交成功'
                                });
                            } else {
                                this.$toast({
                                    type: 'fail',
                                    message: res.msg
                                });
                            }
                        }
                    })
                }
            }
        })
    </script>
</html>
