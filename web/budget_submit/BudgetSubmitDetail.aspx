<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BudgetSubmitDetail.aspx.cs" Inherits="web_budget_submit_BudgetSubmitDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>预算提交详情页</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <meta name="description" content="" />
    <link rel="stylesheet" href="https://unpkg.com/element-ui/lib/theme-chalk/index.css">
    <style>
        h3{margin-left: 20px}
        .el-menu-item, .el-submenu__title {
            height: 80px;
            line-height: 40px;
            position: relative;
            white-space: normal;
            list-style: none;
        }
    </style>
</head>
<body>
<div id="total">
    <el-container>
        <el-aside>
            <h3>选择地区</h3>
            <el-menu :collapse="isCollapse">
                <el-menu-item v-for="(area,index) in areaData" index="1" @click="showDistrictDetail(area)">{{index+1}}.{{area.DepartmentName}}</el-menu-item>
            </el-menu>
        </el-aside>
        <el-container v-loading="loading">
            <h3>汇总</h3>
            <el-header>
                <el-row :gutter="20">
                    <el-col :span="4">
                        <el-input v-model="usedBudget" readonly>
                            <template slot="prepend">已提交额度</template>
                        </el-input>
                    
                    </el-col>
                    <el-col :span="4">
                        <el-input v-model="totalBudget" readonly>
                            <template slot="prepend">总可用额度</template>
                        </el-input>
                    </el-col>
                    <el-col :span="4">
                        <el-button type="primary" round @click="submitBudget" :disabled="readOnly">确定提交</el-button>
                    </el-col>
                </el-row>
            </el-header>
            <h3>详情数据</h3>
            <el-main> 
                <el-table :data="tableData" border style="width: 100%" @cell-dblclick="tableDbEdit">
                    <el-table-column v-if="tableData.length>0" type="index" width="50"></el-table-column>
                    <el-table-column v-for="header in tableHeaders" :label="header.label" :prop="header.prop" align="center">
                        <el-table-column v-show="header.children.length > 0" v-for="subHeader in header.children"  :label="subHeader.label" 
                                         :prop="subHeader.prop" align="center">
                        </el-table-column>
                    </el-table-column>
                    <div slot="empty">
                        <p :style="{'marginTop': '23px'}">暂无数据，请选择地区</p>
                    </div>
                </el-table>
            </el-main>
            <!-- 步骤条-->
            <h3>操作记录</h3>
            <el-footer>
                <el-steps :active="recordData.length" :space="150" v-show="recordData.length > 0">
                    <el-step v-for="record in recordData" :title="record.userName" :description="record.createTime"></el-step>
                </el-steps>
                <el-table v-show="recordData.length == 0"><p>暂无数据</p></el-table>
            </el-footer>
        </el-container>
    </el-container>
