<%@ Page Language="C#" AutoEventWireup="true" CodeFile="mCompanyFeeApproval.aspx.cs" Inherits="mCompanyFeeApproval" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta
        name="viewport"
        content="width=device-width, initial-scale=1.0, user-scalable=no"
    />
    <meta name="description" content="" />
    <title>公司费用审批</title>
    <style>
        [v-cloak] {
            display: none;
        }
        body {
            background-color: #f8f8f8;
        }
        .companyClass{
            font-size: 14px !important;
            font-style: italic
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
                right-text="审批"
                @click-left="refresh"
                @click-right="showApproveDialog"
            >
            </van-nav-bar>
        <div style="padding: 30px 0"></div>
        <van-grid :column-num="3" clickable :gutter="10">
            <van-grid-item text="人员工资" :info="calculateNum('wages')" @click="chooseCompany('wages')">
                <van-icon
                    name="manager-o"
                    slot="icon"
                    size="24"
                    color="red"
                ></van-icon>
            </van-grid-item>
            <van-grid-item text="编外人员工资" :info="calculateNum('outer_wages')" @click="chooseCompany('outer_wages')">
                <van-icon
                    name="friends-o"
                    slot="icon"
                    size="24"
                    color="red"
                ></van-icon>
            </van-grid-item>
            <van-grid-item text="税金" :info="calculateNum('tax')" @click="chooseCompany('tax')">
                <van-icon
                    name="balance-o"
                    slot="icon"
                    size="24"
                    color="red"
                ></van-icon>
            </van-grid-item>
            <van-grid-item text="利息" :info="calculateNum('interest')">
                <van-icon
                    name="cashier-o"
                    slot="icon"
                    size="24"
                    color="red"
                ></van-icon>
            </van-grid-item>
            <van-grid-item text="折旧" :info="calculateNum('depreciation')" @click="chooseCompany('depreciation')">
                <van-icon
                    name="hotel-o"
                    slot="icon"
                    size="24"
                    color="red"
                ></van-icon>
            </van-grid-item>
            <van-grid-item text="摊销" :info="calculateNum('amortize')" @click="chooseCompany('amortize')">
                <van-icon
                    name="points"
                    slot="icon"
                    size="24"
                    color="red"
                ></van-icon>
            </van-grid-item>
        </van-grid>
        <van-divider dashed v-show="detailShow" style="font-size:10px">{{date}} {{company}}</van-divider>
        <van-loading v-show="loadingShow" size="24px" vertical>加载中...</van-loading>
        <div style="padding: 10px 0" v-show="detailShow"></div>
        <!-- 人员工资table -->
        <el-table stripe :data="tableData" style="width: 100%" size="mini" show-summary v-show="detailShow && type === 'wages'">
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
        <el-table stripe :data="tableData" style="width: 100%" size="mini" show-summary v-show="detailShow && type === 'outer_wages'">
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
        <el-table stripe :data="tableData" style="width: 100%" size="mini" show-summary v-show="detailShow && type === 'tax'">
            <el-table-column prop="TaxType" label="税种" width="100"></el-table-column>
            <el-table-column prop="TaxAmount" label="金额" width="100"></el-table-column>
            <el-table-column prop="TaxBasis" label="计税依据" width="100"></el-table-column>
            <el-table-column prop="TaxRate" label="税率" width="100"></el-table-column>
            <el-table-column prop="Remark" label="备注" width="100"></el-table-column>
        </el-table>
        <!-- 折旧table-->
        <el-table stripe :data="tableData" style="width: 100%" size="mini" show-summary v-show="detailShow && type === 'depreciation'">
            <el-table-column prop="Type" label="类型" width="100"></el-table-column>
            <el-table-column prop="Name" label="名称" width="100"></el-table-column>
            <el-table-column prop="OriginValue" label="原值" width="100"></el-table-column>
            <el-table-column prop="CurrentD" label="本期折旧" width="100"></el-table-column>
            <el-table-column prop="AccumulatedD" label="累计折旧" width="100"></el-table-column>
            <el-table-column prop="NetValue" label="净值" width="100"></el-table-column>
        </el-table>
        <!-- 摊销table-->
        <el-table stripe :data="tableData" style="width: 100%" size="mini" show-summary v-show="detailShow && type === 'amortize'">
            <el-table-column prop="Type" label="类型" width="100"></el-table-column>
            <el-table-column prop="Project" label="项目" width="100"></el-table-column>
            <el-table-column prop="OriginValue" label="原值" width="100"></el-table-column>
            <el-table-column prop="CurrentA" label="本月摊销" width="100"></el-table-column>
            <el-table-column prop="AccumulatedA" label="累计摊销" width="100"></el-table-column>
            <el-table-column prop="FinalValue" label="期末余额" width="100"></el-table-column>
            <el-table-column prop="Remark" label="备注" width="100"></el-table-column>
        </el-table>
        <!-- 选择公司弹出框 -->
        <van-action-sheet v-model="dialogShow" close-on-click-action :actions="actions" title="待审批公司" round @select="onSelect"></van-action-sheet>
        <!-- 审批弹出框 -->
        <van-dialog
            v-model="approveShow"
            title="审批"
            confirm-button-text="同意"
            cancel-button-text="拒绝"
            show-cancel-button
            close-on-click-overlay
            @confirm="approve('同意')"
            @cancel="approve('不同意')"
        >
            <van-cell-group>
                <van-field
                    v-model="opinion"
                    rows="3"
                    autosize
                    label="审批意见"
                    type="textarea"
                    placeholder="请输入审批意见，若拒绝则审批意见必填"
                />
            </van-cell-group>
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
                approvalNum: {},
                dialogShow: false,
                actions: [],
                company: '',
                loadingShow: false,
                detailShow: false,
                tableData: [],
                date: '',
                opinion: '',
                approveShow: false,
                docCode: '',
                type: ''
            }
        },
        methods: {
            refresh() {
                location.reload()
            },
            showApproveDialog() {
                if (this.tableData.length === 0) {
                    this.$toast({
                        type: 'fail',
                        message: '暂无数据审批'
                    });
                    return
                }
                this.approveShow = true
            },
            approve(result) {
                if (result === '不同意' && this.opinion === '') {
                    this.$toast({
                        type: 'fail',
                        message: '拒绝理由必填'
                    })
                    return
                }
                $.ajax({
                    url: 'mCompanyFeeApproval.aspx',
                    data: {
                        act: 'approve',
                        type: this.type,
                        docCode: this.docCode,
                        result: result,
                        opinion: this.opinion,
                    },
                    type: 'post',
                    dataType: 'json',
                    success: res => {
                        if (res.code === 200) {
                            this.$toast({
                                type: 'fail',
                                message: '审批完成'
                            })
                        } else {
                            this.$toast({
                                type: 'fail',
                                message: '审批失败，请及时联系管理员'
                            })
                        }
                        location.reload()
                    }
                })
            },
            chooseCompany(type) {
                this.type = type
                this.actions = []
                if (this.approvalNum[type] === '') {
                    this.$toast({
                        type: 'fail',
                        message: '暂无待审批单据'
                    });
                    return
                }
                const companyArray = JSON.parse(this.approvalNum[type])

                companyArray.map(item => {
                    this.actions.push({ name: item.company, className: 'companyClass'})
                })

                this.dialogShow = true
            },
            onSelect(item) {
                this.company = item.name
                this.loadingShow = true
                $.ajax({
                    url: 'mCompanyFeeApproval.aspx',
                    data: {
                        act: 'getData',
                        company: item.name,
                        type: this.type
                    },
                    type: 'post',
                    dataType: 'json',
                    success: res => {
                        this.detailShow = true
                        this.loadingShow = false
                        this.tableData = res
                        this.docCode = res[0].DocCode
                        this.date = res[0].Year + '-' + res[0].Month
                    }
                })
            }
        },
        mounted() {
            // 获取所有类型待审批的数量
            $.ajax({
                url: 'mCompanyFeeApproval.aspx',
                data: {
                    act: 'getApprovalNum',
                },
                type: 'post',
                dataType: 'json',
                success: res => {
                    this.approvalNum = res
                }
            })
        },
        computed: {
            calculateNum() {
                return function (type) {
                    if (this.approvalNum[type] && this.approvalNum[type] !== '') {
                        return JSON.parse(this.approvalNum[type]).length
                    }
                    else
                        return ''
                }
            }
        }
    })
</script>
</html>