</div>
</body>
<script src="/Scripts/jquery.min.js"></script>
<script src="/Scripts/vue.js"></script>
<script src="https://unpkg.com/element-ui/lib/index.js"></script>
<script>
    var vue = new Vue({
        el: '#total',
        data: {
            areaData: [],
            tableData: [],
            tableHeaders: [],
            usedBudget: 0,
            totalBudget: 0,
            budgetLimitType: '',
            docNo: '',
            departmentId: '',
            readOnly: true,
            selfDepartmentId: '',
            recordData: [],
            loading: false,
            isCollapse: false,
        },
        methods: {
            tableDbEdit: function (row, column, cell, event) {
                if (this.readOnly == true) {
                    vue.$alert("该单据属于已提交单据，无权修改，只能查看！", '错误信息');
                    return;
                }
                if (cell.cellIndex > 3 && column.label!='可用额度') {
                    let originValue = event.target.innerHTML;
                    event.target.innerHTML = "";
                    let cellInput = document.createElement("input");
                    cellInput.value = "";
                    cellInput.setAttribute("type", "text");
                    cellInput.setAttribute("id", "tempInput");
                    cellInput.style.width = "80%";
                    cell.appendChild(cellInput);
                    document.getElementById("tempInput").focus();
                    cellInput.onblur = function () {
                        vue.usedBudget -= parseInt(originValue);
                        vue.usedBudget += parseInt(cellInput.value == "" ? "0" : cellInput.value);
                        if (cellInput.value == "") {
                            event.target.innerHTML = 0;
                        } else {
                            event.target.innerHTML = cellInput.value;
                        }

                        // 把修改的值绑定到tableData数据中
                        row[column.property] = cellInput.value;
                        cell.removeChild(cellInput);
                    };
                }
            },
            submitBudget: function () {
                submitBudget(this.tableData);
            },
            showDistrictDetail: function (area) {
                var departmentId = area.DepartmentId;
                var docNo = area.DocNo;
                var selfDepartmentId = area.SelfDepartmentId;

                this.departmentId = departmentId;
                if (docNo == "undefined" || docNo == "") {
                    this.docNo = "";
                } else {
                    this.docNo = docNo;
                }
                this.selfDepartmentId = selfDepartmentId;
                showDistrictDetail(this.departmentId, this.selfDepartmentId, this.docNo);
            }
        },
        mounted: function () {
            getBudgetSubmitDistrict();
        },
        filters: {
            numFilter: function(value) {
                var realVal = Number(value).toFixed(2);
                return realVal.toString();
            }
        }
    });

    function getBudgetSubmitDistrict() {
        $.ajax({
            url: 'BudgetSubmitList.aspx',
            data: { action: 'getDepartmentSubmitList' },
            dataType: 'json',
            type: 'post',
            success: function(msg) {
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    vue.areaData = JSON.parse(msg.Doc);
                }
            },
            error: function() {
                vue.$alert(msg.ErrMsg, '错误信息');
            }
        });
    }

    function showDistrictDetail(departmentId, selfDepartmentId, docNo) {
        vue.loading = true;
        $.ajax({
            url: 'BudgetSubmitDetail.aspx',
            data: { action: 'getBudgetDetail', departmentId: departmentId, selfDepartmentId: selfDepartmentId, docNo: docNo},
            dataType: 'json',
            type: 'post',
            success: function (msg) {
                vue.loading = false;
                vue.totalBudget = 0;
                vue.usedBudget = 0;
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                    vue.readOnly = false;
                } else {
                    vue.tableData = msg.Doc;
                    vue.tableHeaders = msg.Headers;
                    vue.readOnly = msg.readOnly;
                    vue.recordData = JSON.parse(msg.recordData);
                    // 取出总可用额度和已经填写额度
                    for (i = 0; i < vue.tableData.length; i++) {
                        for (var key in vue.tableData[i]) {
                            if (key.indexOf("可用额度") > -1) {
                                vue.totalBudget += parseFloat(vue.tableData[i][key]);
                            } else if (key.indexOf("Original") > -1) {
                                vue.usedBudget += parseFloat(vue.tableData[i][key]);
                            }
                        }
                    }
                    
                }
            },
            error: function () {
                vue.loading = false;
                vue.$alert("网络错误，请及时联系管理员", '错误信息');
            }
        });
    }

    function submitBudget(data) {
        vue.loading = true;
        $.ajax({
            url: 'BudgetSubmitDetail.aspx',
            data: {
                action: 'submitBudget',
                tableData: JSON.stringify(data),
                docNo: vue.docNo,
                selfDepartmentId: vue.selfDepartmentId
            },
            dataType: 'json',
            type: 'post',
            success: function(msg) {
                vue.loading = false;
                if (msg.ErrCode != 0) {
                    vue.$alert(msg.ErrMsg, '错误信息');
                } else {
                    vue.$alert('提交成功',
                        '提示',
                        {
                            confirmButtonText: '确定',
                            callback: action => {
                                location.href = "BudgetSubmitDetail.aspx";
                            }
                        });
                }
            },
            error: function() {
                vue.loading = false;
                vue.$alert('网络错误，请及时联系管理员', '错误信息');
            }
        });
    }

    function GetQueryString(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }
</script>
</html>
